# Hathora Demo - It's safe to delete this dir

## About

This project comes with 2 demos within 1 scene:

1. Hello World - a simple RPC ping/pong test.

2. Hathora SDK Demo - a simple demo of creating/joining a room and connecting to a remote server.


## Dependencies

1. Made with Unity 2021.3 LTS, but likely works with 2019+.

2. The example used FishNet* for Unity Netcode: https://assetstore.unity.com/packages/tools/network/fish-net-networking-evolved-207815

3. Hathora SDK via Unity Package Manager (UPM): https://openupm.com/packages/com.hathora.client/ 


## Demo Test Flow

### (A) Local testing within the editor on a single PC

1. Open ./Scenes/HathoraDemoScene -> find NetworkManager GameObject. 

2. Find NetHathoraClient.NetHathoraConfig component.

3. Click the right () circle to choose a Config to use (or make a new one via top menu `Hathora/Configuration`).

4. Play -> Hello World -> Start Server -> Start Client.

You are now both the server and client - test RPC ping/pong with [R] and see logs.


### (B) Local testing with 1 standalone and a single PC.

1. Follow "A" steps 1~3.

2. Ensure the `HathoraDemoScene` is added to build settings and build. Test locally with any Windows/Linux build target.

3. Run the exe from commandline via `someExe -batchmode -nographics -mode server". Note you can also `-mode client`.

4. Back to the editor: Play -> Hello World -> Start Client (if standalone is running server).


### (C) Hathora Cloud deployment testing as server with editor as client.

1. Follow "A" steps 1~3.

2. Find your Config again via top menu `Hathora/Configuration`.

3. Click the Login button -> Select an app (or click the "Create New App" button - coming soon).

4. [Optional] Change defaults in the build/deploy settings.

5. Click the bottom "Build, Upload & Deploy New Version" button -> Wait until done.

6. Open ./Scenes/HathoraDemoScene -> Find.

7. Follow "B" step 4.

## Attributions

* 'FishNet' is a trademark of FirstGearGames and unaffiliated with Hathora, Inc.; Hathora is a hosting/deployment solution using FishNet as the arbitrary NetCode demo for game server deployment. See FishNet's licensing for more information.
