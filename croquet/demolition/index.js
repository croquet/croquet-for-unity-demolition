// Croquet Demolition demo

import { StartSession } from "./common/unity-bridge";

import { MyViewRoot } from "./src/Views-unity";
import { MyModelRoot } from "./src/Models";

StartSession(MyModelRoot, MyViewRoot);
