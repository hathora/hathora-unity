# Hathora.Cloud.Sdk.Api.LogV1Api

All URIs are relative to *https://api.hathora.dev*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetLogsForApp**](LogV1Api.md#getlogsforapp) | **GET** /logs/v1/{appId}/all |  |
| [**GetLogsForDeployment**](LogV1Api.md#getlogsfordeployment) | **GET** /logs/v1/{appId}/deployment/{deploymentId} |  |
| [**GetLogsForProcess**](LogV1Api.md#getlogsforprocess) | **GET** /logs/v1/{appId}/process/{processId} |  |

<a name="getlogsforapp"></a>
# **GetLogsForApp**
> byte[] GetLogsForApp (string appId, bool? follow = null, int? tailLines = null, Region? region = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class GetLogsForAppExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new LogV1Api(config);
            var appId = "appId_example";  // string | 
            var follow = false;  // bool? |  (optional)  (default to false)
            var tailLines = 56;  // int? |  (optional) 
            var region = (Region) "Seattle";  // Region? |  (optional) 

            try
            {
                byte[] result = apiInstance.GetLogsForApp(appId, follow, tailLines, region);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LogV1Api.GetLogsForApp: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetLogsForAppWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<byte[]> response = apiInstance.GetLogsForAppWithHttpInfo(appId, follow, tailLines, region);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LogV1Api.GetLogsForAppWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **follow** | **bool?** |  | [optional] [default to false] |
| **tailLines** | **int?** |  | [optional]  |
| **region** | **Region?** |  | [optional]  |

### Return type

**byte[]**

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="getlogsfordeployment"></a>
# **GetLogsForDeployment**
> byte[] GetLogsForDeployment (string appId, double deploymentId, bool? follow = null, int? tailLines = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class GetLogsForDeploymentExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new LogV1Api(config);
            var appId = "appId_example";  // string | 
            var deploymentId = 1.2D;  // double | 
            var follow = false;  // bool? |  (optional)  (default to false)
            var tailLines = 56;  // int? |  (optional) 

            try
            {
                byte[] result = apiInstance.GetLogsForDeployment(appId, deploymentId, follow, tailLines);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LogV1Api.GetLogsForDeployment: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetLogsForDeploymentWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<byte[]> response = apiInstance.GetLogsForDeploymentWithHttpInfo(appId, deploymentId, follow, tailLines);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LogV1Api.GetLogsForDeploymentWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **deploymentId** | **double** |  |  |
| **follow** | **bool?** |  | [optional] [default to false] |
| **tailLines** | **int?** |  | [optional]  |

### Return type

**byte[]**

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="getlogsforprocess"></a>
# **GetLogsForProcess**
> byte[] GetLogsForProcess (string appId, string processId, bool? follow = null, int? tailLines = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class GetLogsForProcessExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new LogV1Api(config);
            var appId = "appId_example";  // string | 
            var processId = "processId_example";  // string | 
            var follow = false;  // bool? |  (optional)  (default to false)
            var tailLines = 56;  // int? |  (optional) 

            try
            {
                byte[] result = apiInstance.GetLogsForProcess(appId, processId, follow, tailLines);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LogV1Api.GetLogsForProcess: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetLogsForProcessWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<byte[]> response = apiInstance.GetLogsForProcessWithHttpInfo(appId, processId, follow, tailLines);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LogV1Api.GetLogsForProcessWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **processId** | **string** |  |  |
| **follow** | **bool?** |  | [optional] [default to false] |
| **tailLines** | **int?** |  | [optional]  |

### Return type

**byte[]**

### Authorization

[auth0](../README.md#auth0)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

