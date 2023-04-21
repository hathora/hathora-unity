# Hathora Unity Quickstart Guide

Welcome to the Hathora Unity Quickstart Guide! This guide will help you set up and test a simple 3rd-person 3D Unity project using Unity's Network Game Objects (NGO) as an example. The net code in this project can be replaced with any net code, such as Mirror or Fishnet.

## Prerequisites

Before you start, ensure you have the following:

1. Unity 2021 LTS installed.
2. Linux dedicated server build support (available from Unity Hub).

![image](https://user-images.githubusercontent.com/8840024/233582785-3755eb9c-584f-4cd0-b798-6f29eccacf4a.png)

## Project Setup

1. Open the project in Unity 2021 LTS.
2. Open the `Playground` scene.

## Local Testing Quickstart

1. Build to the project root's `/Build` and run.
2. Player1 "Host + Join" <> Player2 "Join as Client".
3. Both players should spawn together.

![image](https://user-images.githubusercontent.com/8840024/233584587-56352006-9103-4d3c-a817-fbedd21f0fe0.png)

![image](https://user-images.githubusercontent.com/8840024/233575333-00ff64e9-9728-4c40-9001-5bd773f29718.png)

## Hathora Dedicated Server

### Prerequisites

1. Register or login: https://console.hathora.dev/
2. Create an app: Click `Create an Application` >> Ensure the use of `UDP` and port `7777` for this demo >> `Tiny` plan with 1 room-per-process will do.
3. On the `Upload server build` page, see Quickstart below.



### Quickstart

1. Build via Dedicated Server (Linux) to the project root's `/Build-Server`.
2. Run the repo root's `/utils/PrepUploadToHathora.ps1` PowerShell script (commented) to prepare for upload.
3. Drop the archived `/utils/uploadToHathora/Build-Server.tar.gz` in the Hathora `Upload server build` browser window (from prerequisites step #3).
4. At the top-right, choose your closest region and create a room (normally done programatically) >> Note the IP and port.

![image](https://user-images.githubusercontent.com/8840024/233578161-630e86bf-0bcd-4c43-9d97-0470367d1cfc.png)

5. Ensure the logs parity with what you saw in Unity, then hop in your Unity editor's `NetworkManager` inspector.
6. Within the `Unity Transport` component (at the bottom), change the `address` and paste the ip/port from step #4. Note only IP addresses (not host names) work with NGO; a Unity restriction.

![image](https://user-images.githubusercontent.com/8840024/233578298-3e884881-2aa2-4fdc-8bd9-104d94f849f4.png)

7. Press Play within the Unity editor >> "Join as Host" >> Both players should spawn together.

You have now successfully set up and tested your Hathora Unity project! If you encounter any issues or need further assistance, please consult the documentation or reach out to the community for support. Happy coding!

### TODO

1. Add a license to this repo.
2. Automated powershell script to upload to Hathora via Hathora CLI.

# Troubleshooting

> I tried copy+pasting a host name (not a pure IP address) into the NetworkManager's `Address` property, but I'm instantly getting a timeout.

* In the NetworkManager's UnityTransport component (from NetMgr GameObject), ensure that `Allow Remote Connections` is checked at the bottom. This won't have much of an issue for local testing, but only when uploaded to Hathora.
* Unity NGO requires raw IP addresses. If you only have a host name, try converting to an IP address via a tool like https://whatismyipaddress.com/hostname-ip 

> Both `NetworkPlayer`s are stacked on each other and move at the same time - how come?

For your player controller's `Update()`, you may want to add `if (!IsOwner) return` so only the owned network object can be controlled.

> When using "Host + Join" (Player1), both players spawn; Player1 can move their `NetworkPlayer` and see the movement on Player2 when they used `Join as Client` (Player2). Why can I only move Player1 and not Player2?

Player2 needs permission from the Server to move: If following another guide, you may be using the `Network Transform` script. While this is server-authoritative and more-secure, the demo makes use of `Client Network Transform` to give power to the Client for demo purposes and to keep code minimal.

> Why does the other player look like his model isn't using animations; just "gliding" while moving?

Be sure to add a `Network Transform` component to your Player! To save bandwidth, uncheck the `syncing` options you don't need (such as Scale).

# License
TODO
