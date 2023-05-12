// Croquet Demolition demo on THREE

import { App, StartWorldcore } from "@croquet/worldcore";

import { MyViewRoot } from "./src/Views-three";
import { MyModelRoot } from "./src/Models";

App.makeWidgetDock();
App.sync = false;
const loadProgressElem = document.getElementById('loadProgress');
StartWorldcore({
    appId: 'io.croquet.worldcore.demolition',
    apiKey: '14lzk3cMcqBBy19rxhxMbMBBedPNGLhnF6oLrJaF4',
    name: App.autoSession(),
    password: 'password',
    debug: ['session', 'messages'],
    model: MyModelRoot,
    view: MyViewRoot,
    tps: 1000 / 27, // aiming to catch a 50ms Rapier update every other tick
    progressReporter: ratio => {
        loadProgressElem.textContent = `${Math.round(ratio * 100)}%`;
    }
}).then(() => loadProgressElem.remove());
