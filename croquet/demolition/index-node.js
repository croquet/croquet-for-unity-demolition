// startup script for an app using Croquet on NodeJS

const { MyModelRoot } = require("./src/Models");
const { StartSession } = require("./common/unity-bridge");
const { MyViewRoot } = require("./src/Views-unity");

StartSession(MyModelRoot, MyViewRoot);
