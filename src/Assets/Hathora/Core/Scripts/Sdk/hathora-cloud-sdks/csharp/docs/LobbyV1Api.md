# Hathora.Cloud.Sdk.Api.LobbyV1Api

All URIs are relative to *https://api.hathora.dev*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CreatePrivateLobbyDeprecated**](LobbyV1Api.md#createprivatelobbydeprecated) | **POST** /lobby/v1/{appId}/create/private |  |
| [**CreatePublicLobbyDeprecated**](LobbyV1Api.md#createpubliclobbydeprecated) | **POST** /lobby/v1/{appId}/create/public |  |
| [**ListActivePublicLobbiesDeprecated**](LobbyV1Api.md#listactivepubliclobbiesdeprecated) | **GET** /lobby/v1/{appId}/list |  |

<a name="createprivatelobbydeprecated"></a>
# **CreatePrivateLobbyDeprecated**
> string CreatePrivateLobbyDeprecated (string appId, string authorization, Region? region = null, bool? local = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class CreatePrivateLobbyDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new LobbyV1Api(config);
            var appId = "appId_example";  // string | 
            var authorization = "authorization_example";  // string | 
            var region = (Region) "Seattle";  // Region? |  (optional) 
            var local = false;  // bool? |  (optional)  (default to false)

            try
            {
                string result = apiInstance.CreatePrivateLobbyDeprecated(appId, authorization, region, local);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV1Api.CreatePrivateLobbyDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreatePrivateLobbyDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CreatePrivateLobbyDeprecatedWithHttpInfo(appId, authorization, region, local);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV1Api.CreatePrivateLobbyDeprecatedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **authorization** | **string** |  |  |
| **region** | **Region?** |  | [optional]  |
| **local** | **bool?** |  | [optional] [default to false] |

### Return type

**string**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **400** |  |  -  |
| **401** |  |  -  |
| **404** |  |  -  |
| **422** |  |  -  |
| **429** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="createpubliclobbydeprecated"></a>
# **CreatePublicLobbyDeprecated**
> string CreatePublicLobbyDeprecated (string appId, string authorization, Region? region = null, bool? local = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class CreatePublicLobbyDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new LobbyV1Api(config);
            var appId = "appId_example";  // string | 
            var authorization = "authorization_example";  // string | 
            var region = (Region) "Seattle";  // Region? |  (optional) 
            var local = false;  // bool? |  (optional)  (default to false)

            try
            {
                string result = apiInstance.CreatePublicLobbyDeprecated(appId, authorization, region, local);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV1Api.CreatePublicLobbyDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreatePublicLobbyDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CreatePublicLobbyDeprecatedWithHttpInfo(appId, authorization, region, local);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV1Api.CreatePublicLobbyDeprecatedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **authorization** | **string** |  |  |
| **region** | **Region?** |  | [optional]  |
| **local** | **bool?** |  | [optional] [default to false] |

### Return type

**string**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **400** |  |  -  |
| **401** |  |  -  |
| **404** |  |  -  |
| **422** |  |  -  |
| **429** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="listactivepubliclobbiesdeprecated"></a>
# **ListActivePublicLobbiesDeprecated**
> List&lt;Lobby&gt; ListActivePublicLobbiesDeprecated (string appId, string authorization, bool? local = null, Region? region = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class ListActivePublicLobbiesDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new LobbyV1Api(config);
            var appId = "appId_example";  // string | 
            var authorization = "authorization_example";  // string | 
            var local = false;  // bool? |  (optional)  (default to false)
            var region = (Region) "Seattle";  // Region? |  (optional) 

            try
            {
                List<Lobby> result = apiInstance.ListActivePublicLobbiesDeprecated(appId, authorization, local, region);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV1Api.ListActivePublicLobbiesDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ListActivePublicLobbiesDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<Lobby>> response = apiInstance.ListActivePublicLobbiesDeprecatedWithHttpInfo(appId, authorization, local, region);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV1Api.ListActivePublicLobbiesDeprecatedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **authorization** | **string** |  |  |
| **local** | **bool?** |  | [optional] [default to false] |
| **region** | **Region?** |  | [optional]  |

### Return type

[**List&lt;Lobby&gt;**](Lobby.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **401** |  |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

