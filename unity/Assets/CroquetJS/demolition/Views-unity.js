// Croquet Demolition Demo

import { Pawn, mix, GetViewService } from "@croquet/worldcore";
import { GameInputManager, GameViewRoot, PM_GameSmoothed, PM_GameRendered, PM_GameSpatial, PM_GameMaterial } from "../build-tools/sources/unity-bridge";

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

    doShoot({ gun, dir }) {
        // globalThis.timedLog("shoot");
        const index = this.userManager.user(this.viewId).index;
        this.publish('ui', 'shoot', { gun, dir, index });
    }

}
BasePawn.register('BasePawn');

class GamePawn extends mix(Pawn).with(PM_GameRendered, PM_GameSpatial) {

    constructor(...args) {
        super(...args);

        this.setGameObject({ type: this.actor.type });
    }

}
GamePawn.register('GamePawn');

class SmoothedGamePawn extends mix(Pawn).with(PM_GameRendered, PM_GameSmoothed, PM_GameMaterial) {

    constructor(...args) {
        super(...args);

        this.setGameObject({ type: this.actor.type, confirmCreation: true, waitToPresent: true });
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
        //this.placeCamera();
    }

    addEventHandlers() {
        const im = this.inputManager;
        im.addEventHandlers({
            shoot: args => {
                // args[1] is a comma-separated position for the gun and then its direction
                // ['shoot', '-14.11279,-4.907934,-26.27322|-0.7352883,-0.5900273,0.3334945']
                console.log(args);
                var gun =  args[1].split('|')[0].split(',').map(Number);
                console.log("gun: "+gun);

                if (args[1].split('|').length>1){
                    var dir =  args[1].split('|')[1].split(',').map(Number);
                    im.publish('input', 'shoot', { gun, dir });
                }
                else{
                    im.publish('input', 'shoot', { gun });
                }
            
            }
        });
    }

    // placeCamera() {
    //     const pitchMatrix = m4_rotation([1, 0, 0], toRad(20));
    //     const yawMatrix = m4_rotation([0, 1, 0], toRad(-30));

    //     let cameraMatrix = m4_translation([0, 0, -50]);
    //     cameraMatrix = m4_multiply(cameraMatrix, pitchMatrix);
    //     cameraMatrix = m4_multiply(cameraMatrix, yawMatrix);

    //     const translation = m4_getTranslation(cameraMatrix);
    //     const rotation = m4_getRotation(cameraMatrix);
    //     this.pawnManager.updateGeometry('camera', { translationSnap: translation, rotationSnap: rotation });
    // }

}
