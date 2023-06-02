# Hathora.Cloud.Sdk.Api.RoomV1Api

All URIs are relative to *https://api.hathora.dev*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CreateRoomDeprecated**](RoomV1Api.md#createroomdeprecated) | **POST** /rooms/v1/{appId}/create |  |
| [**DestroyRoomDeprecated**](RoomV1Api.md#destroyroomdeprecated) | **POST** /rooms/v1/{appId}/destroy/{roomId} |  |
| [**GetActiveRoomsForProcessDeprecated**](RoomV1Api.md#getactiveroomsforprocessdeprecated) | **GET** /rooms/v1/{appId}/list/{processId}/active |  |
| [**GetConnectionInfoDeprecated**](RoomV1Api.md#getconnectioninfodeprecated) | **GET** /rooms/v1/{appId}/connectioninfo/{roomId} |  |
| [**GetInactiveRoomsForProcessDeprecated**](RoomV1Api.md#getinactiveroomsforprocessdeprecated) | **GET** /rooms/v1/{appId}/list/{processId}/inactive |  |
| [**GetRoomInfoDeprecated**](RoomV1Api.md#getroominfodeprecated) | **GET** /rooms/v1/{appId}/info/{roomId} |  |
| [**SuspendRoomDeprecated**](RoomV1Api.md#suspendroomdeprecated) | **POST** /rooms/v1/{appId}/suspend/{roomId} |  |

<a name="createroomdeprecated"></a>
# **CreateRoomDeprecated**
> string CreateRoomDeprecated (string appId, CreateRoomRequest createRoomRequest, string roomId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class CreateRoomDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new RoomV1Api(config);
            var appId = "appId_example";  // string | 
            var createRoomRequest = new CreateRoomRequest(); // CreateRoomRequest | 
            var roomId = "roomId_example";  // string |  (optional) 

            try
            {
                string result = apiInstance.CreateRoomDeprecated(appId, createRoomRequest, roomId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RoomV1Api.CreateRoomDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CreateRoomDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CreateRoomDeprecatedWithHttpInfo(appId, createRoomRequest, roomId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RoomV1Api.CreateRoomDeprecatedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **createRoomRequest** | [**CreateRoomRequest**](CreateRoomRequest.md) |  |  |
| **roomId** | **string** |  | [optional]  |

### Return type

**string**

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** |  |  -  |
| **400** |  |  -  |
| **404** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="destroyroomdeprecated"></a>
# **DestroyRoomDeprecated**
> void DestroyRoomDeprecated (string appId, string roomId)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class DestroyRoomDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new RoomV1Api(config);
            var appId = "appId_example";  // string | 
            var roomId = "roomId_example";  // string | 

            try
            {
                apiInstance.DestroyRoomDeprecated(appId, roomId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RoomV1Api.DestroyRoomDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DestroyRoomDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.DestroyRoomDeprecatedWithHttpInfo(appId, roomId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RoomV1Api.DestroyRoomDeprecatedWithHttpInfo: " + e.Message);
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

void (empty response body)

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | No content |  -  |
| **404** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="getactiveroomsforprocessdeprecated"></a>
# **GetActiveRoomsForProcessDeprecated**
> List&lt;PickRoomExcludeKeyofRoomAllocations&gt; GetActiveRoomsForProcessDeprecated (string appId, string processId)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class GetActiveRoomsForProcessDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new RoomV1Api(config);
            var appId = "appId_example";  // string | 
            var processId = "processId_example";  // string | 

            try
            {
                List<PickRoomExcludeKeyofRoomAllocations> result = apiInstance.GetActiveRoomsForProcessDeprecated(appId, processId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RoomV1Api.GetActiveRoomsForProcessDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetActiveRoomsForProcessDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<PickRoomExcludeKeyofRoomAllocations>> response = apiInstance.GetActiveRoomsForProcessDeprecatedWithHttpInfo(appId, processId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RoomV1Api.GetActiveRoomsForProcessDeprecatedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **processId** | **string** |  |  |

### Return type

[**List&lt;PickRoomExcludeKeyofRoomAllocations&gt;**](PickRoomExcludeKeyofRoomAllocations.md)

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="getconnectioninfodeprecated"></a>
# **GetConnectionInfoDeprecated**
> ConnectionInfo GetConnectionInfoDeprecated (string appId, string roomId)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class GetConnectionInfoDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            var apiInstance = new RoomV1Api(config);
            var appId = "appId_example";  // string | 
            var roomId = "roomId_example";  // string | 

            try
            {
                ConnectionInfo result = apiInstance.GetConnectionInfoDeprecated(appId, roomId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RoomV1Api.GetConnectionInfoDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetConnectionInfoDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ConnectionInfo> response = apiInstance.GetConnectionInfoDeprecatedWithHttpInfo(appId, roomId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RoomV1Api.GetConnectionInfoDeprecatedWithHttpInfo: " + e.Message);
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

[**ConnectionInfo**](ConnectionInfo.md)

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
| **404** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="getinactiveroomsforprocessdeprecated"></a>
# **GetInactiveRoomsForProcessDeprecated**
> List&lt;PickRoomExcludeKeyofRoomAllocations&gt; GetInactiveRoomsForProcessDeprecated (string appId, string processId)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class GetInactiveRoomsForProcessDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new RoomV1Api(config);
            var appId = "appId_example";  // string | 
            var processId = "processId_example";  // string | 

            try
            {
                List<PickRoomExcludeKeyofRoomAllocations> result = apiInstance.GetInactiveRoomsForProcessDeprecated(appId, processId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RoomV1Api.GetInactiveRoomsForProcessDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetInactiveRoomsForProcessDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<PickRoomExcludeKeyofRoomAllocations>> response = apiInstance.GetInactiveRoomsForProcessDeprecatedWithHttpInfo(appId, processId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RoomV1Api.GetInactiveRoomsForProcessDeprecatedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **processId** | **string** |  |  |

### Return type

[**List&lt;PickRoomExcludeKeyofRoomAllocations&gt;**](PickRoomExcludeKeyofRoomAllocations.md)

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="getroominfodeprecated"></a>
# **GetRoomInfoDeprecated**
> Room GetRoomInfoDeprecated (string appId, string roomId)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class GetRoomInfoDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new RoomV1Api(config);
            var appId = "appId_example";  // string | 
            var roomId = "roomId_example";  // string | 

            try
            {
                Room result = apiInstance.GetRoomInfoDeprecated(appId, roomId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RoomV1Api.GetRoomInfoDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetRoomInfoDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Room> response = apiInstance.GetRoomInfoDeprecatedWithHttpInfo(appId, roomId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RoomV1Api.GetRoomInfoDeprecatedWithHttpInfo: " + e.Message);
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

[**Room**](Room.md)

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="suspendroomdeprecated"></a>
# **SuspendRoomDeprecated**
> void SuspendRoomDeprecated (string appId, string roomId)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class SuspendRoomDeprecatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new RoomV1Api(config);
            var appId = "appId_example";  // string | 
            var roomId = "roomId_example";  // string | 

            try
            {
                apiInstance.SuspendRoomDeprecated(appId, roomId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RoomV1Api.SuspendRoomDeprecated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SuspendRoomDeprecatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SuspendRoomDeprecatedWithHttpInfo(appId, roomId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RoomV1Api.SuspendRoomDeprecatedWithHttpInfo: " + e.Message);
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

void (empty response body)

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | No content |  -  |
| **404** |  |  -  |
| **500** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

