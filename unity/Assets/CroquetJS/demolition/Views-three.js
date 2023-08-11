// Demolition Demo

import { App, ViewRoot, Pawn, mix, InputManager, PM_Spatial, PM_Smoothed, toRad, m4_rotation, m4_multiply, TAU, m4_translation, v3_transform, ViewService, GetModelService, StartWorldcore } from "@croquet/worldcore-kernel"; // eslint-disable-line import/no-unresolved
import { PM_ThreeVisible, ThreeRenderManager, THREE, ThreeInstanceManager, PM_ThreeInstanced } from "@croquet/worldcore-three"; // eslint-disable-line import/no-unresolved
import { Widget2, ButtonWidget2, HUD } from "@croquet/worldcore-widget2"; // eslint-disable-line import/no-unresolved

function setGeometryColor(geometry, color) {
    const count = geometry.getAttribute("position").count;
    const colors = [];
    for (let i = 0; i < count; i++) {
        colors.push(...color);
    }
    geometry.setAttribute( 'color', new THREE.Float32BufferAttribute( colors, 3) );
}

const TUG = 0.6; // more eager than default 0.2

//------------------------------------------------------------------------------------------
//-- BlockPawn -------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class BlockPawn extends mix(Pawn).with(PM_Smoothed, PM_ThreeInstanced) {
    constructor(...args) {
        super(...args);
        this.useInstance(this.actor.shape);
        this.tug = TUG;
    }
}
BlockPawn.register('BlockPawn');

//------------------------------------------------------------------------------------------
//-- BulletPawn -------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class BulletPawn extends mix(Pawn).with(PM_Smoothed, PM_ThreeInstanced) {
    constructor(...args) {
        super(...args);
        this.useInstance("ball"+this.actor.index);
        this.tug = TUG;
    }
}
BulletPawn.register('BulletPawn');

//------------------------------------------------------------------------------------------
//-- BarrelPawn ------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class BarrelPawn extends mix(Pawn).with(PM_Smoothed, PM_ThreeInstanced) {
    constructor(...args) {
        super(...args);
        this.useInstance("barrel");
        this.tug = TUG;

        this.listen('exploded', this.exploded);
    }

    exploded() {
        this.destroy();
    }
}
BarrelPawn.register('BarrelPawn');

//------------------------------------------------------------------------------------------
//-- BasePawn -------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

class BasePawn extends mix(Pawn).with(PM_Spatial, PM_ThreeVisible) {
    constructor(...args) {
        super(...args);

        this.baseMaterial = new THREE.MeshStandardMaterial( {color: new THREE.Color(0.4, 0.8, 0.2)} );
        this.baseMaterial.side = THREE.DoubleSide;
        this.baseMaterial.shadowSide = THREE.DoubleSide;

        const group = new THREE.Group();

        this.baseGeometry = new THREE.BoxGeometry( 40, 1, 40 );
        this.baseGeometry.translate(0,-0.5,0);

        const base = new THREE.Mesh( this.baseGeometry, this.baseMaterial );
        base.receiveShadow = true;
        group.add(base);

        this.setRenderObject(group);
    }
}
BasePawn.register('BasePawn');

//------------------------------------------------------------------------------------------
//-- GodView -------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

let fov = 60;
let pitch = toRad(-20);
let yaw = toRad(150);

class GodView extends ViewService {

    constructor() {
        super("GodView");

        this.updateCamera();

        this.subscribe("input", 'wheel', this.onWheel);
        this.subscribe("input", "pointerDown", this.doPointerDown);
        this.subscribe("input", "pointerUp", this.doPointerUp);
        this.subscribe("input", "pointerDelta", this.doPointerDelta);
    }


    updateCamera() {
        if (this.paused) return;
        const rm = this.service("ThreeRenderManager");

        const pitchMatrix = m4_rotation([1,0,0], pitch);
        const yawMatrix = m4_rotation([0,1,0], yaw);

        let cameraMatrix = m4_translation([0,0,50]);
        cameraMatrix = m4_multiply(cameraMatrix,pitchMatrix);
        cameraMatrix = m4_multiply(cameraMatrix,yawMatrix);

        rm.camera.matrix.fromArray(cameraMatrix);
        rm.camera.matrixAutoUpdate = false;
        rm.camera.matrixWorldNeedsUpdate = true;

        rm.camera.fov = fov;
        rm.camera.updateProjectionMatrix();
    }

    onWheel(data) {
        if (this.paused) return;
        const rm = this.service("ThreeRenderManager");
        fov = Math.max(10, Math.min(120, fov + data.deltaY / 50));
        rm.camera.fov = fov;
        rm.camera.updateProjectionMatrix();
    }

    doPointerDown() {
        if (this.paused) return;
        this.dragging = true;
    }

    doPointerUp() {
        if (this.paused) return;
        this.dragging = false;
    }

    doPointerDelta(e) {
        if (this.paused) return;
        if (!this.dragging) return;
        yaw += 0.005 * e.xy[0];
        yaw %= TAU;
        pitch += -0.005 * e.xy[1];
        pitch = Math.min(pitch, toRad(-15));
        pitch = Math.max(pitch, toRad(-90));
        this.updateCamera();
    }
}

//------------------------------------------------------------------------------------------
//-- MyViewRoot ----------------------------------------------------------------------------
//------------------------------------------------------------------------------------------

const gun = [0,-1,50];

export class MyViewRoot extends ViewRoot {

    static viewServices() {
        return [InputManager, ThreeRenderManager, ThreeInstanceManager, HUD, GodView];
    }

    onStart() {
        this.buildLights();
        this.buildHUD();
        this.buildInstances();

        this.subscribe('input', 'tap', this.requestShot);

        document.getElementById('ThreeCanvas').style.transform = "scale(-1, 1)";
    }


    buildLights() {
        const rm = this.service("ThreeRenderManager");

        rm.renderer.setClearColor(new THREE.Color(0.45, 0.8, 0.8));

        const group = new THREE.Group();

        const ambient = new THREE.AmbientLight( 0xffffff, 0.8 );
        group.add(ambient);

        const sun = new THREE.DirectionalLight( 0xffffff, 0.3 );
        sun.position.set(100, 100, 100);
        sun.castShadow = true;
        sun.shadow.mapSize.width = 4096;
        sun.shadow.mapSize.height = 4096;
        sun.shadow.camera.near = 0.5;
        sun.shadow.camera.far = 300;

        sun.shadow.camera.left = -80;
        sun.shadow.camera.right = 80;
        sun.shadow.camera.top = 80;
        sun.shadow.camera.bottom = -80;

        sun.shadow.bias = -0.0001;
        group.add(sun);

        rm.scene.add(group);
    }


    buildHUD() {
        const hudService = this.service("HUD");
        const hud = new Widget2({parent: hudService.root, autoSize: [1,1]});

        // const recenter = new ButtonWidget2({parent: hud, translation: [-10,10], size: [100,30], anchor:[1,0], pivot: [1,0]});
        // recenter.label.set({text:"Recenter", point:14, border: [4,4,4,4]});
        // recenter.onClick = () => {
        //     this.killNextShot = true; // hack
        //     this.doRecenter();
        // };

        const reset = new ButtonWidget2({parent: hud, translation: [-10,10], anchor: [1,0], pivot:[1,0], size: [100,30]});
        reset.label.set({text:"Reset", point:14, border: [4,4,4,4]});
        reset.onClick = () => {
            this.killNextShot = true; // hack
            const { activeScene } = GetModelService("InitializationManager");
            this.publish(this.sessionId, 'requestToLoadScene', { sceneName: activeScene, forceReload: true });
        };

        const next = new ButtonWidget2({ parent: hud, translation: [-10, 45], size: [100, 30], anchor: [1, 0], pivot: [1, 0] });
        next.label.set({ text: "Next level", point: 14, border: [4, 4, 4, 4] });
        next.onClick = () => {
            this.killNextShot = true; // hack

            // @@ bigger hack.  we assume that this app's scene names are foo<n>, where n starts at 1.
            const { activeScene, sceneDefinitions } = GetModelService("InitializationManager");
            if (sceneDefinitions) {
                const sceneNames = Object.keys(sceneDefinitions);
                const prefixLength = [...activeScene].findIndex(c => c >= '0' && c <= '9');
                const prefix = activeScene.slice(0, prefixLength);
                const sceneNumber = Number(activeScene.slice(prefixLength));
                let nextNumber = sceneNumber + 1;
                if (!sceneDefinitions[prefix + nextNumber]) nextNumber = 1; // loop
                this.publish(this.sessionId, 'requestToLoadScene', { sceneName: prefix + nextNumber, forceReload: false });
            }
        };

        // const cta = new ButtonWidget2({parent: hud, translation: [-10,45], size: [100,30], anchor:[1,0], pivot: [1,0]});
        // cta.label.set({text:"Sign up", point:14, border: [4,4,4,4]});
        // cta.onClick = () => {
        //     this.killNextShot = true; // hack
        //     this.signUp();
        // };
    }

    signUp() {
        const url = 'http://croquet.io/unity?utm_source=gdc&utm_medium=demo&utm_campaign=GDC-23';
        window.open(url, '_blank').focus();
    }

    doRecenter() {
        fov = 60;
        pitch = toRad(-20);
        yaw = toRad(150);
        this.service("GodView").updateCamera();
    }

    requestShot() {
        // wait a beat in case this was a click on a UI button
        this.future(0).doShoot();
    }

    doShoot() {
        // hack for dealing with whole-screen detection of the clicks
        // meant for buttons
        if (this.killNextShot) {
            delete this.killNextShot;
            return;
        }

        const um = this.modelService("UserManager");
        const shooter = um.user(this.viewId);
        const index = shooter.index;
        const pitchMatrix = m4_rotation([1,0,0], pitch);
        const yawMatrix = m4_rotation([0,1,0], yaw);
        const both = m4_multiply(pitchMatrix, yawMatrix);
        const shoot = v3_transform(gun, both);
        // model is expecting two strings: the viewId, and a comma-separated location
        this.publish("ui", "shoot", [ this.viewId, shoot.join(',') ]);
    }

    buildInstances() {
        const im = this.service("ThreeInstanceManager");

        const  material = new THREE.MeshStandardMaterial( {color: new THREE.Color(1,1,1)} );
        material.side = THREE.DoubleSide;
        material.shadowSide = THREE.DoubleSide;
        material.castShadow = true;
        material.vertexColors = true;
        im.addMaterial("default", material);

        for (let n = 0; n < this.model.colors.length; n++) {
            const color = this.model.colors[n];
            const geometry = new THREE.SphereGeometry(0.5, 10, 10);
            setGeometryColor(geometry, color);
            im.addGeometry("ball" + n, geometry);
            const mesh = im.addMesh("ball" + n, "ball" + n, "default");
            mesh.castShadow = true;
            mesh.receiveShadow = true;
        }

        const geo111 = new THREE.BoxGeometry( 1, 1, 1 );
        setGeometryColor(geo111, [0.5,0.5,0.5]);
        im.addGeometry("block111", geo111);

        const mesh111 = im.addMesh("111", "block111", "default");
        mesh111.receiveShadow = true;
        mesh111.castShadow = true;
        mesh111.receiveShadow = true;

        const geo121 = new THREE.BoxGeometry( 1, 2, 1 );
        setGeometryColor(geo121, this.model.colors[6]);
        im.addGeometry("block121", geo121);

        const mesh121 = im.addMesh("121", "block121", "default");
        mesh121.receiveShadow = true;
        mesh121.castShadow = true;
        mesh121.receiveShadow = true;

        const geo414 = new THREE.BoxGeometry( 4, 1, 4 );
        setGeometryColor(geo414, this.model.colors[5]);
        im.addGeometry("block414", geo414);

        const mesh414 = im.addMesh("414", "block414", "default");
        mesh414.receiveShadow = true;
        mesh414.castShadow = true;
        mesh414.receiveShadow = true;

        const barrel = new THREE.CylinderGeometry( 0.5, 0.5, 1, 10);
        setGeometryColor(barrel, [0.9,0,0]);
        im.addGeometry("barrel", barrel);

        const bbb = im.addMesh("barrel", "barrel", "default");
        bbb.receiveShadow = true;
        bbb.castShadow = true;
        bbb.receiveShadow = true;
    }
}

Pawn.register('GamePawn');

export async function StartSession(model, view) {
    let packageVersion = '';
    const toolsFile = 'last-installed-tools.txt'; // will have been copied to the dist folder
    const toolsFetch = await fetch(toolsFile);
    if (toolsFetch.status === 200) {
        const toolsText = await toolsFetch.text();
        packageVersion = JSON.parse(toolsText).packageVersion;
    }
    if (!packageVersion) {
        throw Error(`Cannot find package version in ${toolsFile}`);
    }

    let sceneText = '';
    const sceneFile = 'scene-definitions.txt';
    const sceneFetch = await fetch(sceneFile);
    if (sceneFetch.status === 200) sceneText = await sceneFetch.text();
    // @@ workaround until we're able to request that Croquet.Constants not be
    // frozen.  this value is examined in the model (by the InitializationManager),
    // but this is a legitimate case of reading a view-defined value into the
    // model because we're also hashing this value to determine the sessionId.
    globalThis.GameConstants = { sceneText };

    // include package version and the scene-definition string as options
    // just to force sessions with different values to be distinct
    const options = { c4uPackageVersion: packageVersion, sceneText };

    App.makeWidgetDock();
    App.sync = false;
    const loadProgressElem = document.getElementById('loadProgress');
    StartWorldcore({
        appId: 'io.croquet.worldcore.demolition',
        apiKey: '14lzk3cMcqBBy19rxhxMbMBBedPNGLhnF6oLrJaF4',
        name: App.autoSession(),
        password: 'password',
        options,
        // debug: ['session', 'messages'],
        model,
        view,
        tps: 1000 / 27, // aiming to catch a 50ms Rapier update every other tick
        progressReporter: ratio => {
            loadProgressElem.textContent = `${Math.round(ratio * 100)}%`;
        }
    }).then(() => loadProgressElem.remove());
}
