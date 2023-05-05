# Croquet for Unity Demolition

This repository contains the Croquet for Unity (C4U) port of the Croquet Demolition demo.

The most important directories are the following:
* `unity/` - a loadable Unity project, from which you can run apps in the editor or build standalone apps for deployment on iOS and Android.
* `unity/Assets/Scenes/mainDemolition.unity` - the scene that contains the main game
* `unity/Assets/Scenes/SessionNameChooser.unity` - a pre-game scene that allows the selection of different sessions by simple integer ID.
* `croquet/src` - JavaScript source for building the Croquet side of the Demolition Demo.  You can edit the code under this directory to change the demo behavior.

# Setup

## Repository Installation

To install the repository and prepare for building the demo, carry out the following steps:

Install node.js and the node package manager (npm) for your platform here (LTS Recommended):
https://nodejs.org/en/download


Clone the Repo

```
git clone https://github.com/croquet/croquet-for-unity-demolition.git
```

Navigate and install the project dependencies

```
cd croquet-for-unity-demolition
```

```
npm i
```

Navigate and install the demolition dependencies

```
cd croquet/demolition
```

```
npm i
```

Note: that this repository's large size is predominantly due to our including a specific version of NodeJS for Windows.  On Windows we have to use NodeJS to run the JavaScript side of a C4U session, since Unity on Windows is currently unable to use the WebView mechanism that Croquet prefers.  On MacOS we use the WebView by default, but if a project has the necessary entry point for NodeJS execution, NodeJS can be used on Mac as well.

## Load the Unity Project
Using _exactly_ Unity Editor Version `2021.3.19f1` is **strongly recommended** (for now) - otherwise there's a serious chance of confusion. 2021.3.19f1 can be downloaded easily by pasting the following in your browser: `unityhub://2021.3.19f1/c9714fde33b6` (a deeplink to open hub to the correct version).

In the `Unity Hub` app, select `Open => Add project from disk`, then navigate to the `croquet-for-unity-demolition/unity` folder and hit `Add Project`.  Make sure that it is opened with _exactly_ editor `version 2021.3.19f1` .

If, during this first loading, Unity complains that there appear to be script errors, it's fine to hit `Ignore` and continue.

## Set up your Croquet Developer Credentials

In the Project Navigator (typically at bottom left), go to `Packages/Croquet Multiplayer/Multiplayer/Croquet/Runtime/Settings` and click `CroquetSettings.asset`.  The main field that you need to set up is the `Api Key`.

This can also be found referenced on the CroquetBridge GameObject in the mainDemolition scene.

The API Key is a token of around 40 characters that you can create for yourself at https://croquet.io/account.  It provides access to the Croquet infrastructure.

The App Prefix is the way of identifying with your organization the Croquet apps that you develop and run.  The combination of this prefix and the App Name provided on the Croquet Bridge component in each scene is a full App ID - for example, `io.croquet.demolition`.  For running demolition it is fine to leave this prefix as is, but when you develop your own apps you must change the prefix so that the App ID is a globally unique identifier.  The ID must follow the Android reverse domain naming convention - i.e., each dot-separated segment must start with a letter, and only letters, digits, and underscores are allowed.

For Macs: Find the Path to your Node Executable, which can be found by running
```
which node
```
On the Settings Asset, fill in the **Path to Node** field with the path.

For Windows: Your system may complain about "Script Execution Policy" which will prevent our setup scripts from running. The following command allows script execution on Windows for the current user:
```
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```
then Yes to [A]ll.

## Run the Demolition Demo


In the Project Navigator, go to `Assets/Scenes` and double-click the `mainDemolition.unity` scene.

In the editor's top menu, go to the `Croquet` drop-down and select `Build JS on Play` so that it has a check-mark next to it.

Press the play button.  Because this is the first time you have built the app, it will run a full webpack build of the JavaScript code - eventually writing webpack's log to the Unity console, each line prefixed with "JS builder".  You should then see console output for startup of the app - ending with "Croquet session ready", at which point the app should start to run.

## Debug with Chrome

On MacOS (not on Windows, where the Croquet side is always run using NodeJS) you can use an external browser such as Chrome to run the JavaScript code.  For debugging, this is more convenient than letting the C4U bridge start up an invisible WebView.

In the scene (while play is *not* in progress), select "CroquetBridge" in the hierarchy (typically at top left), then in that object's "Croquet Runner" component (seen on the right) select the "Wait For User Launch" checkbox.

Now whenever you press play on that scene, the console output will stop at a line of the form "ready for browser to load from http://localhost:...".  Copy that address (if you click on the line, it will appear as selectable text in the view below the console) then use it to launch a new browser tab.  This should complete the startup of the app.

When you stop play in the Unity editor, the browser tab will automatically leave the Croquet session.  If you restart play, you will need to reload the tab to join the session again.

# Questions
Please ask questions on our [discord](https://croquet.io/discord).

