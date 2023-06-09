// Croquet Demolition Demo

import { Pawn, mix, GetViewService } from "@croquet/worldcore";
import { GameInputManager, GameViewRoot, PM_GameSmoothed, PM_GameRendered, PM_GameSpatial, PM_GameMaterial } from "../build-tools/sources/unity-bridge";


Pawn.register('BasePawn');

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

// stub pawn classes that don't need specialisation
SmoothedGamePawn.register('BlockPawn');
SmoothedGamePawn.register('BulletPawn');
SmoothedGamePawn.register('BarrelPawn');

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
                // args[1] is a comma-separated position for the gun
                const gun = args[1].split(',').map(Number);
                im.publish('input', 'shoot', { gun });
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
