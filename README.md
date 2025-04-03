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

try
{
    await paymentService.SendHostedPaymentRequest(hostedRedirection, new ResponseListener()
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
}
catch (Exception ex)
{
    // Handle exception
    Console.WriteLine($"An error occurred: {ex.Message}");
    tcs.SetResult((null, ex.Message));
}

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

The `Notification` class is responsible for parsing XML or JSON data to extract the required details about a transaction. This includes determining the final result of the payment transaction, such as whether it was approved, declined, or requires additional processing.

#### Import Necessary Namespaces

To begin, you need to include the necessary namespaces:

```csharp
using DotNetPaymentSDK.src.Parameters.Notification;
using DotNetPayment.Core.Domain.Enums;
```

#### Initialize the Notification Class

Parse the notification string using the `NotificationAdapter` class:

```csharp
string notificationString = /* Your XML or JSON data here */;
Notification notification = NotificationAdapter.ParseNotification(notificationString);
```

#### Retrieve Transaction Details

Once the notification string has been parsed, you can access various transaction details:

```csharp
// Get transaction ID
string transactionId = notification.Operations.OperationList.Last().TransactionId;

// Get merchant transaction ID
string merchantTransactionId = notification.Operations.OperationList.Last().MerchantTransactionId;

// Get payment status
string status = notification.Operations.OperationList.Last().Status;

// Get payment amount
decimal amount = notification.Operations.OperationList.Last().Amount;

// Get currency code
string currency = notification.Operations.OperationList.Last().Currency;

// Get payment solution
PaymentSolutions paymentSolution = notification.Operations.OperationList.Last().PaymentSolution;

```

#### Processing Notifications

Processing notifications is heavily dependent on the Workflow that is configured for your Merchant. The `Notification` class provides all necessary properties to properly handle the notifications you receive according to the transaction workflow.

For example, you can check if a transaction was approved:

```csharp
if (notification.Operations.OperationList.Last().Status == "SUCCESS")
{
    // Transaction was approved
    Console.WriteLine("Transaction Completed Successfully");
}
else
{
    // Transaction was not approved
    Console.WriteLine("Transaction Not Completed");
}
```

#### Full Code Example for Parsing XML

Here's a complete example demonstrating how to parse notification data:

```csharp
using DotNetPaymentSDK.src.Parameters.Notification;
using DotNetPayment.Core.Domain.Enums;

public class NotificationProcessor
{
    public void ProcessNotification(string notificationString)
    {
        try
        {
            // Parse the notification string
            Notification notification = NotificationAdapter.ParseNotification(notificationString);

            // Get basic transaction details
            var operation = notification.Operations.OperationList.Last();
            
            Console.WriteLine($"Transaction ID: {firstOperation.TransactionId}");
            Console.WriteLine($"Merchant Transaction ID: {firstOperation.MerchantTransactionId}");
            Console.WriteLine($"Status: {firstOperation.Status}");
            Console.WriteLine($"Amount: {firstOperation.Amount}");
            Console.WriteLine($"Currency: {firstOperation.Currency}");
            Console.WriteLine($"Payment Solution: {firstOperation.PaymentSolution}");
            Console.WriteLine($"Service Type: {firstOperation.Service}");

            // Implement your business logic based on the notification data
            if (operation.Status == "SUCCESS")
            {
                // Update database with successful transaction
                UpdateDatabaseWithSuccess(operation.MerchantTransactionId);
            }
            else
            {
                // Handle failed transaction
                LogFailedTransaction(operation.MerchantTransactionId, operation.Status);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing notification: {ex.Message}");
            // Handle exceptions appropriately
        }
    }

    private void UpdateDatabaseWithSuccess(string MerchantTransactionId)
    {
        // Implement database update logic
    }

    private void LogFailedTransaction(string MerchantTransactionId, string status)
    {
        // Implement logging logic
    }
}
```