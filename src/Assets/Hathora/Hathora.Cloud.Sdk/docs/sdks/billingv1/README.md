# billingV1

### Available Operations

* [GetBalance](#getbalance)
* [GetInvoices](#getinvoices)
* [GetPaymentMethod](#getpaymentmethod)
* [InitStripeCustomerPortalUrl](#initstripecustomerportalurl)

## GetBalance

### Example Usage

```csharp
using Hathora;
using HathoraSdk.Models.Operations;

var sdk = new HathoraSDK();

using(var res = await sdk.BillingV1.GetBalanceAsync(new GetBalanceSecurity() {
        Auth0 = "",
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                           | Type                                                                | Required                                                            | Description                                                         |
| ------------------------------------------------------------------- | ------------------------------------------------------------------- | ------------------------------------------------------------------- | ------------------------------------------------------------------- |
| `security`                                                          | [GetBalanceSecurity](../../models/operations/GetBalanceSecurity.md) | :heavy_check_mark:                                                  | The security requirements to use for the request.                   |


### Response

**[GetBalanceResponse](../../models/operations/GetBalanceResponse.md)**


## GetInvoices

### Example Usage

```csharp
using Hathora;
using HathoraSdk.Models.Operations;

var sdk = new HathoraSDK();

using(var res = await sdk.BillingV1.GetInvoicesAsync(new GetInvoicesSecurity() {
        Auth0 = "",
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                             | Type                                                                  | Required                                                              | Description                                                           |
| --------------------------------------------------------------------- | --------------------------------------------------------------------- | --------------------------------------------------------------------- | --------------------------------------------------------------------- |
| `security`                                                            | [GetInvoicesSecurity](../../models/operations/GetInvoicesSecurity.md) | :heavy_check_mark:                                                    | The security requirements to use for the request.                     |


### Response

**[GetInvoicesResponse](../../models/operations/GetInvoicesResponse.md)**


## GetPaymentMethod

### Example Usage

```csharp
using Hathora;
using HathoraSdk.Models.Operations;

var sdk = new HathoraSDK();

using(var res = await sdk.BillingV1.GetPaymentMethodAsync(new GetPaymentMethodSecurity() {
        Auth0 = "",
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                                       | Type                                                                            | Required                                                                        | Description                                                                     |
| ------------------------------------------------------------------------------- | ------------------------------------------------------------------------------- | ------------------------------------------------------------------------------- | ------------------------------------------------------------------------------- |
| `security`                                                                      | [GetPaymentMethodSecurity](../../models/operations/GetPaymentMethodSecurity.md) | :heavy_check_mark:                                                              | The security requirements to use for the request.                               |


### Response

**[GetPaymentMethodResponse](../../models/operations/GetPaymentMethodResponse.md)**


## InitStripeCustomerPortalUrl

### Example Usage

```csharp
using Hathora;
using HathoraSdk.Models.Shared;
using HathoraSdk.Models.Operations;

var sdk = new HathoraSDK();

using(var res = await sdk.BillingV1.InitStripeCustomerPortalUrlAsync(new InitStripeCustomerPortalUrlSecurity() {
        Auth0 = "",
    }, new CustomerPortalUrl() {
        ReturnUrl = "quibusdam",
    }))
{
    // handle response
}
```

### Parameters

| Parameter                                                                                             | Type                                                                                                  | Required                                                                                              | Description                                                                                           |
| ----------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- |
| `request`                                                                                             | [CustomerPortalUrl](../../models/shared/CustomerPortalUrl.md)                                         | :heavy_check_mark:                                                                                    | The request object to use for the request.                                                            |
| `security`                                                                                            | [InitStripeCustomerPortalUrlSecurity](../../models/operations/InitStripeCustomerPortalUrlSecurity.md) | :heavy_check_mark:                                                                                    | The security requirements to use for the request.                                                     |


### Response

**[InitStripeCustomerPortalUrlResponse](../../models/operations/InitStripeCustomerPortalUrlResponse.md)**

