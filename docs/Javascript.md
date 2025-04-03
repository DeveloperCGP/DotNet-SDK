# JavaScript

## Table of Contents
- [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object)
- [Use Case 1: JavaScript Authentication Request](#use-case-1-javascript-authentication-request)
  - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite)
  - [Step 2: Setting Payment Parameters](#step-2-setting-payment-parameters)
  - [Step 3: Sending the Authentication Request and Retrieve it](#step-3-sending-the-authentication-request-and-retrieve-it)
- [Use Case 2: JavaScript Charge Request](#use-case-2-javascript-charge-request)
  - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-1)
  - [Step 2: Setting Payment Parameters for Charge Request](#step-2-setting-payment-parameters-for-charge-request)
  - [Step 3: Sending the Charge Request and Retrieve it](#step-3-sending-the-charge-request-and-retrieve-it)

## Common Prerequisite: Creating Credentials Object

First, instantiate the Credentials object with your merchant details. This includes your Merchant ID and Merchant Pass which are essential for authenticating requests to the AddonPayments API. In this section, we set up the necessary credentials for the payment service. The credentials include the merchant ID, merchant password, environment, product ID, and API version.

### Steps

- **Initialize Credentials Object:** Create a new instance of the Credentials class to hold the authentication and configuration details.
- **Set Merchant ID:** Assign the merchant ID using the SetMerchantId method. This ID is provided by the payment service provider and identifies the merchant account.
- **Set Merchant Key:** Assign the merchant key using the SetMerchantKey method. This key is provided by the payment service provider and is used for authentication.
- **Set Environment:** Specify the environment (e.g., STAGING, PRODUCTION) using the SetEnvironment method. This determines the endpoint URL for the payment requests.
- **Set Product ID:** Assign the product ID using the SetProductId method. This ID identifies the specific product or service being paid for.
- **Set API Version:** Specify the API version using the SetApiVersion method. This ensures compatibility with the payment service's API.
- **Assign Credentials to Payment Service:** Finally, assign the configured credentials object to the Credentials property of the paymentService. This step is crucial as it links the payment service instance with the necessary authentication and configuration details, allowing it to authenticate and process payment requests.

```java
Credentials credentials = new();
credentials.SetMerchantId("114658");
credentials.SetEnvironment(EnvironmentEnum.STAGING);
credentials.SetProductId("1146580001");
credentials.SetApiVersion(5);
credentials.SetMerchantKey("3535457e-qe21-40t7-863e-e5838e53499e");
paymentService.Credentials = credentials;
```

## Use Case 1: JavaScript Authentication Request

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Setting Payment Parameters

In this step, we will provide the SDK with the payment parameters:
- Amount
- Currency
- Country
- Customer Id

```csharp
JSAuthorizationRequestParameters jSAuthPaymentParameters = new();
jSAuthPaymentParameters.SetCustomerId("8881");
jSAuthPaymentParameters.SetCurrency(Currency.EUR);
jSAuthPaymentParameters.SetCountry(CountryCodeAlpha2.ES);
jSAuthPaymentParameters.SetOperationType(OperationTypes.DEBIT);
```

### Step 3: Sending the Authentication Request and Retrieve it

The response from the payment service is handled using a custom `JSResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `JSResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### JSResponseListener Class Implementation

```csharp
namespace DotNetPaymentSDK.Integration.Demo.Callbacks
{
    public class JSResponseListener : IJSPaymentListener
    {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURLReceivedAction { get; set; }

        public Action<string, JSAuthorizationResponse> OnAuthorizationResponseReceivedAction { get; set; }

        public void OnError(ErrorsEnum error, string message)
        {
            OnErrorAction?.Invoke(error, message);
        }

        public void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult)
        {
            OnResponseReceivedAction?.Invoke(rawResponse, notification, transactionResult);
        }

        public void OnRedirectionURLReceived(string redirectionURL)
        {
            OnRedirectionURLReceivedAction?.Invoke(redirectionURL);
        }

        public void OnAuthorizationResponseReceived(string rawResponse, JSAuthorizationResponse jsAuthorizationResponse)
        {
            OnAuthorizationResponseReceivedAction?.Invoke(rawResponse, jsAuthorizationResponse);
        }
    }
}
```

#### Code Snippet for Using the JSResponseListener

```csharp
var tcs = new TaskCompletionSource < (string ? , string ? ) > ();

await Task.Run(() => {
  paymentService.SendJSAuthorizationRequest(jSAuthPaymentParameters, new JSResponseListener() {
    OnErrorAction = (ErrorsEnum error, string message) => {
        tcs.SetResult((null, message));
      },
      OnAuthorizationResponseReceivedAction = (string rawResponse, JSAuthorizationResponse jsAuthorizationResponse) => {
        tcs.SetResult((jsAuthorizationResponse.AuthToken, null));
      }
  });
});

var result = await tcs.Task;

if (result.Item1 == null) {
  // On error case
  return StatusCode(400, result.Item2);
} else {
  // return auth token
  return Ok(new {
    auth_token = result.Item1
  });
}
```

## Use Case 2: JavaScript Charge Request

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Setting Payment Parameters for Charge Request

In this step, we will provide the SDK with the payment parameters:
- Amount
- Currency
- Country
- Customer Id
- Merchant Transaction Id
- Payment Solution
- Operation Type
- Set Prepay Token
- StatusURL
- ErrorURL 
- SuccessURL 
- CancelURL 
- AwaitingURL

```csharp
JSChargeParameters jSChargeParameters = new JSChargeParameters();
jSChargeParameters.SetAmount("50");
jSChargeParameters.SetCustomerId("8881");
jSChargeParameters.SetCurrency(Currency.EUR);
jSChargeParameters.SetCountry(CountryCodeAlpha2.ES);
jSChargeParameters.SetOperationType(OperationTypes.DEBIT);
jSChargeParameters.SetPaymentSolution(PaymentSolutions.creditcards);
jSChargeParameters.SetMerchantTransactionId("55555555");
jSChargeParameters.SetPrepayToken(request.PrepayToken);
jSChargeParameters.SetStatusURL("https://test.com/status");
jSChargeParameters.SetSuccessURL("https://test.com/success");
jSChargeParameters.SetErrorURL("https://test.com/error");
jSChargeParameters.SetAwaitingURL("https://test.com/awaiting");
jSChargeParameters.SetCancelURL("https://test.com/cancel");
```

### Step 3: Sending the Charge Request and Retrieve it

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Integration.Demo.Callbacks
{
    public class ResponseListener : IResponseListener
    {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURLReceivedAction { get; set; }

        public void OnError(ErrorsEnum error, string message)
        {
            OnErrorAction?.Invoke(error, message);
        }

        public void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult)
        {
            OnResponseReceivedAction?.Invoke(rawResponse, notification, transactionResult);
        }

        public void OnRedirectionURLReceived(string redirectionURL)
        {
            OnRedirectionURLReceivedAction?.Invoke(redirectionURL);
        }
    }
}
```

#### Code Snippet for Using the JSResponseListener

```csharp
var tcs = new TaskCompletionSource<(string?, string?)>();

await Task.Run(() =>
{
    paymentService.SendJSChargeRequest(jSChargeParameters, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification.Get

RedirectUrl(), null));
        }
    });
});

var result = await tcs.Task;

if (result.Item1 == null)
{
    // On error case
    return StatusCode(400, result.Item2);
}
else
{
    // return redirection url
    return Ok(new { redirect_url = result.Item1 });
}
```


## Use Case 3: JavaScript Recurring Charge Request

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Setting Payment Parameters for Recurring Charge Request

In this step, we will provide the SDK with the payment parameters:
- Amount
- Currency
- Country
- Customer Id
- Merchant Transaction Id
- Payment Solution
- Payment Recurring Type
- Operation Type
- Set Prepay Token
- StatusURL
- ErrorURL 
- SuccessURL 
- CancelURL 
- AwaitingURL

```csharp
            JSPaymentRecurrentInitial jSPaymentRecurrentInitial = new();
            jSPaymentRecurrentInitial.SetAmount(request.amount);
            jSPaymentRecurrentInitial.SetCustomerId("8881");
            jSPaymentRecurrentInitial.SetPaymentRecurringType(PaymentRecurringType.newCof);
            jSPaymentRecurrentInitial.SetChallengeInd(ChallengeIndEnum._04);
            jSPaymentRecurrentInitial.SetCurrency(Currency.EUR);
            jSPaymentRecurrentInitial.SetCountry(CountryCodeAlpha2.ES);
            jSPaymentRecurrentInitial.SetOperationType(OperationTypes.DEBIT);
            jSPaymentRecurrentInitial.SetPaymentSolution(PaymentSolutions.creditcards);
            jSPaymentRecurrentInitial.SetPrepayToken(request.prepayToken);
            jSPaymentRecurrentInitial.SetStatusURL("https://test.com/status");
            jSPaymentRecurrentInitial.SetSuccessURL(configurations["baseURL"] + "https://test.com/success");
            jSPaymentRecurrentInitial.SetErrorURL(configurations["baseURL"] + "https://test.com/error");
            jSPaymentRecurrentInitial.SetAwaitingURL(configurations["baseURL"] + "https://test.com/awaiting");
            jSPaymentRecurrentInitial.SetCancelURL(configurations["baseURL"] + "https://test.com/cancel");
```

### Step 3: Sending the Recurring Charge Request and Retrieve it

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Integration.Demo.Callbacks
{
    public class ResponseListener : IResponseListener
    {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURLReceivedAction { get; set; }

        public void OnError(ErrorsEnum error, string message)
        {
            OnErrorAction?.Invoke(error, message);
        }

        public void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult)
        {
            OnResponseReceivedAction?.Invoke(rawResponse, notification, transactionResult);
        }

        public void OnRedirectionURLReceived(string redirectionURL)
        {
            OnRedirectionURLReceivedAction?.Invoke(redirectionURL);
        }
    }
}
```

#### Code Snippet for Using the JSResponseListener

```csharp
            var tcs = new TaskCompletionSource<(string?, string?)>();

            await Task.Run(() =>
            {
                // GeneralUtils.ToQueryString()
                paymentService.SendJSChargeRequest(jSPaymentRecurrentInitial, new ResponseListener()
                {
                    OnErrorAction = (ErrorsEnum error, string message) =>
                    {
                        tcs.SetResult((null, message));
                    },
                    OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
                    {
                        tcs.SetResult((notification.GetRedirectUrl(), null));
                    },

                });
            });

            var result = await tcs.Task;

            if (result.Item1 == null)
            {
                // On error case
                return StatusCode(400, result.Item2);
            }
            else
            {
                // return redirection url
                return Ok(new { redirect_url = result.Item1 });
            }
```