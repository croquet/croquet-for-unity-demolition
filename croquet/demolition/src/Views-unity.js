// Croquet Demolition Demo

import { Pawn, mix, toRad, m4_rotation, m4_multiply, m4_translation, m4_getTranslation, m4_getRotation, GetViewService } from "@croquet/worldcore";
import { GameInputManager, GameViewRoot, PM_GameSmoothed, PM_GameRendered } from "../common/unity-bridge";

//------------------------------------------------------------------------------------------
//-- BasePawn -------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class BasePawn extends Pawn {
    constructor(...args) {
        super(...args);

        this.pawnManager = GetViewService('GameEnginePawnManager');
        this.userManager = this.modelService('UserManager');

        this.subscribe('input', 'pointerUp', this.doPointerUp);
        this.subscribe('input', 'shoot', this.doShoot);
    }

    doPointerUp({ button }) {
        if (button === 1) this.publish('ui', 'new'); // reset the scene
    }

    doShoot({ gun }) {
        // globalThis.timedLog("shoot");
        const index = this.userManager.user(this.viewId).index;
        this.publish('ui', 'shoot', { gun, index });
    }

}
BasePawn.register('BasePawn');

class SmoothedGamePawn extends mix(Pawn).with(PM_GameRendered, PM_GameSmoothed) {

    constructor(...args) {
        super(...args);

        this.setGameObject({ type: this.actor.type, color: this.actor.color, alpha: this.actor.alpha, confirmCreation: true, waitToActivate: true });
    }

}
SmoothedGamePawn.register('SmoothedGamePawn');

//------------------------------------------------------------------------------------------
//-- BarrelPawn ------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class BarrelPawn extends SmoothedGamePawn {

    constructor(...args) {
        super(...args);
        this.listen('fuseLit', this.fuseLit);
        this.listen('exploded', this.exploded);
    }

    fuseLit() {
        this.sendToUnity('fuseLit');
    }

    exploded() {
        this.sendToUnity('exploded');
    }

}
BarrelPawn.register('BarrelPawn');

// stub pawn classes that don't need specialisation
SmoothedGamePawn.register('BlockPawn');
SmoothedGamePawn.register('BulletPawn');

export class MyViewRoot extends GameViewRoot {

    static viewServices() {
        return [GameInputManager].concat(super.viewServices());
    }

    onStart() {
        this.pawnManager = GetViewService('GameEnginePawnManager');
        this.inputManager = GetViewService('GameInputManager');

        this.addEventHandlers();
        this.placeCamera();
    }

    addEventHandlers() {
        const im = this.inputManager;
        im.addEventHandlers({
            shoot: args => {
                // args[1] is a comma-separated position for the gun
                const gun = args[1].split(',').map(Number);
                im.publish('input', 'shoot', { gun });
            }
        });
    }

    placeCamera() {
        const pitchMatrix = m4_rotation([1, 0, 0], toRad(20));
        const yawMatrix = m4_rotation([0, 1, 0], toRad(-30));

        let cameraMatrix = m4_translation([0, 0, -50]);
        cameraMatrix = m4_multiply(cameraMatrix, pitchMatrix);
        cameraMatrix = m4_multiply(cameraMatrix, yawMatrix);

        const translation = m4_getTranslation(cameraMatrix);
        const rotation = m4_getRotation(cameraMatrix);
        this.pawnManager.updateGeometry('camera', { translationSnap: translation, rotationSnap: rotation });
    }

}
