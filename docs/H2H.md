# H2H

This section provides a step-by-step guide for implementing Host-to-Host payment transactions using the Java SDK. This method enables direct communication between the merchant's server and the AddonPayments API, offering a more integrated and seamless payment processing experience.

## Table of Contents

1. [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object)
2. [H2H Request](#h2h-request)
   - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite)
   - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object)
   - [Step 3: Send The H2H Request and Retrieve Response](#step-3-send-the-h2h-request-and-retrieve-response)
   - [Complete Example](#complete-example)
3. [Pre-Authorization Request](#pre-authorization-request)
   - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-1)
   - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-1)
   - [Step 3: Send The H2H Request and Retrieve Response](#step-3-send-the-h2h-request-and-retrieve-response-1)
   - [Complete Example](#complete-example-1)
4. [Capture Pre-Authorization](#capture-pre-authorization)
   - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-2)
   - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-2)
   - [Step 3: Send The H2H Capture Request and Retrieve Response](#step-3-send-the-h2h-capture-request-and-retrieve-response)
   - [Complete Example](#complete-example-2)
5. [Void Pre-Authorization](#void-pre-authorization)
   - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-3)
   - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-3)
   - [Step 3: Send The H2H Void Request and Retrieve Response](#step-3-send-the-h2h-void-request-and-retrieve-response)
   - [Complete Example](#complete-example-3)
6. [Recurrent Initial](#recurrent-initial)
   - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-4)
   - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-4)
   - [Step 3: Send The H2H Recurrent Initial Request and Retrieve Response](#step-3-send-the-h2h-recurrent-initial-request-and-retrieve-response)
   - [Complete Example](#complete-example-4)
7. [Recurrent Subsequent](#recurrent-subsequent)
   - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-5)
   - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-5)
   - [Step 3: Send The H2H Recurrent Initial Request and Retrieve Response](#step-3-send-the-h2h-recurrent-initial-request-and-retrieve-response-1)
   - [Complete Example](#complete-example-5)
8. [Refund](#refund)
   - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-6)
   - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-6)
   - [Step 3: Send The H2H Refund Request and Retrieve Response](#step-3-send-the-h2h-refund-request-and-retrieve-response)
   - [Complete Example](#complete-example-6)

## Common Prerequisite: Creating Credentials Object

First, instantiate the `Credentials` object with your merchant details. This includes your Merchant ID and Merchant Pass, which are essential for authenticating requests to the AddonPayments API. 

### Steps

1. **Initialize Credentials Object:** Create a new instance of the `Credentials` class to hold the authentication and configuration details.
2. **Set Merchant ID:** Assign the merchant ID using the `SetMerchantId` method. This ID is provided by the payment service provider and identifies the merchant account.
3. **Set Merchant Password:** Assign the merchant password using the `SetMerchantPass` method. This password is provided by the payment service provider and is used for authentication.
4. **Set Environment:** Specify the environment (e.g., STAGING, PRODUCTION) using the `SetEnvironment` method. This determines the endpoint URL for the payment requests.
5. **Set Product ID:** Assign the product ID using the `SetProductId` method. This ID identifies the specific product or service being paid for.
6. **Set API Version:** Specify the API version using the `SetApiVersion` method. This ensures compatibility with the payment service's API.
7. **Assign Credentials to Payment Service:** Finally, assign the configured credentials object to the `Credentials` property of the paymentService. This step is crucial as it links the payment service instance with the necessary authentication and configuration details, allowing it to authenticate and process payment requests.

```java
Credentials credentials = new();
credentials.SetMerchantId(configurations["merchantId"]);
credentials.SetMerchantPass(configurations["merchantPassword"]);
credentials.SetEnvironment(EnvironmentEnum.STAGING);
credentials.SetProductId(configurations["productId"]);
credentials.SetApiVersion(5);
paymentService.Credentials = credentials;
```

## H2H Request

Sending a normal payment H2H request which is used in a normal payment.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

- Amount
- Currency
- Country
- Customer Id
- Merchant Transaction Id
- Payment Solution
- ChName
- CardNumber
- ExpDate
- CvnNumber
- StatusURL
- ErrorURL
- SuccessURL
- CancelURL
- AwaitingURL

```csharp
H2HRedirectionParameters h2HRedirection = new();
h2HRedirection.SetAmount("50");
h2HRedirection.SetCurrency(Currency.EUR);
h2HRedirection.SetCountry(CountryCodeAlpha2.ES);
h2HRedirection.SetCustomerId("903");
h2HRedirection.SetCardNumber("4907270002222227");
h2HRedirection.SetMerchantTransactionId("4556115");
h2HRedirection.SetChName("Pablo");
h2HRedirection.SetCvnNumber("123");
h2HRedirection.SetExpDate("0230");
h2HRedirection.SetPaymentSolution(PaymentSolutions.creditcards);
h2HRedirection.SetStatusURL("https://test.com/status");
h2HRedirection.SetSuccessURL("https://test.com/success");
h2HRedirection.SetErrorURL("https://test.com/error");
h2HRedirection.SetAwaitingURL("https://test.com/awaiting");
h2HRedirection.SetCancelURL("https://test.com/cancel");
h2HRedirection.SetForceTokenRequest(false);
List<Tuple<string, string>> merchantParams = [];
merchantParams.Add(new("name", "pablo"));
merchantParams.Add(new("surname", "ferre"));
h2HRedirection.SetMerchantParameters(merchantParams);
```

### Step 3: Send The H2H Request and Retrieve Response

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

#### Code Snippet for Using the ResponseListener

```csharp
var tcs = new TaskCompletionSource<(string?, string?)>();

await Task.Run(() =>
{
    paymentService.SendH2HRedirectionPaymentRequest(h2HRedirection, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((

notification.GetRedirectUrl(), null));
        }
    });
});

var result = await tcs.Task;

if (result.Item1 == null)
{
    Console.Write($"TEST RESULT: {result.Item2}");
    return StatusCode(400, result.Item2);
}
else
{
    return Ok(new { redirect_url = result.Item1 });
}
```

### Complete Example

Here is the complete example combining all the sections, including the `ResponseListener` class implementation directly within the same file for ease of understanding:

```csharp
public class H2HController : ControllerBase
{
    private readonly H2HService paymentService;

    public H2HController(H2HService paymentService)
    {
        this.paymentService = paymentService;
    }

    [HttpPost("h2h/payment")]
    public async Task<IActionResult> SendH2HPaymentRequest()
    {
        Credentials credentials = new();
        credentials.SetMerchantId(configurations["merchantId"]);
        credentials.SetMerchantPass(configurations["merchantPassword"]);
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId(configurations["productId"]);
        credentials.SetApiVersion(5);
        paymentService.Credentials = credentials;

        H2HRedirectionParameters h2HRedirection = new();
        h2HRedirection.SetAmount("50");
        h2HRedirection.SetCurrency(Currency.EUR);
        h2HRedirection.SetCountry(CountryCodeAlpha2.ES);
        h2HRedirection.SetCustomerId("903");
        h2HRedirection.SetCardNumber("4907270002222227");
        h2HRedirection.SetMerchantTransactionId("4556115");
        h2HRedirection.SetChName("Pablo");
        h2HRedirection.SetCvnNumber("123");
        h2HRedirection.SetExpDate("0230");
        h2HRedirection.SetPaymentSolution(PaymentSolutions.creditcards);
        h2HRedirection.SetStatusURL("https://test.com/status");
        h2HRedirection.SetSuccessURL("https://test.com/success");
        h2HRedirection.SetErrorURL("https://test.com/error");
        h2HRedirection.SetAwaitingURL("https://test.com/awaiting");
        h2HRedirection.SetCancelURL("https://test.com/cancel");
        h2HRedirection.SetForceTokenRequest(false);

        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendH2HRedirectionPaymentRequest(h2HRedirection, new ResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
                {
                    tcs.SetResult((notification.GetRedirectUrl(), null));
                }
            });
        });

        var result = await tcs.Task;

        if (result.Item1 == null)
        {
            // On error case
            Console.Write($"TEST RESUALT: {result.Item2}");
            return StatusCode(400, result.Item2);
        }
        else
        {
            // return redirection url
            return Ok(new { redirect_url = result.Item1 });
        }
    }
}

// ResponseListener Class Implementation
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

Note: The status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Pre-Authorization Request

Sending a normal Pre-Authorization H2H request.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

- Amount
- Currency
- Country
- Customer Id
- Merchant Transaction Id
- Payment Solution
- ChName
- CardNumber
- ExpDate
- CvnNumber
- StatusURL
- ErrorURL
- SuccessURL
- CancelURL
- AwaitingURL

```csharp
H2HPreAuthorizationParameters h2hPaymentAuthorizationParameters = new();
h2hPaymentAuthorizationParameters.SetAmount("50");
h2hPaymentAuthorizationParameters.SetCurrency(Currency.EUR);
h2hPaymentAuthorizationParameters.SetCountry(CountryCodeAlpha2.ES);
h2hPaymentAuthorizationParameters.SetCardNumber("4907270002222227");
h2hPaymentAuthorizationParameters.SetMerchantTransactionId("4556115");
h2hPaymentAuthorizationParameters.SetCustomerId("903");
h2hPaymentAuthorizationParameters.SetChName("Pablo");
h2hPaymentAuthorizationParameters.SetCvnNumber("123");
h2hPaymentAuthorizationParameters.SetExpDate("0230");
h2hPaymentAuthorizationParameters.SetStatusURL("https://test.com/status");
h2hPaymentAuthorizationParameters.SetSuccessURL("https://test.com/success");
h2hPaymentAuthorizationParameters.SetErrorURL("https://test.com/error");
h2hPaymentAuthorizationParameters.SetAwaitingURL("https://test.com/awaiting");
h2hPaymentAuthorizationParameters.SetCancelURL("https://test.com/cancel");
```

### Step 3: Send The H2H Request and Retrieve Response

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Integration.Demo.Callbacks {
    public class ResponseListener : IResponseListener {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURLReceivedAction { get; set; }

        public void OnError(ErrorsEnum error, string message) {
            OnErrorAction?.Invoke(error, message);
        }

        public void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult) {
            OnResponseReceivedAction?.Invoke(rawResponse, notification, transactionResult);
        }

        public void OnRedirectionURLReceived(string redirectionURL) {
            OnRedirectionURLReceivedAction?.Invoke(redirectionURL);
        }
    }
}
```

#### Code Snippet for Using the ResponseListener

```csharp
var tcs = new TaskCompletionSource<(string?, string?)>();

await Task.Run(() =>
{
    paymentService.SendH2hPreAuthorizationRequest(h2hPaymentAuthorizationParameters, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification.GetRedirectUrl(), null));
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

### Complete Example

Here is the complete example combining all the sections, including the `ResponseListener` class implementation directly within the same file for ease of understanding:

```csharp
public class H2HController : ControllerBase
{
    private readonly H2HService paymentService;

    public H2HController(H2HService paymentService)
    {
        this.paymentService = paymentService;
    }

    [HttpPost("authorization/payment")]
    public async Task<IActionResult> SendH2HPaymentAuthorizationRequest()
    {
        Credentials credentials = new();
        credentials.SetMerchantId(configurations["merchantId"]);
        credentials.SetMerchantPass(configurations["merchantPassword"]);
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId(configurations["productId"]);
        credentials.SetApiVersion(5);
        paymentService.Credentials = credentials;

        H2HPreAuthorizationParameters h2hPaymentAuthorizationParameters = new();
        h2hPaymentAuthorizationParameters.SetAmount("50");
        h2hPaymentAuthorizationParameters.SetCurrency(Currency.EUR);
        h2

hPaymentAuthorizationParameters.SetCountry(CountryCodeAlpha2.ES);
        h2hPaymentAuthorizationParameters.SetCardNumber("4907270002222227");
        h2hPaymentAuthorizationParameters.SetMerchantTransactionId("4556115");
        h2hPaymentAuthorizationParameters.SetCustomerId("903");
        h2hPaymentAuthorizationParameters.SetChName("Pablo");
        h2hPaymentAuthorizationParameters.SetCvnNumber("123");
        h2hPaymentAuthorizationParameters.SetExpDate("0230");
        h2hPaymentAuthorizationParameters.SetStatusURL("https://test.com/status");
        h2hPaymentAuthorizationParameters.SetSuccessURL("https://test.com/success");
        h2hPaymentAuthorizationParameters.SetErrorURL("https://test.com/error");
        h2hPaymentAuthorizationParameters.SetAwaitingURL("https://test.com/awaiting");
        h2hPaymentAuthorizationParameters.SetCancelURL("https://test.com/cancel");

        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendH2hPreAuthorizationRequest(h2hPaymentAuthorizationParameters, new ResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
                {
                    tcs.SetResult((notification.GetRedirectUrl(), null));
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
    }
}

// ResponseListener Class Implementation
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

Note: The status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Capture Pre-Authorization

Sending a normal Pre-Authorization H2H request.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

- Merchant Transaction Id
- Payment Solution
- Transaction Id

```csharp
H2HPreAuthorizationCaptureParameters h2hPreAuthorizationCaptureParameters = new();
h2hPreAuthorizationCaptureParameters.SetMerchantTransactionId("334198711");
h2hPreAuthorizationCaptureParameters.SetPaymentSolution(PaymentSolutions.caixapucpuce);
h2hPreAuthorizationCaptureParameters.SetTransactionId("31399103");
```

### Step 3: Send The H2H Capture Request and Retrieve Response

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Integration.Demo.Callbacks {
    public class ResponseListener : IResponseListener {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURLReceivedAction { get; set; }

        public void OnError(ErrorsEnum error, string message) {
            OnErrorAction?.Invoke(error, message);
        }

        public void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult) {
            OnResponseReceivedAction?.Invoke(rawResponse, notification, transactionResult);
        }

        public void OnRedirectionURLReceived(string redirectionURL) {
            OnRedirectionURLReceivedAction?.Invoke(redirectionURL);
        }
    }
}
```

#### Code Snippet for Using the ResponseListener

```csharp
var tcs = new TaskCompletionSource<(string?, string?)>();

await Task.Run(() =>
{
    paymentService.SendH2hPreAuthorizationCapture(h2hPreAuthorizationCaptureParameters, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification.Status, null));
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
    return Ok(new { status = result.Item1 });
}
```

### Complete Example

Here is the complete example combining all the sections, including the `ResponseListener` class implementation directly within the same file for ease of understanding:

```csharp
public class H2HController : ControllerBase
{
    private readonly H2HService paymentService;

    public H2HController(H2HService paymentService)
    {
        this.paymentService = paymentService;
    }

    [HttpPost("capture/payment")]
    public async Task<IActionResult> SendH2HCapture([FromBody] H2HCaptureDto request)
    {
        Credentials credentials = new();
        credentials.SetMerchantId(configurations["merchantId"]);
        credentials.SetMerchantPass(configurations["merchantPassword"]);
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId(configurations["productId"]);
        credentials.SetApiVersion(5);
        paymentService.Credentials = credentials;

        H2HPreAuthorizationCaptureParameters h2hPreAuthorizationCaptureParameters = new();
        h2hPreAuthorizationCaptureParameters.SetMerchantTransactionId("334198711");
        h2hPreAuthorizationCaptureParameters.SetPaymentSolution(PaymentSolutions.caixapucpuce);
        h2hPreAuthorizationCaptureParameters.SetTransactionId("31399103");

        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendH2hPreAuthorizationCapture(h2hPreAuthorizationCaptureParameters, new ResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
                {
                    tcs.SetResult((notification.Status, null));
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
            return Ok(new { status = result.Item1 });
        }
    }
}

// ResponseListener Class Implementation
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

Note: The status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Void Pre-Authorization

Sending a normal Void Pre-Authorization H2H request.

### Step 1:

 Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

- Merchant Transaction Id
- Payment Solution
- Transaction Id

```csharp
H2HVoidParameters h2hVoidParameters = new();
h2hVoidParameters.SetMerchantTransactionId("34455122231");
h2hVoidParameters.SetPaymentSolution(PaymentSolutions.creditcards);
h2hVoidParameters.SetTransactionId("3445512221");
```

### Step 3: Send The H2H Void Request and Retrieve Response

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Integration.Demo.Callbacks {
    public class ResponseListener : IResponseListener {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURLReceivedAction { get; set; }

        public void OnError(ErrorsEnum error, string message) {
            OnErrorAction?.Invoke(error, message);
        }

        public void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult) {
            OnResponseReceivedAction?.Invoke(rawResponse, notification, transactionResult);
        }

        public void OnRedirectionURLReceived(string redirectionURL) {
            OnRedirectionURLReceivedAction?.Invoke(redirectionURL);
        }
    }
}
```

#### Code Snippet for Using the ResponseListener

```csharp
var tcs = new TaskCompletionSource<(string?, string?)>();

await Task.Run(() =>
{
    paymentService.SendH2hVoidRequest(h2hVoidParameters, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification.Status, null));
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

### Complete Example

Here is the complete example combining all the sections, including the `ResponseListener` class implementation directly within the same file for ease of understanding:

```csharp
public class H2HController : ControllerBase
{
    private readonly H2HService paymentService;

    public H2HController(H2HService paymentService)
    {
        this.paymentService = paymentService;
    }

    [HttpPost("void/payment")]
    public async Task<IActionResult> SendH2HVoid()
    {
        // Validate and process the request

        Credentials credentials = new();
        credentials.SetMerchantId(configurations["merchantId"]);
        credentials.SetMerchantPass(configurations["merchantPassword"]);
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId(configurations["productId"]);
        credentials.SetApiVersion(5);
        paymentService.Credentials = credentials;

        H2HVoidParameters h2hVoidParameters = new();
        h2hVoidParameters.SetMerchantTransactionId("34455122231");
        h2hVoidParameters.SetPaymentSolution(PaymentSolutions.creditcards);
        h2hVoidParameters.SetTransactionId("3445512221");

        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendH2hVoidRequest(h2hVoidParameters, new ResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
                {
                    tcs.SetResult((notification.Status, null));
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
            return Ok(new { status = result.Item1 });
        }
    }
}

// ResponseListener Class Implementation
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

Note: The status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Recurrent Initial

Sending a Recurrent Initial H2H request.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

- Amount
- Currency
- Country
- Customer Id
- Merchant Transaction Id
- Payment Solution
- Payment Recurring Type
- ChName
- CardNumber
- ExpDate
- CvnNumber
- StatusURL
- ErrorURL
- SuccessURL
- CancelURL
- AwaitingURL

```csharp
H2HPaymentRecurrentInitialParameters h2hPaymentRecurrentInitialParameters = new();
h2hPaymentRecurrentInitialParameters.SetAmount("50");
h2hPaymentRecurrentInitialParameters.SetCurrency(Currency.EUR);
h2hPaymentRecurrentInitialParameters.SetCountry(CountryCodeAlpha2.ES);
h2hPaymentRecurrentInitialParameters.SetCustomerId("903");
h2hPaymentRecurrentInitialParameters.SetCardNumber("4907270002222227");
h2hPaymentRecurrentInitialParameters.SetMerchantTransactionId("64884555");
h2hPaymentRecurrentInitialParameters.SetChName("Pablo");
h2hPaymentRecurrentInitialParameters.SetCvnNumber("123");
h2hPaymentRecurrentInitialParameters.SetExpDate("0230");
h2hPaymentRecurrentInitialParameters.SetPaymentRecurringType(PaymentRecurringType.newCof);
h2hPaymentRecurrentInitialParameters.SetPaymentSolution(PaymentSolutions.creditcards);
h2hPaymentRecurrentInitialParameters.SetStatusURL("https://test.com/status");
h2hPaymentRecurrentInitialParameters.SetSuccessURL("https://test.com/success");
h2hPaymentRecurrentInitialParameters.SetErrorURL("https://test.com/error");
h2hPaymentRecurrentInitialParameters.SetAwaitingURL("https://test.com/awaiting");
h2hPaymentRecurrentInitialParameters.SetCancelURL("https://test.com/cancel");
h2hPaymentRecurrentInitialParameters.SetForceTokenRequest(false);
```

### Step 3: Send The H2H Recurrent Initial Request and Retrieve Response

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Integration.Demo.Callbacks {
    public class ResponseListener : IResponseListener {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURLReceivedAction { get; set; }

        public void OnError(ErrorsEnum error, string message) {
            OnErrorAction?.Invoke(error, message);
        }

        public void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult) {
            OnResponseReceivedAction?.Invoke(rawResponse, notification,

 transactionResult);
        }

        public void OnRedirectionURLReceived(string redirectionURL) {
            OnRedirectionURLReceivedAction?.Invoke(redirectionURL);
        }
    }
}
```

#### Code Snippet for Using the ResponseListener

```csharp
var tcs = new TaskCompletionSource<(string?, string?)>();

await Task.Run(() =>
{
    paymentService.SendH2hPaymentRecurrentInitial(h2hPaymentRecurrentInitialParameters, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification.GetRedirectUrl(), null));
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

### Complete Example

Here is the complete example combining all the sections, including the `ResponseListener` class implementation directly within the same file for ease of understanding:

```csharp
public class H2HController : ControllerBase
{
    private readonly H2HService paymentService;

    public H2HController(H2HService paymentService)
    {
        this.paymentService = paymentService;
    }

    [HttpPost("recurring/payment")]
    public async Task<IActionResult> SendH2HPaymentRecurringRequest()
    {
        Credentials credentials = new();
        credentials.SetMerchantId(configurations["merchantId"]);
        credentials.SetMerchantPass(configurations["merchantPassword"]);
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId(configurations["productId"]);
        credentials.SetApiVersion(5);
        paymentService.Credentials = credentials;

        H2HPaymentRecurrentInitialParameters h2hPaymentRecurrentInitialParameters = new();
        h2hPaymentRecurrentInitialParameters.SetAmount("50");
        h2hPaymentRecurrentInitialParameters.SetCurrency(Currency.EUR);
        h2hPaymentRecurrentInitialParameters.SetCountry(CountryCodeAlpha2.ES);
        h2hPaymentRecurrentInitialParameters.SetCustomerId("903");
        h2hPaymentRecurrentInitialParameters.SetCardNumber("4907270002222227");
        h2hPaymentRecurrentInitialParameters.SetMerchantTransactionId("64884555");
        h2hPaymentRecurrentInitialParameters.SetChName("Pablo");
        h2hPaymentRecurrentInitialParameters.SetCvnNumber("123");
        h2hPaymentRecurrentInitialParameters.SetExpDate("0230");
        h2hPaymentRecurrentInitialParameters.SetPaymentRecurringType(PaymentRecurringType.newCof);
        h2hPaymentRecurrentInitialParameters.SetPaymentSolution(PaymentSolutions.creditcards);
        h2hPaymentRecurrentInitialParameters.SetStatusURL("https://test.com/status");
        h2hPaymentRecurrentInitialParameters.SetSuccessURL("https://test.com/success");
        h2hPaymentRecurrentInitialParameters.SetErrorURL("https://test.com/error");
        h2hPaymentRecurrentInitialParameters.SetAwaitingURL("https://test.com/awaiting");
        h2hPaymentRecurrentInitialParameters.SetCancelURL("https://test.com/cancel");
        h2hPaymentRecurrentInitialParameters.SetForceTokenRequest(false);

        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendH2hPaymentRecurrentInitial(h2hPaymentRecurrentInitialParameters, new ResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
                {
                    tcs.SetResult((notification.GetRedirectUrl(), null));
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
    }
}

// ResponseListener Class Implementation
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

Note: The status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Recurrent Subsequent

Sending a Recurrent Subsequent H2H request.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

- Subscription Plan
- Payment Recurring Type
- Merchant Exemptions Sca
- Customer Id
- Merchant Transaction Id
- Payment Solution
- ChName
- Card Number Token

```csharp
H2HPaymentRecurrentSuccessiveParameters h2hPaymentRecurrentSuccessiveParameters = new();
h2hPaymentRecurrentSuccessiveParameters.SetSubscriptionPlan("613317123312");
h2hPaymentRecurrentSuccessiveParameters.SetPaymentRecurringType(PaymentRecurringType.cof);
h2hPaymentRecurrentSuccessiveParameters.SetMerchantExemptionsSca(MerchantExemptionsScaEnum.MIT);
h2hPaymentRecurrentSuccessiveParameters.SetCardNumberToken("51331223312");
h2hPaymentRecurrentSuccessiveParameters.SetCustomerId("903");
h2hPaymentRecurrentSuccessiveParameters.SetChName("First name Last name");
h2hPaymentRecurrentSuccessiveParameters.SetPaymentSolution(PaymentSolutions.creditcards);
```

### Step 3: Send The H2H Recurrent Initial Request and Retrieve Response

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Integration.Demo.Callbacks {
    public class ResponseListener : IResponseListener {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURLReceivedAction { get; set; }

        public void OnError(ErrorsEnum error, string message) {
            OnErrorAction?.Invoke(error, message);
        }

        public void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult) {
            OnResponseReceivedAction?.Invoke(rawResponse, notification, transactionResult);
        }

        public void OnRedirectionURLReceived(string redirectionURL) {
            OnRedirectionURLReceivedAction?.Invoke(redirectionURL);
        }
    }
}
```

#### Code Snippet for Using the ResponseListener

```csharp
var tcs = new TaskCompletionSource<(string?, string?)>();

await Task.Run(() =>
{
    paymentService.SendH2hPaymentRecurrentInitial(h2hPaymentRecurrentInitialParameters, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification.GetRedirectUrl(), null));
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

### Complete Example

Here is the complete example combining all the sections, including the `ResponseListener` class implementation directly within the same file for ease of understanding:

```csharp
public class H2HController : ControllerBase
{
    private readonly H2HService paymentService;

    public H2HController(H2HService paymentService)
    {
        this.paymentService = payment

Service;
    }

    [HttpPost("recurringSubsequent/payment")]
    public async Task<IActionResult> SendH2HPaymentRecurringSubsuqentRequest()
    {
        Console.Write($"Received Payment Request: {request}");

        // Validate and process the request

        Credentials credentials = new();
        credentials.SetMerchantId(configurations["merchantId"]);
        credentials.SetMerchantPass(configurations["merchantPassword"]);
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId(configurations["productId"]);
        credentials.SetApiVersion(5);
        paymentService.Credentials = credentials;

        H2HPaymentRecurrentSuccessiveParameters h2hPaymentRecurrentSuccessiveParameters = new();
        h2hPaymentRecurrentSuccessiveParameters.SetSubscriptionPlan("613317123312");
        h2hPaymentRecurrentSuccessiveParameters.SetPaymentRecurringType(PaymentRecurringType.cof);
        h2hPaymentRecurrentSuccessiveParameters.SetMerchantExemptionsSca(MerchantExemptionsScaEnum.MIT);
        h2hPaymentRecurrentSuccessiveParameters.SetCardNumberToken("51331223312");
        h2hPaymentRecurrentSuccessiveParameters.SetCustomerId("903");
        h2hPaymentRecurrentSuccessiveParameters.SetChName("First name Last name");
        h2hPaymentRecurrentSuccessiveParameters.SetPaymentSolution(PaymentSolutions.creditcards);

        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendH2hPaymentRecurrentSuccessive(h2hPaymentRecurrentSuccessiveParameters, new ResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
                {
                    tcs.SetResult((notification.Status, null));
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
            return Ok(new { status = result.Item1 });
        }
    }
}

// ResponseListener Class Implementation
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

Note: The status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Refund

Sending a normal Refund H2H request.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

- Amount
- Merchant Transaction Id
- Payment Solution
- Transaction Id

```csharp
H2HRefundParameters h2hRefundParameters = new();
h2hRefundParameters.SetAmount("10");
h2hRefundParameters.SetMerchantTransactionId("4144412231");
h2hRefundParameters.SetPaymentSolution(PaymentSolutions.creditcards);
h2hRefundParameters.SetTransactionId("45465466");
```

### Step 3: Send The H2H Refund Request and Retrieve Response

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Integration.Demo.Callbacks {
    public class ResponseListener : IResponseListener {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURLReceivedAction { get; set; }

        public void OnError(ErrorsEnum error, string message) {
            OnErrorAction?.Invoke(error, message);
        }

        public void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult) {
            OnResponseReceivedAction?.Invoke(rawResponse, notification, transactionResult);
        }

        public void OnRedirectionURLReceived(string redirectionURL) {
            OnRedirectionURLReceivedAction?.Invoke(redirectionURL);
        }
    }
}
```

#### Code Snippet for Using the ResponseListener

```csharp
var tcs = new TaskCompletionSource<(string?, string?)>();

await Task.Run(() =>
{
    paymentService.SendH2hRefundRequest(h2hRefundParameters, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification.Status, null));
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

### Complete Example

Here is the complete example combining all the sections, including the `ResponseListener` class implementation directly within the same file for ease of understanding:

```csharp
public class H2HController : ControllerBase
{
    private readonly H2HService paymentService;

    public H2HController(H2HService paymentService)
    {
        this.paymentService = paymentService;
    }

    [HttpPost("refund/payment")]
    public async Task<IActionResult> SendH2HRefund([FromBody] H2HCaptureDto request)
    {
        Console.Write($"Received Payment Request: {request}");

        // Validate and process the request

        Credentials credentials = new();
        credentials.SetMerchantId(configurations["merchantId"]);
        credentials.SetMerchantPass(configurations["merchantPassword"]);
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId(configurations["productId"]);
        credentials.SetApiVersion(5);
        paymentService.Credentials = credentials;

        H2HRefundParameters h2hRefundParameters = new();
        h2hRefundParameters.SetAmount("10");
        h2hRefundParameters.SetMerchantTransactionId("4144412231");
        h2hRefundParameters.SetPaymentSolution(PaymentSolutions.creditcards);
        h2hRefundParameters.SetTransactionId("45465466");

        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendH2hRefundRequest(h2hRefundParameters, new ResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
                {
                    tcs.SetResult((notification.Status, null));
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
            return Ok(new { status = result.Item1 });
        }
    }
}

// ResponseListener Class Implementation
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
            OnRedirectionURLReceivedAction?.Invoke(red

irectionURL);
        }
    }
}
```

Note: The status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.