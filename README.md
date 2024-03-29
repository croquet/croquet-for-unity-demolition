# Croquet for Unity Demolition

<img width="1998" alt="demolition-screenshot" src="https://github.com/croquet/croquet-for-unity-demolition/assets/123010049/5659adc3-0b85-452b-b69b-8ab217b23bf6">


This repository contains a Croquet for Unity (C4U) view of Brian Upton's "Demolition" app built on Worldcore.

The most important directories are the following:
* `unity/` - the Unity project directory, from which you can run the game in the Unity editor or create builds for any platform Unity supports except WebGL.
* `unity/Assets/Scenes/` - the `demolition` scenes (1 to 4), along with the `MainMenu` scene (explained below) that allows a group of players to join the same session to play together.
* `unity/Assets/CroquetJS/demolition/` - JavaScript source for building the Croquet side of the game. You can edit the code under this directory to change the game's behavior.

# Questions
Please feel free to ask questions on our [discord](https://croquet.io/discord).

# Setup
To setup the project follow these steps...

## 1.0 Node Installation
Node is a prerequisite for installing JavaScript libraries like Croquet and Worldcore, as well as facilitating webpack builds.

Install node.js and the node package manager (npm) for your platform here (LTS Recommended): https://nodejs.org/en/download


## 2.0 Clone the Repo
Install git from https://git-scm.com/downloads

```
git clone https://github.com/croquet/croquet-for-unity-demolition.git
```

Note: this repository's large size is predominantly due to our including a specific version of NodeJS for Windows.  On Windows we normally use NodeJS to run the JavaScript side of a C4U session, since Unity on Windows is currently unable to use the WebView mechanism that Croquet prefers.  On MacOS we use the WebView by default, but if a project has the necessary entry point for NodeJS execution (as the Demolition game does), NodeJS can be used on Mac as well.


## 3.0 Load the Unity Project

Make sure you have the Unity Hub installed from
https://unity.com/download

 > **NOTE:** For now, we **strongly recommend** using _exactly_ Unity Editor Version `2021.3.19f1` for C4U projects

2021.3.19f1 can be downloaded by pasting the following in your browser: `unityhub://2021.3.19f1/c9714fde33b6`  This deeplink to the Unity Hub should open an installation dialog for the correct version.

In the `Unity Hub` app, select `Open => Add project from disk`, then navigate to the `croquet-for-unity-demolition/unity` folder and hit `Add Project`.


## 4.0 Set up your Croquet Developer Credentials

In the Project Navigator (typically at bottom left), go to `Assets/Settings` and click `CroquetSettings.asset`.  The main field that you need to set up is the **Api Key**.

The API Key is a token of around 40 characters that you can create for yourself at https://croquet.io/account.  It provides access to the Croquet infrastructure.

The App Prefix is the way of identifying with your organization the Croquet apps that you develop and run.  The combination of this prefix and the App Name provided on the `Croquet Bridge` component in each scene is a full App ID - for example, `io.croquet.worldcore.demolition`.  For running the game it is fine to leave this prefix as is, but when you develop your own apps you must change the prefix so that the App ID is a globally unique identifier.  The ID must follow the Android reverse domain naming convention - i.e., each dot-separated segment must start with a letter, and only letters, digits, and underscores are allowed.

**For MacOS only:** Find the Path to your Node executable, by going to a terminal and running
```
which node
```
On the `CroquetSettings` asset, fill in the **Path to Node** field with the path.


## 5.0 Run the Game
In the Project Navigator, go to `Assets/Scenes` and double-click `MainMenu.unity`.  If a "TMP importer" dialog comes up at this point, hit the top button ("Import TMP Essentials") then close the dialog. This is just part of the standard setup for Text Mesh Pro (which is used for all the UI).

In the editor's top menu, go to the `Croquet` drop-down and ensure that the `Build JS on Play` item has a check-mark next to it.  If not, invoke the item to toggle the mark.

**IMPORTANT: During the next step the editor may appear to have frozen, possibly for some minutes. This is not a sign of failure; please be patient.**

Press the Play button.  The first time you do so after installation, C4U will notice that you have not yet installed the JavaScript build tools from the package.  It will copy them across, and also run an `npm install` that fetches all Croquet and other dependencies that are needed.  *As noted above, depending on network conditions this could take a few minutes - during which, because of Unity's scheduling mechanisms, nothing will appear in the console.*

In addition to the tools installation, because `Build JS on Play` is set C4U will run a full webpack build of the JavaScript code - eventually adding webpack's output to the console, each line prefixed with "JS builder".  The first build of the game will take the longest; on subsequent runs the build process will be faster.

Once all initialization completes, you will see the Demolition splash screen with some buttons. On the very first run, the buttons' labels might not show; if that is the case, resizing the editor a little should bring them to life.

Press the "New Game" button.  The Unity console should show a series of log messages for startup of the game - ending with "Croquet view for scene demolition1 running", at which point the game's first block structure will appear.  Click to fire; drag to move.

### 5.1 Specifying a Croquet Session Name
_This is an optional configurability feature, not required for you to start playing with Demolition._

Croquet sessions are inherently multi-user, and this applies fully to the sessions that drive a C4U application. If you start the same application on multiple devices, you can expect that all those devices will be in the application together - for example, all cooperating in the Demolition app.

That said, the definition of what counts as "the same application" hinges on the application instances agreeing on _all_ the following factors:

1. **Application ID**. As mentioned in Section 4.0 above, this is a dot-separated name that in C4U is a concatenation of the **App Prefix** in the `CroquetSettings` asset and the **App Name** specified on the scene's `Croquet Bridge`. For example, `io.croquet.worldcore.demolition`.
2. **API Key**. Also mentioned in Section 4.0, this is a developer-specific key for using the Croquet infrastructure. In C4U this is specified in `CroquetSettings`. Note: strictly, the API keys do not have to be identical; as long as they were issued _for the same developer_ they will count as being in agreement.
3. **Session Name**. All Croquet sessions are launched with a Session Name, which in general can be any alphanumeric token - such as `helloworld`, or `123`. Given that the Application ID and API Key for a given app are unlikely to change frequently, Session Name is the most flexible way to control whether application instances will join the same session or not.

Our initial C4U applications come with two alternative ways to specify the Session Name:

* **Through a menu scene**. Loading the `MainMenu` scene into the editor and pressing Play will bring up a simple alphanumeric Join Code menu that works like popular party games such as Jackbox.  Hitting the "New Game" button generates a random five-letter game code that is then used as the Session Name.  If instead you type a code into the name field and hit "Join Game", the code you entered becomes the Session Name.

<ul>
The `MainMenu` scene was configured to work this way by setting the **Launch Via Menu Into Scene** property on the scene's `Croquet Bridge` object.  This means that C4U will wait for a Session Name to be supplied by a menu-driven script, and will then launch the specified next scene (here, `demolition1`) using that name.
</ul>

* **"Default Session Name" property**. If the **Launch Via Menu Into Scene** property is left blank, pressing Play will cause C4U to take as its Session Name the value found in the `Croquet Bridge`'s **Default Session Name** property (or, if that is also blank, the Session Name will simply be "unnamed").


# App UI Details
The Demolition app is an instantly-joinable multiplayer app where you and your friends are shooting at a structure of heavy blocks and some embedded barrels of TNT that explode when disturbed.

## Controls

|Input|Function|
|--------------|-------------------------------------------------------------|
| left click   | Shoot                                                       |
| left drag    | Move shooting position                                      |
| right-pointing button  | Move to next scene (wrapping from scene 4 back to scene 1) |
| left-pointing button  | Move to previous scene (wrapping from scene 1 to scene 4) |
| RESET button  | Reset the blocks in the current scene to their initial state |
| MAIN MENU button  | Leave the game and return to the menu |



# Debugging Techniques
## Using a Web Browser to Debug the JavaScript Code

On both MacOS and Windows, you can choose to use an external browser such as Chrome to run the JavaScript code.  For debugging, this is more convenient than letting the C4U bridge start up an invisible WebView or Node JS process.

Before hitting Play on a scene, select the `Croquet` object in the scene's hierarchy, then in that object's `Croquet Runner` component select the **Debug Using External Session** checkbox.

With this option set, whenever you start playing from that scene the Unity console will include a line of the form "ready for browser to load from http://localhost:...".  Click on the line so that it appears in the text area below the console, and there click on the URL part to load it into your system's default browser.  The loading of the URL will enable the app to continue.  All the JS developer tools (console, breakpoints etc) offered by the browser are available for working with the code.

When you stop play in the Unity editor, the browser tab will automatically leave the Croquet session.  If you restart play, you will need to reload the tab to join the session again.

## Viewing JS Log Output in Unity
The `Croquet Bridge` component's **JS Log Forwarding** property has checkboxes that let you select which categories of console output in the JavaScript session will be transferred across the bridge and appear in the Unity console.  By default, the "warn" and "error" categories are sent.

# Making Sharable Builds
Before building the app to deploy for a chosen platform (e.g., Windows or MacOS standalone, or iOS or Android), there are some settings that you need to pay attention to:

* there must be an **Api Key** present in `CroquetSettings.asset`
* on `Croquet Runner` the **Debug Using External Session** checkbox _must_ be cleared
* on `Croquet Runner` the **Force To Use Node JS** checkbox _must_ be cleared for anything other than a Windows build
* on `Croquet Runner` the **Run Offline** checkbox _must_ be cleared
* on `Croquet Bridge`, ensuring that all checkboxes are cleared under **Croquet Debug Logging** and **JS Log Forwarding** will reduce possibly resource-hungry logging

Hit **Build**!  If any of the obligatory conditions listed above are not met, the build will be halted.  Fix the conditions and try again.

## Supplementary information for sharing MacOS builds

We have found that distributing a standalone MacOS build (`.app` file) requires some care to ensure that recipients can open it without being blocked by MacOS's security checks. One approach that we have found to work - there are doubtless others - is as follows:

1. Make the build - arriving at, say, a file `build.app`
2. In a terminal, execute the following command to replace the app's code signature
    `codesign --deep -s - -f /path/to/build.app`
3. Also use a terminal command (rather than the Finder) to zip the file, to ensure that the full directory structure is captured
    `tar -czf build.tgz /path/to/build.app`
4. Distribute the resulting `.tgz` file, **along with the following instructions to recipients**

    a. download this `.tgz` file

    b. double-click the `.tgz` to unpack the `.app` file

    c. **IMPORTANT: right-click (_not_ double-click)** the `.app` file and choose "Open"

    d. in the security dialog that appears, again choose "Open"

    e. if prompted to give permission for the app to access the network, agree.


# Contribution
Contributions to the project are welcome as these projects are open source and we encourage community involvement.

1. Base your `feature/my-feature-name` branch off of `develop` branch
2. Make your changes
3. Open a PR against the `develop` branch
4. Discuss and Review the PR with the team
