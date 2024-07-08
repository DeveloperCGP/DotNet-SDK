# Quix Hosted

## Table of Contents
- [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object)
- [Quix Hosted Items Request](#quix-hosted-items-request)
  - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite)
  - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object)
  - [Step 3: Send The Quix Hosted Items Request and Retrieve Response](#step-3-send-the-quix-hosted-items-request-and-retrieve-response)
- [Quix Hosted Accommodation Request](#quix-hosted-accommodation-request)
  - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-1)
  - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-1)
  - [Step 3: Send The Quix Accommodation Items Request and Retrieve Response](#step-3-send-the-quix-accommodation-items-request-and-retrieve-response)
- [Quix Hosted Service Request](#quix-hosted-service-request)
  - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-2)
  - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-2)
  - [Step 3: Send The Quix Service Items Request and Retrieve Response](#step-3-send-the-quix-service-items-request-and-retrieve-response)
- [Quix Hosted Flights Request](#quix-hosted-flights-request)
  - [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-3)
  - [Step 2: Creating Payment Parameter Object](#step-2-creating-payment-parameter-object-3)
  - [Step 3: Send The Quix Hosted Flights Request and Retrieve Response](#step-3-send-the-quix-hosted-flights-request-and-retrieve-response)
- [Complete Example](#complete-example)

## Common Prerequisite: Creating Credentials Object

First, instantiate the Credentials object with your merchant details. This includes your Merchant ID and Merchant Pass which are essential for authenticating requests to the AddonPayments API. In this section, we set up the necessary credentials for the payment service. The credentials include the merchant ID, merchant password, environment, product ID, and API version.

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
credentials.SetMerchantId("114465");
credentials.SetMerchantPass("b223a2ce4ec3540c848d5914520c8199");
credentials.SetEnvironment(EnvironmentEnum.STAGING);
credentials.SetProductId("1144650001");
credentials.SetApiVersion(5);
paymentService.Credentials = credentials;
```

## Quix Hosted Items Request

Sending normal payment h2h request which is used in a normal payment.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

```csharp
HostedQuixProduct hostedQuixItem = new();
hostedQuixItem.SetAmount(amount.ToString());
hostedQuixItem.SetCustomerId("903");
hostedQuixItem.SetStatusURL("https://test.com/status");
hostedQuixItem.setCancelURL("https://test.com/cancel");
hostedQuixItem.setErrorURL("https://test.com/error");
hostedQuixItem.setSuccessURL("https://test.com/success");
hostedQuixItem.SetAwaitingURL("https://test.com/awaiting");
hostedQuixItem.SetCustomerEmail("test@mail.com");
hostedQuixItem.SetCustomerNationalId("99999999R");
hostedQuixItem.SetDob("01-12-1999");
hostedQuixItem.SetFirstName("Name");
hostedQuixItem.SetLastName("Last Name");
hostedQuixItem.SetIpAddress("0.0.0.0");

QuixArticleProduct quixArticleProduct = new();
quixArticleProduct.SetName("Nombre del servicio 2");
quixArticleProduct.SetReference("4912345678903");
quixArticleProduct.SetUnitPriceWithTax("50");
quixArticleProduct.SetCategory(CategoryEnum.digital);

QuixItemCartItemProduct quixItemCartItemProduct = new();
quixItemCartItemProduct.setArticle(quixArticleProduct);
quixItemCartItemProduct.SetUnits(1);
quixItemCartItemProduct.SetAutoShipping(true);
quixItemCartItemProduct.SetTotalPriceWithTax("50");

List<QuixItemCartItemProduct> items = [];
items.Add(quixItemCartItemProduct);

QuixCartProduct quixCartProduct = new();
quixCartProduct.SetCurrency(Currency.EUR);
quixCartProduct.SetItems(items);
quixCartProduct.SetTotalPriceWithTax("50");

QuixAddress quixAddress = new();
quixAddress.SetCity("Barcelona");
quixAddress.SetCountry(CountryCodeAlpha3.ESP);
quixAddress.SetStreetAddress("Nombre de la vía y nº");
quixAddress.SetPostalCode("28003");

QuixBilling quixBilling = new();
quixBilling.SetAddress(quixAddress);
quixBilling.SetFirstName("Nombre");
quixBilling.SetLastName("Apellido");

QuixProductPaySolExtendedData quixItemPaySolExtendedData = new();
quixItemPaySolExtendedData.SetCart(quixCartProduct);
quixItemPaySolExtendedData.SetBilling(quixBilling);
quixItemPaySolExtendedData.SetProduct("instalments");

hostedQuixItem.SetPaySolExtendedData(quixItemPaySolExtendedData);
```

### Step 3: Send The Quix Hosted Items Request and Retrieve Response

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
    paymentService.SendHostedQuixProductRequest(hostedQuixItem, new ResponseListener()
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

Note: It's important to note that the status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Quix Hosted Accommodation Request

Sending normal payment h2h request which is used in a

 normal payment.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

```csharp
HostedQuixAccommodation hostedQuixAccommodation = new();
hostedQuixAccommodation.SetAmount(amount.ToString());
hostedQuixAccommodation.SetCustomerId("903");
hostedQuixAccommodation.SetStatusURL("https://test.com/status");
hostedQuixAccommodation.setCancelURL("https://test.com/cancel");
hostedQuixAccommodation.setErrorURL("https://test.com/error");
hostedQuixAccommodation.setSuccessURL("https://test.com/success");
hostedQuixAccommodation.SetAwaitingURL("https://test.com/awaiting");
hostedQuixAccommodation.SetCustomerEmail("test@mail.com");
hostedQuixAccommodation.SetCustomerNationalId("99999999R");
hostedQuixAccommodation.SetDob("01-12-1999");
hostedQuixAccommodation.SetFirstName("Name");
hostedQuixAccommodation.SetLastName("Last Name");
hostedQuixAccommodation.SetIpAddress("0.0.0.0");

QuixAddress quixAddress = new QuixAddress();
quixAddress.SetCity("Barcelona");
quixAddress.SetCountry(CountryCodeAlpha3.ESP);
quixAddress.SetStreetAddress("Nombre de la vía y nº");
quixAddress.SetPostalCode("28003");

QuixArticleAccommodation quixArticleAccommodation = new();
quixArticleAccommodation.SetName("Nombre del servicio 2");
quixArticleAccommodation.SetReference("4912345678903");
quixArticleAccommodation.SetCheckinDate("2024-10-30T00:00:00+01:00");
quixArticleAccommodation.SetCheckoutDate("2024-12-31T23:59:59+01:00");
quixArticleAccommodation.SetGuests(1);
quixArticleAccommodation.SetEstablishmentName("Hotel");
quixArticleAccommodation.SetAddress(quixAddress);
quixArticleAccommodation.SetUnitPriceWithTax("50");
quixArticleAccommodation.SetCategory(CategoryEnum.digital);

QuixItemCartItemAccommodation quixItemCartItemAccommodation = new();
quixItemCartItemAccommodation.SetArticle(quixArticleAccommodation);
quixItemCartItemAccommodation.SetUnits(1);
quixItemCartItemAccommodation.SetAutoShipping(true);
quixItemCartItemAccommodation.SetTotalPriceWithTax("50");

List<QuixItemCartItemAccommodation> items = [];
items.Add(quixItemCartItemAccommodation);

QuixCartAccommodation quixCartAccommodation = new QuixCartAccommodation();
quixCartAccommodation.SetCurrency(Currency.EUR);
quixCartAccommodation.SetItems(items);
quixCartAccommodation.SetTotalPriceWithTax("50");

QuixBilling quixBilling = new QuixBilling();
quixBilling.SetAddress(quixAddress);
quixBilling.SetFirstName("Nombre");
quixBilling.SetLastName("Apellido");

QuixAccommodationPaySolExtendedData quixAccommodationPaySolExtendedData = new QuixAccommodationPaySolExtendedData
{
    Cart = quixCartAccommodation,
    Billing = quixBilling,
    Product = "instalments"
};

hostedQuixAccommodation.SetPaySolExtendedData(quixAccommodationPaySolExtendedData);
```

### Step 3: Send The Quix Accommodation Items Request and Retrieve Response

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
    paymentService.SendHostedQuixAccommodationRequest(hostedQuixAccommodation, new ResponseListener()
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

Note: It's important to note that the status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Quix Hosted Service Request

Sending normal payment h2h request which is used in a normal payment.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

```csharp
HostedQuixService hostedQuixService = new();
hostedQuixService.SetAmount(amount.ToString());
hostedQuixService.SetCustomerId("903");
hostedQuixService.SetStatusURL("https://test.com/status");
hostedQuixService.setCancelURL("https://test.com/cancel");
hostedQuixService.setErrorURL("https://test.com/error");
hostedQuixService.setSuccessURL("https://test.com/success");
hostedQuixService.SetAwaitingURL("https://test.com/awaiting");
hostedQuixService.SetCustomerEmail("test@mail.com");
hostedQuixService.SetCustomerNationalId("99999999R");
hostedQuixService.SetDob("01-12-1999");
hostedQuixService.SetFirstName("Name");
hostedQuixService.SetLastName("Last Name");
hostedQuixService.SetIpAddress("0.0.0.0");

QuixArticleService quixArticleService = new();
quixArticleService.SetName("Nombre del servicio 2");
quixArticleService.SetReference("4912345678903");
quixArticleService.SetEndDate("2024-12-31T23:59:59+01:00");
quixArticleService.SetUnitPriceWithTax("50");
quixArticleService.SetCategory(CategoryEnum.digital);

QuixItemCartItemService quixItemCartItemService = new QuixItemCartItemService();
quixItemCartItemService.SetArticle(quixArticleService);
quixItemCartItemService.SetUnits(1);
quixItemCartItemService.SetAutoShipping(true);
quixItemCartItemService.SetTotalPriceWithTax("50");

List<QuixItemCartItemService> items = [];
items.Add(quixItemCartItemService);

QuixCartService quixCartService = new();
quixCartService.SetCurrency(Currency.EUR);
quixCartService.SetItems(items);
quixCartService.SetTotalPriceWithTax("50");

QuixAddress quixAddress = new QuixAddress();
quixAddress.SetCity("Barcelona");
quixAddress.SetCountry(CountryCodeAlpha3.ESP);
quixAddress.SetStreetAddress("Nombre de la vía y nº");
quixAddress.SetPostalCode("28003");

QuixBilling quixBilling = new();
quixBilling.SetAddress(quixAddress);
quixBilling.SetFirstName("Nombre");
quixBilling.SetLastName("Apellido");

QuixServicePaySolExtendedData quixServicePaySolExtendedData = new();
quixServicePaySolExtendedData.SetCart(quixCartService);
quixServicePaySolExtendedData.SetBilling(quixBilling);
quixServicePaySolExtendedData.SetProduct("instalments");

hostedQuixService.SetPaySolExtendedData(quixServicePaySolExtendedData);
```

### Step 3: Send The Quix Service Items Request and Retrieve Response

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

#### ResponseListener Class Implementation

```csharp
using DotNetPayment.Core.Domain.Enums;
using Dot

NetPaymentSDK.Callbacks;
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
    paymentService.SendHostedQuixServiceRequest(hostedQuixService, new ResponseListener()
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

Note: It's important to note that the status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Quix Hosted Flights Request

Sending normal payment h2h request which is used in a normal payment.

### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

### Step 2: Creating Payment Parameter Object

In this step, we will provide the SDK with the payment parameters:

```csharp
HostedQuixFlight hostedQuixFlight = new();
hostedQuixFlight.SetAmount(amount.ToString());
hostedQuixFlight.SetCustomerId("903");
hostedQuixFlight.SetStatusURL("https://test.com/status");
hostedQuixFlight.setCancelURL("https://test.com/cancel");
hostedQuixFlight.setErrorURL("https://test.com/error");
hostedQuixFlight.setSuccessURL("https://test.com/success");
hostedQuixFlight.SetAwaitingURL("https://test.com/awaiting");
hostedQuixFlight.SetCustomerEmail("test@mail.com");
hostedQuixFlight.SetCustomerNationalId("99999999R");
hostedQuixFlight.SetDob("01-12-1999");
hostedQuixFlight.SetFirstName("Name");
hostedQuixFlight.SetLastName("Last Name");

hostedQuixFlight.SetIpAddress("0.0.0.0");

QuixPassengerFlight quixPassengerFlight = new QuixPassengerFlight();
quixPassengerFlight.SetFirstName("Pablo");
quixPassengerFlight.SetLastName("Navvaro");

List<QuixPassengerFlight> passangers = [];
passangers.Add(quixPassengerFlight);

QuixSegmentFlight quixSegmentFlight = new QuixSegmentFlight();
quixSegmentFlight.SetIataDepartureCode("MAD");
quixSegmentFlight.SetIataDestinationCode("BCN");

List<QuixSegmentFlight> segments = [];
segments.Add(quixSegmentFlight);

QuixArticleFlight quixArticleFlight = new QuixArticleFlight();
quixArticleFlight.SetName("Nombre del servicio 2");
quixArticleFlight.SetReference("4912345678903");
quixArticleFlight.SetDepartureDate("2024-12-31T23:59:59+01:00");
quixArticleFlight.SetPassengers(passangers);
quixArticleFlight.SetSegments(segments);
quixArticleFlight.SetUnitPriceWithTax("50");
quixArticleFlight.SetCategory(CategoryEnum.digital);

QuixItemCartItemFlight quixItemCartItemFlight = new QuixItemCartItemFlight();
quixItemCartItemFlight.SetArticle(quixArticleFlight);
quixItemCartItemFlight.SetUnits(1);
quixItemCartItemFlight.SetAutoShipping(true);
quixItemCartItemFlight.SetTotalPriceWithTax("50");

List<QuixItemCartItemFlight> items = [];
items.Add(quixItemCartItemFlight);

QuixCartFlight quixCartFlight = new QuixCartFlight();
quixCartFlight.SetCurrency(Currency.EUR);
quixCartFlight.SetItems(items);
quixCartFlight.SetTotalPriceWithTax("50");

QuixAddress quixAddress = new QuixAddress();
quixAddress.SetCity("Barcelona");
quixAddress.SetCountry(CountryCodeAlpha3.ESP);
quixAddress.SetStreetAddress("Nombre de la vía y nº");
quixAddress.SetPostalCode("28003");

QuixBilling quixBilling = new QuixBilling();
quixBilling.SetAddress(quixAddress);
quixBilling.SetFirstName("Nombre");
quixBilling.SetLastName("Apellido");

QuixFlightPaySolExtendedData quixFlightPaySolExtendedData = new QuixFlightPaySolExtendedData
{
    Cart = quixCartFlight,
    Billing = quixBilling,
    Product = "instalments"
};

hostedQuixFlight.SetPaySolExtendedData(quixFlightPaySolExtendedData);
```

### Step 3: Send The Quix Hosted Flights Request and Retrieve Response

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
    paymentService.SendHostedQuixFlightRequest(hostedQuixFlight, new ResponseListener()
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

Note: It's important to note that the status of the transaction, whether it's a success or an error, will be communicated asynchronously via a webhook notification. Within the SDK, we've included a method to create a webhook and notification handler, enabling you to receive these transaction notifications efficiently and take action. This allows for real-time updates on transaction statuses directly within your application.

## Complete Example

Here is the complete example combining all the sections, including the ResponseListener class implementation directly within the same file for ease of understanding:

```csharp
public class QuixHostedController : Controller
{
    private readonly QUIXHostedService paymentService;

    public QuixHostedController(QUIXHostedService paymentService)
    {
        this.paymentService = paymentService;
    }

    [HttpPost("items/payment")]
    public async Task<IActionResult> SendQUIXHostedItemsPaymentRequest([FromQuery] int amount)
    {
        Credentials credentials = new();
        credentials.SetMerchantId("114465");
        credentials.SetMerchantPass("b223a2ce4ec3540c848d5914520c8199");
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId("1144650001");
        credentials.SetApiVersion(5);
        paymentService.Credentials = credentials;

        HostedQuixProduct hostedQuixItem = new();
        hostedQuixItem.SetAmount(amount.ToString());
        hostedQuixItem.SetCustomerId("903");
        hostedQuixItem.SetStatusURL("https://test.com/status");
        hostedQuixItem.setCancelURL("https://test.com/cancel");
        hostedQuixItem.setError

URL("https://test.com/error");
        hostedQuixItem.setSuccessURL("https://test.com/success");
        hostedQuixItem.SetAwaitingURL("https://test.com/awaiting");
        hostedQuixItem.SetCustomerEmail("test@mail.com");
        hostedQuixItem.SetCustomerNationalId("99999999R");
        hostedQuixItem.SetDob("01-12-1999");
        hostedQuixItem.SetFirstName("Name");
        hostedQuixItem.SetLastName("Last Name");
        hostedQuixItem.SetIpAddress("0.0.0.0");

        QuixArticleProduct quixArticleProduct = new();
        quixArticleProduct.SetName("Nombre del servicio 2");
        quixArticleProduct.SetReference("4912345678903");
        quixArticleProduct.SetUnitPriceWithTax("50");
        quixArticleProduct.SetCategory(CategoryEnum.digital);

        QuixItemCartItemProduct quixItemCartItemProduct = new();
        quixItemCartItemProduct.setArticle(quixArticleProduct);
        quixItemCartItemProduct.SetUnits(1);
        quixItemCartItemProduct.SetAutoShipping(true);
        quixItemCartItemProduct.SetTotalPriceWithTax("50");

        List<QuixItemCartItemProduct> items = [];
        items.Add(quixItemCartItemProduct);

        QuixCartProduct quixCartProduct = new();
        quixCartProduct.SetCurrency(Currency.EUR);
        quixCartProduct.SetItems(items);
        quixCartProduct.SetTotalPriceWithTax("50");

        QuixAddress quixAddress = new();
        quixAddress.SetCity("Barcelona");
        quixAddress.SetCountry(CountryCodeAlpha3.ESP);
        quixAddress.SetStreetAddress("Nombre de la vía y nº");
        quixAddress.SetPostalCode("28003");

        QuixBilling quixBilling = new();
        quixBilling.SetAddress(quixAddress);
        quixBilling.SetFirstName("Nombre");
        quixBilling.SetLastName("Apellido");

        QuixProductPaySolExtendedData quixItemPaySolExtendedData = new();
        quixItemPaySolExtendedData.SetCart(quixCartProduct);
        quixItemPaySolExtendedData.SetBilling(quixBilling);
        quixItemPaySolExtendedData.SetProduct("instalments");

        hostedQuixItem.SetPaySolExtendedData(quixItemPaySolExtendedData);
        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendHostedQuixProductRequest(hostedQuixItem, new ResponseListener()
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