# LobbyV3
(*LobbyV3*)

### Available Operations

* [CreateLobby](#createlobby) - Create a new [lobby](https://hathora.dev/docs/concepts/hathora-entities#lobby) for an existing [application](https://hathora.dev/docs/concepts/hathora-entities#application) using `appId`.
* [GetLobbyInfoByRoomId](#getlobbyinfobyroomid) - Get details for an existing [lobby](https://hathora.dev/docs/concepts/hathora-entities#lobby) using `appId` and `roomId`.
* [GetLobbyInfoByShortCode](#getlobbyinfobyshortcode) - Get details for an existing [lobby](https://hathora.dev/docs/concepts/hathora-entities#lobby) using `appId` and `roomId`.
* [ListActivePublicLobbies](#listactivepubliclobbies) - Get all active [lobbies](https://hathora.dev/docs/concepts/hathora-entities#lobby) for a given [application](https://hathora.dev/docs/concepts/hathora-entities#application) using `appId`. Filter the array by optionally passing in a `region`.

## CreateLobby

Create a new [lobby](https://hathora.dev/docs/concepts/hathora-entities#lobby) for an existing [application](https://hathora.dev/docs/concepts/hathora-entities#application) using `appId`.

### Example Usage

```csharp
using HathoraCloud;
using HathoraCloud.Models.Operations;
using HathoraCloud.Models.Shared;

var sdk = new HathoraCloudSDK(
    appId: "app-af469a92-5b45-4565-b3c4-b79878de67d2"
);

using(var res = await sdk.LobbyV3.CreateLobbyAsync(new CreateLobbySecurity() {
        PlayerAuth = "",
    }, new CreateLobbyRequest() {
        CreateLobbyV3Params = new CreateLobbyV3Params() {
            Region = HathoraCloud.Models.Shared.Region.Chicago,
            RoomConfig = "{\"name\":\"my-room\"}",
            Visibility = HathoraCloud.Models.Shared.LobbyVisibility.Private,
        },
        ShortCode = "LFG4",
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                             | Type                                                                  | Required                                                              | Description                                                           |
| --------------------------------------------------------------------- | --------------------------------------------------------------------- | --------------------------------------------------------------------- | --------------------------------------------------------------------- |
| `request`                                                             | [CreateLobbyRequest](../../models/operations/CreateLobbyRequest.md)   | :heavy_check_mark:                                                    | The request object to use for the request.                            |
| `security`                                                            | [CreateLobbySecurity](../../models/operations/CreateLobbySecurity.md) | :heavy_check_mark:                                                    | The security requirements to use for the request.                     |


### Response

**[CreateLobbyResponse](../../models/operations/CreateLobbyResponse.md)**


## GetLobbyInfoByRoomId

Get details for an existing [lobby](https://hathora.dev/docs/concepts/hathora-entities#lobby) using `appId` and `roomId`.

### Example Usage

```csharp
using HathoraCloud;
using HathoraCloud.Models.Shared;
using HathoraCloud.Models.Operations;

var sdk = new HathoraCloudSDK(
    security: new Security() {
        HathoraDevToken = "",
    },
    appId: "app-af469a92-5b45-4565-b3c4-b79878de67d2"
);

using(var res = await sdk.LobbyV3.GetLobbyInfoByRoomIdAsync(new GetLobbyInfoByRoomIdRequest() {
        RoomId = "2swovpy1fnunu",
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                                             | Type                                                                                  | Required                                                                              | Description                                                                           |
| ------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------- |
| `request`                                                                             | [GetLobbyInfoByRoomIdRequest](../../models/operations/GetLobbyInfoByRoomIdRequest.md) | :heavy_check_mark:                                                                    | The request object to use for the request.                                            |


### Response

**[GetLobbyInfoByRoomIdResponse](../../models/operations/GetLobbyInfoByRoomIdResponse.md)**


## GetLobbyInfoByShortCode

Get details for an existing [lobby](https://hathora.dev/docs/concepts/hathora-entities#lobby) using `appId` and `roomId`.

### Example Usage

```csharp
using HathoraCloud;
using HathoraCloud.Models.Shared;
using HathoraCloud.Models.Operations;

var sdk = new HathoraCloudSDK(
    security: new Security() {
        HathoraDevToken = "",
    },
    appId: "app-af469a92-5b45-4565-b3c4-b79878de67d2"
);

using(var res = await sdk.LobbyV3.GetLobbyInfoByShortCodeAsync(new GetLobbyInfoByShortCodeRequest() {
        ShortCode = "LFG4",
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                                                   | Type                                                                                        | Required                                                                                    | Description                                                                                 |
| ------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- |
| `request`                                                                                   | [GetLobbyInfoByShortCodeRequest](../../models/operations/GetLobbyInfoByShortCodeRequest.md) | :heavy_check_mark:                                                                          | The request object to use for the request.                                                  |


### Response

**[GetLobbyInfoByShortCodeResponse](../../models/operations/GetLobbyInfoByShortCodeResponse.md)**


## ListActivePublicLobbies

Get all active [lobbies](https://hathora.dev/docs/concepts/hathora-entities#lobby) for a given [application](https://hathora.dev/docs/concepts/hathora-entities#application) using `appId`. Filter the array by optionally passing in a `region`.

### Example Usage

```csharp
using HathoraCloud;
using HathoraCloud.Models.Shared;
using HathoraCloud.Models.Operations;

var sdk = new HathoraCloudSDK(
    security: new Security() {
        HathoraDevToken = "",
    },
    appId: "app-af469a92-5b45-4565-b3c4-b79878de67d2"
);

using(var res = await sdk.LobbyV3.ListActivePublicLobbiesAsync(new ListActivePublicLobbiesRequest() {
        Region = HathoraCloud.Models.Shared.Region.London,
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                                                   | Type                                                                                        | Required                                                                                    | Description                                                                                 |
| ------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- |
| `request`                                                                                   | [ListActivePublicLobbiesRequest](../../models/operations/ListActivePublicLobbiesRequest.md) | :heavy_check_mark:                                                                          | The request object to use for the request.                                                  |


### Response

**[ListActivePublicLobbiesResponse](../../models/operations/ListActivePublicLobbiesResponse.md)**

