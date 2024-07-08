# Hosted

## Table of Contents
- [Introduction](#introduction)
- [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object)
  - [Steps](#steps)
- [Hosted Request](#hosted-request)
  - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite)
  - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object)
  - [Step 3: Send The Hosted Request and Retrieve Response](#step-3-send-the-hosted-request-and-retrieve-response)
- [Hosted Recurring](#hosted-recurring)
  - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-1)
  - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-1)
  - [Step 3: Send The Hosted Recurring Request and Retrieve Response](#step-3-send-the-hosted-recurring-request-and-retrieve-response)
- [Complete Example](#complete-example)

## Introduction

This documentation focuses on how to make Hosted transactions using the SDK. This payment method sends the payment details and then shows a web page directed from Addon Payments for the user to enter the card data and proceed with the transaction.

## Common Prerequisite: Creating Credentials Object

First, instantiate the Credentials object with your merchant details. This includes your Merchant ID, Merchant Pass which are essential for authenticating requests to the AddonPayments API. In this section, we set up the necessary credentials for the payment service. The credentials include the merchant ID, merchant password, environment, product ID, and API version.

### Steps

- **Initialize Credentials Object:** Create a new instance of the Credentials class to hold the authentication and configuration details.
- **Set Merchant ID:** Assign the merchant ID using the SetMerchantId method. This ID is provided by the payment service provider and identifies the merchant account.
- **Set Merchant Password:** Assign the merchant password using the SetMerchantPass method. This password is provided by the payment service provider and is used for authentication.
- **Set Environment:** Specify the environment (e.g., STAGING, PRODUCTION) using the SetEnvironment method. This determines the endpoint URL for the payment requests.
- **Set Product ID:** Assign the product ID using the SetProductId method. This ID identifies the specific product or service being paid for.
- **Set API Version:** Specify the API version using the SetApiVersion method. This ensures compatibility with the payment service's API.
- **Assign Credentials to Payment Service:** Finally, assign the configured credentials object to the Credentials property of the paymentService. This step is crucial as it links the payment service instance with the necessary authentication and configuration details, allowing it to authenticate and process payment requests.

```java
Credentials credentials = new();
credentials.SetMerchantId(configurations["merchantId"]);
credentials.SetMerchantPass(configurations["merchantPassword"]);
credentials.SetEnvironment(EnvironmentEnum.STAGING);
credentials.SetProductId(configurations["productId"]);
credentials.SetApiVersion(5);
paymentService.Credentials = credentials;
```

## Hosted Request

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
- StatusURL
- ErrorURL 
- SuccessURL 
- CancelURL 
- AwaitingURL

```csharp
HostedPaymentRedirection hostedRedirection = new();
hostedRedirection.SetAmount("50");
hostedRedirection.SetCurrency(Currency.EUR);
hostedRedirection.SetCountry(CountryCodeAlpha2.ES);
hostedRedirection.SetCustomerId("903");
hostedRedirection.SetMerchantTransactionId("3123123");
hostedRedirection.SetPaymentSolution(PaymentSolutions.creditcards);
hostedRedirection.SetStatusURL("https://test.com/status");
hostedRedirection.SetSuccessURL("https://test.com/success");
hostedRedirection.SetErrorURL("https://test.com/error");
hostedRedirection.SetAwaitingURL("https://test.com/awaiting");
hostedRedirection.SetCancelURL("https://test.com/cancel");
hostedRedirection.SetForceTokenRequest(false);
```

### Step 3: Send The Hosted Request and Retrieve Response

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Nottification;

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
    paymentService.SendHostedPaymentRequest(hostedRedirection, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnRedirectionURLReceivedAction = (redirectionURL) =>
        {
            tcs.SetResult((redirectionURL, null));
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

Note: It's important to note that the status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Hosted Recurring

Sending a normal payment H2H request which is used in a normal payment.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

- Amount
- Currency
- Country
- Customer Id
- Payment Recurring Type
- Challenge Ind
- Merchant Transaction Id
- Payment Solution
- StatusURL
- ErrorURL 
- SuccessURL 
- CancelURL 
- AwaitingURL

```csharp
HostedPaymentRecurrentInitial hostedRecurring = new();
hostedRecurring.SetAmount("50");
hostedRecurring.SetCurrency(Currency.EUR);
hostedRecurring.SetCountry(CountryCodeAlpha2.ES);
hostedRecurring.SetCustomerId("903");
hostedRecurring.SetPaymentRecurringType(PaymentRecurringType.newCof);
hostedRecurring.SetChallengeInd(ChallengeIndEnum._04);
hostedRecurring.SetMerchantTransactionId("3123123");
hostedRecurring.SetPaymentSolution(PaymentSolutions.creditcards);
hostedRecurring.SetStatusURL("https://test.com/status");
hostedRecurring.SetSuccessURL("https://test.com/success");
hostedRecurring.SetErrorURL("https://test.com/error");
hostedRecurring.SetAwaitingURL("https://test.com/awaiting");
hostedRecurring.SetCancelURL("https://test.com/cancel");
hostedRecurring.SetForceTokenRequest(false);
```

### Step 3: Send The Hosted Recurring Request and Retrieve Response

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Nottification;

namespace DotNetPaymentSDK.Integration.Demo.Callbacks
{
    public class ResponseListener : IResponseListener
    {
        public Action<ErrorsEnum, string> OnErrorAction { get; set; }
        public Action<string, Notification, TransactionResult> OnResponseReceivedAction { get; set; }
        public Action<string> OnRedirectionURL

ReceivedAction { get; set; }

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
    paymentService.SendHostedRecurrentInitial(hostedRecurring, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnRedirectionURLReceivedAction = (redirectionURL) =>
        {
            tcs.SetResult((redirectionURL, null));
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

Note: It's important to note that the status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Complete Example

Here is the complete example combining all the sections, including the `ResponseListener` class implementation directly within the same file for ease of understanding:

```csharp
public class HostedController : ControllerBase
{
    private readonly HostedService paymentService;

    public HostedController(HostedService paymentService)
    {
        this.paymentService = paymentService;
    }

    [HttpPost("hosted/payment")]
    public async Task<IActionResult> SendHostedPaymentRequest()
    {
        Credentials credentials = new();
        credentials.SetMerchantId(configurations["merchantId"]);
        credentials.SetMerchantPass(configurations["merchantPassword"]);
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId(configurations["productId"]);
        credentials.SetApiVersion(5);
        paymentService.Credentials = credentials;

        HostedPaymentRedirection hostedRedirection = new();
        hostedRedirection.SetAmount("50");
        hostedRedirection.SetCurrency(Currency.EUR);
        hostedRedirection.SetCountry(CountryCodeAlpha2.ES);
        hostedRedirection.SetCustomerId("903");
        hostedRedirection.SetMerchantTransactionId("3123123");
        hostedRedirection.SetPaymentSolution(PaymentSolutions.creditcards);
        hostedRedirection.SetStatusURL("https://test.com/status");
        hostedRedirection.SetSuccessURL("https://test.com/success");
        hostedRedirection.SetErrorURL("https://test.com/error");
        hostedRedirection.SetAwaitingURL("https://test.com/awaiting");
        hostedRedirection.SetCancelURL("https://test.com/cancel");
        hostedRedirection.SetForceTokenRequest(false);

        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendHostedPaymentRequest(hostedRedirection, new ResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnRedirectionURLReceivedAction = (redirectionURL) =>
                {
                    tcs.SetResult((redirectionURL, null));
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
    }
}

// ResponseListener Class Implementation
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Parameters.Nottification;

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

Note: It's important to note that the status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.