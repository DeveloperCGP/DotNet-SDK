# Introduction and QuickStart

The DotNetPayment SDK simplifies the integration of payment processing features into your e-commerce projects. It provides a robust and flexible framework for handling payments, supporting a variety of payment methods including hosted payments. This guide will help you get started with the SDK, from setting up your development environment to making your first hosted payment request.

*To run the integration project, use the command `dotnet run`.*

## Table of Contents

1. [Environment](#environment)
2. [Including The SDK](#including-the-sdk)
3. [Usage](#usage)
   - [Creating Credentials Object](#creating-credentials-object)
   - [Setting Payment Parameters](#setting-payment-parameters)
   - [Send Payment Hosted Redirection Request and Retrieve the Response](#send-payment-hosted-redirection-request-and-retrieve-the-response)
   - [Error Handling](#error-handling)
   - [Handle Notification Data](#handle-notification-data)

## Environment

**Requirement:**

- .NET 8.0 or higher

Ensure your development environment is set up with one of the supported versions of .NET. This SDK leverages the latest features of .NET, providing asynchronous operations, strong typing, and integration with .NET's dependency injection systems.

## Including The SDK

To include the SDK, you need to specify the location of the DLL file in the `.csproj` file and include the SDK DLLs:

```xml
<ItemGroup>
  <Reference Include="DotNetPaymentSDK">
    <HintPath>PaymentSdk\DotNetPaymentSDK.dll</HintPath>
  </Reference>
  <Reference Include="DotNetPaymentSDK.Utilities">
    <HintPath>PaymentSdk\DotNetPaymentSDK.Utilities.dll</HintPath>
  </Reference>
  <Reference Include="DotNetPayment.Core.Domain">
    <HintPath>PaymentSdk\DotNetPayment.Core.Domain.dll</HintPath>
  </Reference>
  <Reference Include="DotNetPaymentSDK.Contracts">
    <HintPath>PaymentSdk\DotNetPaymentSDK.Contracts.dll</HintPath>
  </Reference>
</ItemGroup>
```

## Usage

In this section, we explain the generic process of processing transactions. Some details can vary depending on the type of transaction (please refer to the specific transaction type documentation for more information).

### Creating Credentials Object

First, instantiate the `Credentials` object with your merchant details. This includes your Merchant ID and Merchant Pass, which are essential for authenticating requests to the AddonPayments API. 

#### Steps

1. **Initialize Credentials Object:** Create a new instance of the `Credentials` class to hold the authentication and configuration details.
2. **Set Merchant ID:** Assign the merchant ID using the `SetMerchantId` method. This ID is provided by the payment service provider and identifies the merchant account.
3. **Set Merchant Password:** Assign the merchant password using the `SetMerchantPass` method. This password is provided by the payment service provider and is used for authentication.
4. **Set Environment:** Specify the environment (e.g., STAGING, PRODUCTION) using the `SetEnvironment` method. This determines the endpoint URL for the payment requests.
5. **Set Product ID:** Assign the product ID using the `SetProductId` method. This ID identifies the specific product or service being paid for.
6. **Set API Version:** Specify the API version using the `SetApiVersion` method. This ensures compatibility with the payment service's API.
7. **Assign Credentials to Payment Service:** Finally, assign the configured credentials object to the `Credentials` property of the paymentService. This step is crucial as it links the payment service instance with the necessary authentication and configuration details, allowing it to authenticate and process payment requests.

```csharp
Credentials credentials = new();
credentials.SetMerchantId(configurations["merchantId"]);
credentials.SetMerchantPass(configurations["merchantPassword"]);
credentials.SetEnvironment(EnvironmentEnum.STAGING);
credentials.SetProductId(configurations["productId"]);
credentials.SetApiVersion(5);
paymentService.Credentials = credentials;
```

To use environment variables, you can add those values to the appsettings file or to your normal environment variables so you can use `configuration[""]` normally.

```json
{
  "merchantId": "",
  "merchantPassword": "",
  "merchantKey": "",
  "productId": "",
  "environment": "",
  "productIdAccommodation": "",
  "productIdItem": "",
  "productIdService": "",
  "productIdFlight": "",
  "statusUrl": "",
  "successUrl": "",
  "cancelUrl": "",
  "awaitingUrl": "",
  "errorUrl": ""
}
```

### Setting Payment Parameters

To process a hosted payment, you need to specify various transaction details. This is accomplished by creating an instance of `HostedPaymentParameters` and setting its properties.

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
hostedRedirection.SetMerchantParameter("name", "pablo");
hostedRedirection.SetMerchantParameter("surname", "ferre");
            
```

### Send Payment Hosted Redirection Request and Retrieve the Response

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
    return StatusCode(400, result.Item2);
}
else
{
    return Ok(new { redirect_url = result.Item1 });
}
```

### Error Handling

An exception will be thrown in case of an error:

- **MissingParameterException:** In case there is a missing parameter
- **ClientException:** In case the endpoint returned 400
- **ServerException:** In case the endpoint returned 500
- **ParseException:** In case an exception occurred during the parse



### Handle Notification Data

The `Notification` class is responsible for parsing XML or JSON data to extract the required details about a transaction.

#### Web API Controller to Handle HTTP POST Requests

Define an API controller that accepts POST requests and handles the notification string.

1. **Define the API Controller**

```csharp
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace YourNamespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        // POST api/notification
        [HttpPost]
        public IActionResult Post([FromBody] string notificationString)
        {
            if (string.IsNullOrEmpty(notificationString))
            {
                return BadRequest("Notification string is null or empty.");
            }

            try
            {
                // Parse the notification string
                Notification notification = NotificationAdapter.ParseNotification(notificationString);

                // Simple logic: print notification details to the console
                var firstOperation = notification.OperationsArray.First();
                Console.WriteLine($"Notification Received For {firstOperation.Service} with status = {firstOperation.Status}");

                // Return OK (200) status code
                return Ok();
            }
            catch (Exception ex)
            {
                // Handle parsing errors or other exceptions
                return Ok();
            }
        }
    }
}
```

In this controller:
- The `Post` method accepts a `notificationString` from the body of the HTTP POST request.
- It parses the notification string using the `Notification` class, which is designed to handle XML or JSON data.
- It performs simple logic by printing the notification details to the console.
- It returns an HTTP 200 status code (`Ok`) to make that AddonPayments knows your endpoint recived the notification.