# Hathora-Unity Guide

This guide helps you set up and test a simple 3rd-person 3D Unity project using [FishNet](https://github.com/FirstGearGames/FishNet) as an example. You can replace the net code with others like Mirror or Unity NGO.

# Quickstart

1. Open "Playground" scene >> Set NetworkManager's 'Tugboat' component Server port + Client IP.
   * Use `localhost` IP + arbitrary port for local testing or a Hathora room's server:ip.
2. Build a *Linux Dedicated Server* to `/Build-Server/Build-Server.x86_64` in Unity root.
3. Deploy using `/utils/DeployToHathora.ps1` PowerShell script in repo root >> web console launches.
4. Create a room near your region >> Copy the room's server:port (via the 'details' button) back to **Server** port + **Client** IP.

![image](https://user-images.githubusercontent.com/8840024/233578161-630e86bf-0bcd-4c43-9d97-0470367d1cfc.png)
![image](https://i.imgur.com/cENnBNn.png)
![image](https://i.imgur.com/cM32Vqq.png)

5. Run local editor with "Client" button >> you spawn only if the Server is found! Press `[R]` for ping/pong RPC tests.

![image](https://i.imgur.com/amil9K4.png)

__________________________

# Verbose Guide

## Prerequisites

Ensure you have:

1. Unity 2021 LTS installed.
2. Linux dedicated server build support (available in Unity Hub).

![image](https://user-images.githubusercontent.com/8840024/233582785-3755eb9c-584f-4cd0-b798-6f29eccacf4a.png)

## Project Setup

1. Open the project in Unity 2021 LTS.
2. Open the `Playground` scene.

## Local Testing

1. Open `PlayGround` scene's `NetworkManager` from the hierarchy.
2. Set **Server** port arbitrarily and **Client** IP to `localhost`.

![image](https://i.imgur.com/ZEbjEsO.png)

3. Build to the project root's `/Build` and run.
4. Player1 clicks "Server" and "Client" (both).
5. Player2 clicks "Client".

Both players should spawn; see each other move!

![image](https://user-images.githubusercontent.com/8840024/233584587-56352006-9103-4d3c-a817-fbedd21f0fe0.png)

![image](https://i.imgur.com/dMXjRGy.png)

* **Optional:** To simulate a dedicated server, run the build from the console with `-mode server` args, optionally with `-batchmode` and `-nographics` to run headless (may need to manually kill the process). Append `-logFile` to log in a local file.

## Hathora Dedicated Server

### Prerequisites

1. Register or login: https://console.hathora.dev/
2. Create an app: Click `Create an Application` >> Use `UDP` and port `7777` for this demo >> Choose `Tiny` plan with 1 room-per-process.
3. See Quickstart below for the `Upload server build` page.

### Deploy via scripts

Follow these naming conventions to use deployment scripts:

1. Create `Build-Server` dir at Unity project root >> Build here via Dedicated Server (Linux).
2. Run the repo root's `utils/DeployToHathora.ps1` PowerShell script to automatically deploy your app and launch the web console.
3. At the top-right of the web console, choose your closest region and create a room (normally done programmatically) that will appear under "Active Processes".

![image](https://user-images.githubusercontent.com/8840024/233578161-630e86bf-0bcd-4c43-9d97-0470367d1cfc.png)

4. Click the room "Details" on the right.

![image](https://i.imgur.com/qhaTYFq.png)

5. In the NetworkManager's `Tugboat` component (from `NetworkManager` GameObject), paste the **Server** `Port` and **Client** Ipv4 (probably a host name, like `1.proxy.hathora.dev`). (!) Note you should leave **Server** IP empty:

![image](https://i.imgur.com/faWmgcO.png)

6. Press Play within the Unity editor >> "Client" button >> You should spawn in (you _only_ spawn in if the Server is found)! Try pressing `[R]` to RPC the server for a ping/pong.

You have now successfully set up and tested your Hathora Unity project! If you encounter any issues or need further assistance, please consult the documentation or reach out to the community for support. Happy coding!

# Troubleshooting

> What do I need to do with the deployment container's port?

* Nothing! This is arbitrary: Set and forget. You'll be using the _room's_ ip:port; not the container's port.

> I tried copy+pasting a host name (not a pure IP address) into the NetworkManager's `Address` property, but I'm instantly getting a timeout.

* Within the Hathora web console details page, ensure the logs parity with what you normally see in Unity to ensure !errors.

* In the NetworkManager's `Tugboat` component (from `NetworkManager` GameObject), ensure the `Client Address` and `Server Port` match your room's server:port.

* Unity NGO requires raw IP addresses (opposed to host names, like `proxy.hathora.dev` or `localhost`). If you only have a host name, try converting to an IP address via a tool like https://whatismyipaddress.com/hostname-ip . If you use NGO, you probably want to programatically do this, eventually.

> When I tried implementing this tutorial logic into my own game, both Player GameObjects move at the same time from 1 PC - how come?

* For your player controller's `Update()`, you may want to add `if (!IsOwner) return` so only the owned network object can be controlled.

> When using _both_ the "Server" + "Client" buttons (Player1 'Host Mode'), both players spawn; Player1 can move their `NetworkPlayer` and see the movement on Player2 when they pressed the `Client` button (Player2). Why can I only move Player1 and not Player2?

* Player2 needs _permission_ from the Server to move: If following another guide, you may be using the `Network Transform` script. While this is server-authoritative and more-secure, the demo makes use of `Client Network Transform` to give power to the Client for demo purposes and to keep code minimal.

> Why does the other player look like his model isn't using animations; just "gliding" while moving?

* Be sure to add a `Network Transform` component to your Player! To save bandwidth, uncheck the `syncing` options you don't need (such as Scale).

> With Unity NGO: When I override OnNetworkSpawn() and check ownership of the Player (as a client), I'm the owner. The next time I call test ping, I'm not! What's going on?

* This seems to be a Unity NGO bug: The server appears to take ownership a few moments *after* the client spawns. You can async/await for !IsOwner. This is likely because clients can join *before* a server kicks in.

> I tried swapping Fishnet for Unity NGO, but experiencing oddities.

* Remember that Unity NGO is in very early access: We recommend you use a production-ready service, for now.

# TODO

1. Add a license to this repo.
2. Launch the Hathora Deployment tool at post-build (within Unity editor), prompting if you wish to deploy now.

# License

TODO
