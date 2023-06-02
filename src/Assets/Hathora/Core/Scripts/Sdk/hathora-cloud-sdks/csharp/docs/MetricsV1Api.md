# Hathora.Cloud.Sdk.Api.MetricsV1Api

All URIs are relative to *https://api.hathora.dev*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetMetrics**](MetricsV1Api.md#getmetrics) | **GET** /metrics/v1/{appId}/process/{processId} |  |

<a name="getmetrics"></a>
# **GetMetrics**
> RecordPartialMetricNameMetricValueArray GetMetrics (string appId, string processId, List<MetricName> metrics = null, int? end = null, int? start = null, int? step = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Hathora.Cloud.Sdk.Api;
using Hathora.Cloud.Sdk.Client;
using Hathora.Cloud.Sdk.Model;

namespace Example
{
    public class GetMetricsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.hathora.dev";
            // Configure Bearer token for authorization: auth0
            config.AccessToken = "YOUR_BEARER_TOKEN";

            var apiInstance = new MetricsV1Api(config);
            var appId = "appId_example";  // string | 
            var processId = "processId_example";  // string | 
            var metrics = new List<MetricName>(); // List<MetricName> |  (optional) 
            var end = 56;  // int? |  (optional) 
            var start = 56;  // int? |  (optional) 
            var step = 60;  // int? |  (optional)  (default to 60)

            try
            {
                RecordPartialMetricNameMetricValueArray result = apiInstance.GetMetrics(appId, processId, metrics, end, start, step);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MetricsV1Api.GetMetrics: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetMetricsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<RecordPartialMetricNameMetricValueArray> response = apiInstance.GetMetricsWithHttpInfo(appId, processId, metrics, end, start, step);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MetricsV1Api.GetMetricsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  |  |
| **processId** | **string** |  |  |
| **metrics** | [**List&lt;MetricName&gt;**](MetricName.md) |  | [optional]  |
| **end** | **int?** |  | [optional]  |
| **start** | **int?** |  | [optional]  |
| **step** | **int?** |  | [optional] [default to 60] |

### Return type

[**RecordPartialMetricNameMetricValueArray**](RecordPartialMetricNameMetricValueArray.md)

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
| **422** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

