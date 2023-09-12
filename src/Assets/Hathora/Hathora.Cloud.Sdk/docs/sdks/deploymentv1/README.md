# deploymentV1

## Overview

Operations that allow you configure and manage an application's [build](https://hathora.dev/docs/concepts/hathora-entities#build) at runtime.

### Available Operations

* [CreateDeployment](#createdeployment) - Create a new [deployment](https://hathora.dev/docs/concepts/hathora-entities#deployment) for an existing [application](https://hathora.dev/docs/concepts/hathora-entities#application) and [build](https://hathora.dev/docs/concepts/hathora-entities#build).
* [GetDeploymentInfo](#getdeploymentinfo) - Get details for an existing [deployment](https://hathora.dev/docs/concepts/hathora-entities#deployment) using `appId`.
* [GetDeployments](#getdeployments) - Returns an array of [deployment](https://hathora.dev/docs/concepts/hathora-entities#deployment) objects for an existing [application](https://hathora.dev/docs/concepts/hathora-entities#application) using `appId`.

## CreateDeployment

Create a new [deployment](https://hathora.dev/docs/concepts/hathora-entities#deployment) for an existing [application](https://hathora.dev/docs/concepts/hathora-entities#application) and [build](https://hathora.dev/docs/concepts/hathora-entities#build).

### Example Usage

```csharp
using Hathora;
using HathoraSdk.Models.Operations;
using HathoraSdk.Models.Shared;

var sdk = new HathoraSDK();

using(var res = await sdk.DeploymentV1.CreateDeploymentAsync(new CreateDeploymentSecurity() {
        Auth0 = "",
    }, new CreateDeploymentRequest() {
        DeploymentConfig = new DeploymentConfig() {
            AdditionalContainerPorts = new List<ContainerPort>() {
                new ContainerPort() {
                    Name = "default",
                    Port = 8000,
                    TransportType = HathoraSdk.Models.Shared.TransportType.Udp,
                },
            },
            ContainerPort = 4000,
            Env = new List<DeploymentConfigEnv>() {
                new DeploymentConfigEnv() {
                    Name = "EULA",
                    Value = "TRUE",
                },
            },
            PlanName = HathoraSdk.Models.Shared.PlanName.Tiny,
            RoomsPerProcess = 3,
            TransportType = HathoraSdk.Models.Shared.TransportType.Tls,
        },
        AppId = "app-af469a92-5b45-4565-b3c4-b79878de67d2",
        BuildId = 1,
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                                       | Type                                                                            | Required                                                                        | Description                                                                     |
| ------------------------------------------------------------------------------- | ------------------------------------------------------------------------------- | ------------------------------------------------------------------------------- | ------------------------------------------------------------------------------- |
| `request`                                                                       | [CreateDeploymentRequest](../../models/operations/CreateDeploymentRequest.md)   | :heavy_check_mark:                                                              | The request object to use for the request.                                      |
| `security`                                                                      | [CreateDeploymentSecurity](../../models/operations/CreateDeploymentSecurity.md) | :heavy_check_mark:                                                              | The security requirements to use for the request.                               |


### Response

**[CreateDeploymentResponse](../../models/operations/CreateDeploymentResponse.md)**


## GetDeploymentInfo

Get details for an existing [deployment](https://hathora.dev/docs/concepts/hathora-entities#deployment) using `appId`.

### Example Usage

```csharp
using Hathora;
using HathoraSdk.Models.Operations;

var sdk = new HathoraSDK();

using(var res = await sdk.DeploymentV1.GetDeploymentInfoAsync(new GetDeploymentInfoSecurity() {
        Auth0 = "",
    }, new GetDeploymentInfoRequest() {
        AppId = "app-af469a92-5b45-4565-b3c4-b79878de67d2",
        DeploymentId = 1,
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                                         | Type                                                                              | Required                                                                          | Description                                                                       |
| --------------------------------------------------------------------------------- | --------------------------------------------------------------------------------- | --------------------------------------------------------------------------------- | --------------------------------------------------------------------------------- |
| `request`                                                                         | [GetDeploymentInfoRequest](../../models/operations/GetDeploymentInfoRequest.md)   | :heavy_check_mark:                                                                | The request object to use for the request.                                        |
| `security`                                                                        | [GetDeploymentInfoSecurity](../../models/operations/GetDeploymentInfoSecurity.md) | :heavy_check_mark:                                                                | The security requirements to use for the request.                                 |


### Response

**[GetDeploymentInfoResponse](../../models/operations/GetDeploymentInfoResponse.md)**


## GetDeployments

Returns an array of [deployment](https://hathora.dev/docs/concepts/hathora-entities#deployment) objects for an existing [application](https://hathora.dev/docs/concepts/hathora-entities#application) using `appId`.

### Example Usage

```csharp
using Hathora;
using HathoraSdk.Models.Operations;

var sdk = new HathoraSDK();

using(var res = await sdk.DeploymentV1.GetDeploymentsAsync(new GetDeploymentsSecurity() {
        Auth0 = "",
    }, new GetDeploymentsRequest() {
        AppId = "app-af469a92-5b45-4565-b3c4-b79878de67d2",
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                                   | Type                                                                        | Required                                                                    | Description                                                                 |
| --------------------------------------------------------------------------- | --------------------------------------------------------------------------- | --------------------------------------------------------------------------- | --------------------------------------------------------------------------- |
| `request`                                                                   | [GetDeploymentsRequest](../../models/operations/GetDeploymentsRequest.md)   | :heavy_check_mark:                                                          | The request object to use for the request.                                  |
| `security`                                                                  | [GetDeploymentsSecurity](../../models/operations/GetDeploymentsSecurity.md) | :heavy_check_mark:                                                          | The security requirements to use for the request.                           |


### Response

**[GetDeploymentsResponse](../../models/operations/GetDeploymentsResponse.md)**

