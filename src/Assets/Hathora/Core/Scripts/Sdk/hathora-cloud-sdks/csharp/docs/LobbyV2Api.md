# Hathora.Cloud.Sdk.Api.LobbyV2Api

All URIs are relative to *https://api.hathora.dev*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CreateLobby**](LobbyV2Api.md#createlobby) | **POST** /lobby/v2/{appId}/create |  |
| [**CreateLocalLobby**](LobbyV2Api.md#createlocallobby) | **POST** /lobby/v2/{appId}/create/local |  |
| [**CreatePrivateLobby**](LobbyV2Api.md#createprivatelobby) | **POST** /lobby/v2/{appId}/create/private |  |
| [**CreatePublicLobby**](LobbyV2Api.md#createpubliclobby) | **POST** /lobby/v2/{appId}/create/public |  |
| [**GetLobbyInfo**](LobbyV2Api.md#getlobbyinfo) | **GET** /lobby/v2/{appId}/info/{roomId} |  |
| [**ListActivePublicLobbies**](LobbyV2Api.md#listactivepubliclobbies) | **GET** /lobby/v2/{appId}/list/public |  |
| [**SetLobbyState**](LobbyV2Api.md#setlobbystate) | **POST** /lobby/v2/{appId}/setState/{roomId} |  |

<a name="createlobby"></a>
# **CreateLobby**
> Lobby CreateLobby (string appId, string authorization, CreateLobbyRequest createLobbyRequest, string roomId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class CreateLobbyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new LobbyV2Api(config);
            var appId = "appId_example";  // string | 
            var authorization = "authorization_example";  // string | 
            var createLobbyRequest = new CreateLobbyRequest(); // CreateLobbyRequest | 
            var roomId = "roomId_example";  // string |  (optional) 

            try
            {
                Lobby result = apiInstance.CreateLobby(appId, authorization, createLobbyRequest, roomId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV2Api.CreateLobby: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateLobbyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Lobby> response = apiInstance.CreateLobbyWithHttpInfo(appId, authorization, createLobbyRequest, roomId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV2Api.CreateLobbyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **authorization** | **string** |  |  |
| **createLobbyRequest** | [**CreateLobbyRequest**](CreateLobbyRequest.md) |  |  |
| **roomId** | **string** |  | [optional]  |

### Return type

[**Lobby**](Lobby.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** |  |  -  |
| **400** |  |  -  |
| **401** |  |  -  |
| **404** |  |  -  |
| **422** |  |  -  |
| **429** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="createlocallobby"></a>
# **CreateLocalLobby**
> Lobby CreateLocalLobby (string appId, string authorization, CreatePrivateLobbyRequest createPrivateLobbyRequest, string roomId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class CreateLocalLobbyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new LobbyV2Api(config);
            var appId = "appId_example";  // string | 
            var authorization = "authorization_example";  // string | 
            var createPrivateLobbyRequest = new CreatePrivateLobbyRequest(); // CreatePrivateLobbyRequest | 
            var roomId = "roomId_example";  // string |  (optional) 

            try
            {
                Lobby result = apiInstance.CreateLocalLobby(appId, authorization, createPrivateLobbyRequest, roomId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV2Api.CreateLocalLobby: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateLocalLobbyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Lobby> response = apiInstance.CreateLocalLobbyWithHttpInfo(appId, authorization, createPrivateLobbyRequest, roomId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV2Api.CreateLocalLobbyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **authorization** | **string** |  |  |
| **createPrivateLobbyRequest** | [**CreatePrivateLobbyRequest**](CreatePrivateLobbyRequest.md) |  |  |
| **roomId** | **string** |  | [optional]  |

### Return type

[**Lobby**](Lobby.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** |  |  -  |
| **400** |  |  -  |
| **401** |  |  -  |
| **404** |  |  -  |
| **422** |  |  -  |
| **429** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="createprivatelobby"></a>
# **CreatePrivateLobby**
> Lobby CreatePrivateLobby (string appId, string authorization, CreatePrivateLobbyRequest createPrivateLobbyRequest, string roomId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class CreatePrivateLobbyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new LobbyV2Api(config);
            var appId = "appId_example";  // string | 
            var authorization = "authorization_example";  // string | 
            var createPrivateLobbyRequest = new CreatePrivateLobbyRequest(); // CreatePrivateLobbyRequest | 
            var roomId = "roomId_example";  // string |  (optional) 

            try
            {
                Lobby result = apiInstance.CreatePrivateLobby(appId, authorization, createPrivateLobbyRequest, roomId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV2Api.CreatePrivateLobby: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreatePrivateLobbyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Lobby> response = apiInstance.CreatePrivateLobbyWithHttpInfo(appId, authorization, createPrivateLobbyRequest, roomId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV2Api.CreatePrivateLobbyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **authorization** | **string** |  |  |
| **createPrivateLobbyRequest** | [**CreatePrivateLobbyRequest**](CreatePrivateLobbyRequest.md) |  |  |
| **roomId** | **string** |  | [optional]  |

### Return type

[**Lobby**](Lobby.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** |  |  -  |
| **400** |  |  -  |
| **401** |  |  -  |
| **404** |  |  -  |
| **422** |  |  -  |
| **429** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="createpubliclobby"></a>
# **CreatePublicLobby**
> Lobby CreatePublicLobby (string appId, string authorization, CreatePrivateLobbyRequest createPrivateLobbyRequest, string roomId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class CreatePublicLobbyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new LobbyV2Api(config);
            var appId = "appId_example";  // string | 
            var authorization = "authorization_example";  // string | 
            var createPrivateLobbyRequest = new CreatePrivateLobbyRequest(); // CreatePrivateLobbyRequest | 
            var roomId = "roomId_example";  // string |  (optional) 

            try
            {
                Lobby result = apiInstance.CreatePublicLobby(appId, authorization, createPrivateLobbyRequest, roomId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV2Api.CreatePublicLobby: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreatePublicLobbyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Lobby> response = apiInstance.CreatePublicLobbyWithHttpInfo(appId, authorization, createPrivateLobbyRequest, roomId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV2Api.CreatePublicLobbyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **authorization** | **string** |  |  |
| **createPrivateLobbyRequest** | [**CreatePrivateLobbyRequest**](CreatePrivateLobbyRequest.md) |  |  |
| **roomId** | **string** |  | [optional]  |

### Return type

[**Lobby**](Lobby.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** |  |  -  |
| **400** |  |  -  |
| **401** |  |  -  |
| **404** |  |  -  |
| **422** |  |  -  |
| **429** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="getlobbyinfo"></a>
# **GetLobbyInfo**
> Lobby GetLobbyInfo (string appId, string roomId)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class GetLobbyInfoExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new LobbyV2Api(config);
            var appId = "appId_example";  // string | 
            var roomId = "roomId_example";  // string | 

            try
            {
                Lobby result = apiInstance.GetLobbyInfo(appId, roomId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV2Api.GetLobbyInfo: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetLobbyInfoWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Lobby> response = apiInstance.GetLobbyInfoWithHttpInfo(appId, roomId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV2Api.GetLobbyInfoWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **roomId** | **string** |  |  |

### Return type

[**Lobby**](Lobby.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="listactivepubliclobbies"></a>
# **ListActivePublicLobbies**
> List&lt;Lobby&gt; ListActivePublicLobbies (string appId, Region? region = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class ListActivePublicLobbiesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new LobbyV2Api(config);
            var appId = "appId_example";  // string | 
            var region = (Region) "Seattle";  // Region? |  (optional) 

            try
            {
                List<Lobby> result = apiInstance.ListActivePublicLobbies(appId, region);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV2Api.ListActivePublicLobbies: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ListActivePublicLobbiesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<Lobby>> response = apiInstance.ListActivePublicLobbiesWithHttpInfo(appId, region);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV2Api.ListActivePublicLobbiesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
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

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="setlobbystate"></a>
# **SetLobbyState**
> Lobby SetLobbyState (string appId, string roomId, SetLobbyStateRequest setLobbyStateRequest)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class SetLobbyStateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new LobbyV2Api(config);
            var appId = "appId_example";  // string | 
            var roomId = "roomId_example";  // string | 
            var setLobbyStateRequest = new SetLobbyStateRequest(); // SetLobbyStateRequest | 

            try
            {
                Lobby result = apiInstance.SetLobbyState(appId, roomId, setLobbyStateRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LobbyV2Api.SetLobbyState: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SetLobbyStateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Lobby> response = apiInstance.SetLobbyStateWithHttpInfo(appId, roomId, setLobbyStateRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LobbyV2Api.SetLobbyStateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **roomId** | **string** |  |  |
| **setLobbyStateRequest** | [**SetLobbyStateRequest**](SetLobbyStateRequest.md) |  |  |

### Return type

[**Lobby**](Lobby.md)

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** |  |  -  |
| **422** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

