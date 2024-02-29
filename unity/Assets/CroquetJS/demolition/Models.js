// Demolition Demo

import { Actor, AM_Spatial, mix, sphericalRandom, v3_scale, v3_normalize, v3_sub, v3_magnitude, User, UserManager } from "@croquet/worldcore-kernel";
import { RapierManager, AM_RapierWorld, AM_RapierRigidBody, RAPIER } from "@croquet/worldcore-rapier";
import { GameModelRoot, AM_InitializationClient } from "@croquet/game-models";

function rgb(r, g, b) {
    return [r / 255, g / 255, b / 255];
}

// every actor that needs a representation in the game engine should inherit
// from GameActor.
// because all objects in this app are governed by Rapier physics, GameActor
// includes a Rapier mixin.
export class GameActor extends mix(Actor).with(AM_Spatial, AM_RapierRigidBody) {
    static okayToIgnore() { return [...super.okayToIgnore(), '$rigidBody'] }

    get pawn() { return 'GamePawn' } // if not otherwise specialised
    get gamePawnType() { return this._type } // Unity prefab to use
    get type() { return this._type || "primitiveCube" }
    get color() { return this._color || [0.5, 0.5, 0.5] }
    get alpha() { return this._alpha === undefined ? 1 : this._alpha }
}
GameActor.register('GameActor');

class DynamicDemolitionActor extends GameActor {
    init(options) {
        options.rigidBodyType = 'dynamic';
        super.init(options);
        this.worldActor.dynamics.add(this);
    }

    destroy() {
        super.destroy();
        this.worldActor.dynamics.delete(this);
    }

    translationSet(t) {
        if (t[1] > -20) return;
        // console.log("kill plane");
        this.future(0).destroy();
    }
}
DynamicDemolitionActor.register('DynamicDemolitionActor');

//------------------------------------------------------------------------------------------
//-- BlockActor ----------------------------------------------------------------------------
//------------------------------------------------------------------------------------------
class BlockActor extends DynamicDemolitionActor {
    get pawn() { return 'BlockPawn' } // no point declaring in every instantiation
    get shape() { return this._shape || "111" } // we don't hold (or snapshot) any _shape value for the cubes

    init(options) {
        options.ccdEnabled = false;
        // minor hack: if initialised from Unity, the options will contain a type value
        // which is the name of a prefab.  in that case, make sure we have the
        // corresponding shape value for when we initialise the collider.  in line
        // with the getter above, lack of shape value implies a cube.
        if (options.type) {
            const shape = { WoodColumn: "121", WoodPlatform: "414" }[options.type];
            if (shape) options.shape = shape;
        }
        super.init(options);
        this.buildCollider();
        this._color = [1, 1, 1]; // full brightness
    }

    buildCollider() {
        let d;
        switch (this.shape) {
            case "121":
                d = [0.5, 1, 0.5];
                this._type = "WoodColumn";
                break;
            case "414":
                d = [2, 0.5, 2];
                this._type = "WoodPlatform";
                break;
            case "111":
                d = [0.5, 0.5, 0.5];
                this._type = "WoodCube";
                break;
            default:
                throw Error("UNKNOWN SHAPE!");
        }
        const cd = RAPIER.ColliderDesc.cuboid(...d);
        cd.setDensity(1);
        cd.setFriction(0.5);
        cd.setRestitution(0.2);
        this.createCollider(cd);
    }
}
BlockActor.register('BlockActor');


//------------------------------------------------------------------------------------------
//-- BulletActor ------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class BulletActor extends DynamicDemolitionActor {
    get pawn() { return 'BulletPawn' }
    get index() { return this._index }

    init(options) {
        super.init(options);
        this._type = "Projectile";
        this.buildCollider();
    }

    buildCollider() {
        const cd = RAPIER.ColliderDesc.ball(0.5);
        cd.setDensity(5);
        cd.setRestitution(0.95);
        this.createCollider(cd);
    }
}
BulletActor.register('BulletActor');


//------------------------------------------------------------------------------------------
//-- BarrelActor -----------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class BarrelActor extends DynamicDemolitionActor {
    get pawn() { return 'BarrelPawn' }
    init(options) {
        options.ccdEnabled = false;
        super.init(options);
        this.buildCollider();

        this._type = "TNT";
        this._color = [1, 1, 1];

        this.future(2000).set({ hasAccelerometer: true });
    }

    arm() {
        this.set({ hasAccelerometer: true });
    }

    buildCollider() {
        const cd = RAPIER.ColliderDesc.cylinder(0.5, 0.5);
        cd.setDensity(1);
        cd.setRestitution(0.2);
        this.createCollider(cd);
    }

    accelerationSet(acceleration) {
        const a = v3_magnitude(acceleration);
        if (a > 27) {
            this.lightFuse();
        }
    }

    lightFuse() {
        this.set({ hasAccelerometer: false });
        this.say('fuseLit');
        this.future(500).explode();
    }

    explode() {
        const radius = 14;
        const world = this.getWorldActor();
        world.dynamics.forEach(block => {
            const to = v3_sub(block.translation, this.translation);
            const distance = v3_magnitude(to);
            if (distance === 0) return; // presumably the barrel itself

            // calculate a ratio with inverse-square drop-off to zero
            // at the blast radius
            const ratio = (radius - distance) / radius; // 1 at centre; 0 at radius
            if (ratio < 0) return; // too far to be affected

            const MAX_F = 45;
            const force = MAX_F * ratio * ratio;
            const aim = v3_normalize(to);
            const push = v3_scale(aim, force);
            block.rigidBody.applyImpulse(new RAPIER.Vector3(...push), true);
        });
        this.say('exploded');
        this.future(1000).destroy(); // stick around a bit for the explosion to be communicated, and seen
    }

}
BarrelActor.register('BarrelActor');


// static objects representing the scene boundaries
class EnvironmentActor extends GameActor {
    // get pawn() { return 'EnvironmentPawn'; } no - use default GamePawn
    get gamePawnType() { return '' } // no Unity pawn

    init(options) {
        options.rigidBodyType = 'static';
        super.init(options);
        this._type = 'Table';
        this.buildCollider(options.colliderSize, options.colliderPosition);
        this._color = [1, 1, 1];
    }

    buildCollider(size, position) {
        const cd = RAPIER.ColliderDesc.cuboid(...size.map(l => l * 1.0));
        cd.setTranslation(...position);
        this.createCollider(cd);
    }
}
EnvironmentActor.register('EnvironmentActor');

//------------------------------------------------------------------------------------------
//-- BaseActor ------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class BaseActor extends mix(Actor).with(AM_Spatial, AM_RapierWorld, AM_InitializationClient) {
    get pawn() { return 'BasePawn' }
    get gamePawnType() { return '' } // no Unity pawn

    init(options) {
        super.init(options);
        this.active = [];
        this.dynamics = new Set();

        // create a rigid object for the table, with a view in Unity but
        // not in THREE
        EnvironmentActor.create({
            parent: this,
            position: [0, 0, 0],
            colliderSize: [20, 2, 20],
            colliderPosition: [0, -2, 0]
        });

        this.subscribe("ui", "shoot", this.shoot);

        this.versionBump = 0;
    }

    onPrepareForInitialization() {
        this.destroyAllDynamics(); // $$$ perhaps no longer just dynamics?
    }

    onInitializationStart() {
        if (this.service('InitializationManager').activeScene === 'demolition4') {
            this.buildAll();
        }
    }

    onObjectInitialization(cls, spec) {
        // all objects created in this world are children of this object
        spec.parent = this;
        cls.create(spec);
    }

    shoot(data) {
        // data is a string array with viewId and a comma-separated launch position
        const [ viewId, positionStr ] = data;
        const gun = positionStr.split(',').map(Number);
        const index = this.service('UserManager').user(viewId).index;
        const aim = v3_normalize(v3_sub([0, 0, 1], gun));
        const translation = gun; // v3_add(gun, [0, 0, 0]);
        const color = this.wellKnownModel("ModelRoot").colors[index];
        const bullet = BulletActor.create({ parent: this, index, color, translation });
        const force = v3_scale(aim, 95);
        const spin = v3_scale(sphericalRandom(), Math.random() * 5);
        bullet.rigidBody.applyImpulse(new RAPIER.Vector3(...force), true);
        bullet.rigidBody.applyTorqueImpulse(new RAPIER.Vector3(...spin), true);
    }

    destroyAllDynamics() {
        Array.from(this.dynamics).forEach(b => b.destroy()); // don't iterate on the set itself while removing
    }

    buildAll() {
        // this.buildAll_max(); // hardcore
        // this.buildAll_full();
        this.buildAll_mini();
        // this.buildAll_minimal();
    }

    buildAll_full() {
        const baseY = 0; // how high off the ground the building starts

        // centre buildings all have TNT barrels
        this.buildBuilding(2, baseY, 2, true);
        this.buildBuilding(-2, baseY, 2, true);
        this.buildBuilding(2, baseY, -2, true);
        this.buildBuilding(-2, baseY, -2, true);

        this.buildBuilding(10, baseY, 2);
        this.buildBuilding(10, baseY, -2);
        this.buildBuilding(10, baseY, 6);
        this.buildBuilding(10, baseY, -6);

        this.buildBuilding(-10, baseY, 2);
        this.buildBuilding(-10, baseY, -2);
        this.buildBuilding(-10, baseY, 6);
        this.buildBuilding(-10, baseY, -6);

        this.buildBuilding(2, baseY, 10);
        this.buildBuilding(-2, baseY, 10);
        this.buildBuilding(6, baseY, 10);
        this.buildBuilding(-6, baseY, 10);

        this.buildBuilding(2, baseY, -10);
        this.buildBuilding(-2, baseY, -10);
        this.buildBuilding(6, baseY, -10);
        this.buildBuilding(-6, baseY, -10);

    }

    buildAll_max() {
        const baseY = 0; // how high off the ground the building starts

        // centre buildings all have TNT barrels
        this.buildBuilding(2, baseY, 2, true);
        this.buildBuilding(-2, baseY, 2, true);
        this.buildBuilding(2, baseY, -2, true);
        this.buildBuilding(-2, baseY, -2, true);

        this.buildBuilding(10, baseY, 2);
        this.buildBuilding(10, baseY, -2);
        this.buildBuilding(10, baseY, 6);
        this.buildBuilding(10, baseY, -6);
        this.buildBuilding(10, baseY, 10);
        this.buildBuilding(10, baseY, -10);
        this.buildBuilding(10, baseY, 14);
        this.buildBuilding(10, baseY, -14);

        this.buildBuilding(14, baseY, 2);
        this.buildBuilding(14, baseY, -2);
        this.buildBuilding(14, baseY, 6);
        this.buildBuilding(14, baseY, -6);
        this.buildBuilding(14, baseY, 10);
        this.buildBuilding(14, baseY, -10);
        this.buildBuilding(14, baseY, 14);
        this.buildBuilding(14, baseY, -14);

        this.buildBuilding(-10, baseY, 2);
        this.buildBuilding(-10, baseY, -2);
        this.buildBuilding(-10, baseY, 6);
        this.buildBuilding(-10, baseY, -6);
        this.buildBuilding(-10, baseY, 10);
        this.buildBuilding(-10, baseY, -10);
        this.buildBuilding(-10, baseY, 14);
        this.buildBuilding(-10, baseY, -14);

        this.buildBuilding(-14, baseY, 2);
        this.buildBuilding(-14, baseY, -2);
        this.buildBuilding(-14, baseY, 6);
        this.buildBuilding(-14, baseY, -6);
        this.buildBuilding(-14, baseY, 10);
        this.buildBuilding(-14, baseY, -10);
        this.buildBuilding(-14, baseY, 14);
        this.buildBuilding(-14, baseY, -14);

        this.buildBuilding(2, baseY, 10);
        this.buildBuilding(-2, baseY, 10);
        this.buildBuilding(6, baseY, 10);
        this.buildBuilding(-6, baseY, 10);
        this.buildBuilding(2, baseY, 14);
        this.buildBuilding(-2, baseY, 14);
        this.buildBuilding(6, baseY, 14);
        this.buildBuilding(-6, baseY, 14);

        this.buildBuilding(2, baseY, -10);
        this.buildBuilding(-2, baseY, -10);
        this.buildBuilding(6, baseY, -10);
        this.buildBuilding(-6, baseY, -10);
        this.buildBuilding(2, baseY, -14);
        this.buildBuilding(-2, baseY, -14);
        this.buildBuilding(6, baseY, -14);
        this.buildBuilding(-6, baseY, -14);

    }

    buildAll_mini() {
        const baseY = 0; // how high off the ground the building starts

        this.buildBuilding(-2, baseY, 2);
        this.buildBuilding(2, baseY, -2);
        this.buildBuilding(-2, baseY, -2, true); // TNT

        this.buildBuilding(-10, baseY, 10, true); // TNT
        this.buildBuilding(-10, baseY, 6);
        this.buildBuilding(-10, baseY, 2);
        this.buildBuilding(-10, baseY, -2);
        this.buildBuilding(-10, baseY, -6);

        this.buildBuilding(-6, baseY, -10);
        this.buildBuilding(-2, baseY, -10);
        this.buildBuilding(2, baseY, -10);
        this.buildBuilding(6, baseY, -10);
        this.buildBuilding(10, baseY, -10, true); // TNT
    }

    buildAll_minimal() {
        const baseY = 0; // how high off the ground the building starts

        // this.buildBuilding(-2, baseY, 2);
        // this.buildBuilding(2, baseY, -2);
        this.buildBuilding(-2, baseY, -2, true); // TNT

        this.buildBuilding(-10, baseY, 2, true);
        this.buildBuilding(-10, baseY, -2);
        this.buildBuilding(-10, baseY, -6);

        this.buildBuilding(-6, baseY, -10);
        this.buildBuilding(-2, baseY, -10);
        this.buildBuilding(2, baseY, -10, true);
    }

    build121(x, y, z) {
        BlockActor.create({ parent: this, shape: "121", translation: [x, y + 1, z] });
        BlockActor.create({ parent: this, shape: "121", translation: [x, y + 3, z] });
    }

    buildFloor(x, y, z) {
        this.build121(x - 1.5, y, z - 1.5);
        this.build121(x - 1.5, y, z + 1.5);
        this.build121(x + 1.5, y, z - 1.5);
        this.build121(x + 1.5, y, z + 1.5);
        BlockActor.create({ parent: this, shape: "414", translation: [x + 0, y + 4.5, z + 0] });
    }

    buildBuilding(x, y, z, addBarrel = false) {
        this.buildFloor(x, y, z);
        this.buildFloor(x, y + 5, z);

        // default shape is "111"
        BlockActor.create({ parent: this, translation: [x - 1.5, y + 10.5, z - 1.5] });
        BlockActor.create({ parent: this, translation: [x - 1.5, y + 10.5, z + 1.5] });
        BlockActor.create({ parent: this, translation: [x + 1.5, y + 10.5, z - 1.5] });
        BlockActor.create({ parent: this, translation: [x + 1.5, y + 10.5, z + 1.5] });

        BlockActor.create({ parent: this, translation: [x + 0, y + 10.5, z - 1.5] });
        BlockActor.create({ parent: this, translation: [x - 0, y + 10.5, z + 1.5] });
        BlockActor.create({ parent: this, translation: [x + 1.5, y + 10.5, z - 0] });
        BlockActor.create({ parent: this, translation: [x - 1.5, y + 10.5, z + 0] });

        if (addBarrel) BarrelActor.create({ parent: this, translation: [x, y + 5.5, z] });
    }

}
BaseActor.register('BaseActor');

//------------------------------------------------------------------------------------------
//-- MyUser --------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class MyUser extends User {

    init(options) {
        super.init(options);
        this.index = Math.floor(Math.random() * 21);
    }
}
MyUser.register("MyUser");

//------------------------------------------------------------------------------------------
//-- MyUserManager -------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class MyUserManager extends UserManager {
    get defaultUser() { return MyUser }
}
MyUserManager.register("MyUserManager");


//------------------------------------------------------------------------------------------
//-- MyModelRoot ---------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

export class MyModelRoot extends GameModelRoot {

    static modelServices() {
        return [RapierManager, MyUserManager, ...super.modelServices()];
    }

    init(...args) {
        super.init(...args);
        console.log("Start root model!!!");
        this.seedColors();

        this.base = BaseActor.create({ gravity: [0, -12, 0], translation: [0, 0, 0] });
    }

    seedColors() {
        this.colors = [
            rgb(242, 215, 213),        // Red
            rgb(217, 136, 128),        // Red
            rgb(192, 57, 43),        // Red

            rgb(240, 178, 122),        // Orange
            rgb(230, 126, 34),        // Orange
            rgb(175, 96, 26),        // Orange

            rgb(247, 220, 111),        // Yellow
            rgb(241, 196, 15),        // Yellow
            rgb(183, 149, 11),        // Yellow

            rgb(125, 206, 160),        // Green
            rgb(39, 174, 96),        // Green
            rgb(30, 132, 73),        // Green

            rgb(133, 193, 233),         // Blue
            rgb(52, 152, 219),        // Blue
            rgb(40, 116, 166),        // Blue

            rgb(195, 155, 211),        // Purple
            rgb(155, 89, 182),         // Purple
            rgb(118, 68, 138),        // Purple

            [0.9, 0.9, 0.9],        // White
            [0.5, 0.5, 0.5],        // Gray
            [0.2, 0.2, 0.2]        // Black
        ];

    }

}
MyModelRoot.register("MyModelRoot");
