# Quix Js

## Table of Contents
1. [Common Prerequisite: Creating Credentials Object](#common-prerequisite-creating-credentials-object)
2. [Use Case 1: JavaScript Authentication Request](#use-case-1-javascript-authentication-request)
   1. [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite)
   2. [Step 2: Setting Payment Parameters](#step-2-setting-payment-parameters)
   3. [Step 3: Sending the Authentication Request and Retrieve it](#step-3-sending-the-authentication-request-and-retrieve-it)
3. [Use Case 2: JavaScript Quix Items Request](#use-case-2-javascript-quix-items-request)
   1. [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-1)
   2. [Step 2: Setting Payment Parameters for Charge Request](#step-2-setting-payment-parameters-for-charge-request)
   3. [Step 3: Sending the Charge Quix Items Request and Retrieve it](#step-3-sending-the-charge-quix-items-request-and-retrieve-it)
4. [Use Case 2: JavaScript Quix Accommodation Request](#use-case-2-javascript-quix-accommodation-request)
   1. [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-2)
   2. [Step 2: Setting Payment Parameters for Charge Request](#step-2-setting-payment-parameters-for-charge-request-1)
   3. [Step 3: Sending the Charge Quix Accommodation Request and Retrieve it](#step-3-sending-the-charge-quix-accommodation-request-and-retrieve-it)
5. [Use Case 2: JavaScript Quix Service Request](#use-case-2-javascript-quix-service-request)
   1. [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-3)
   2. [Step 2: Setting Payment Parameters for Charge Request](#step-2-setting-payment-parameters-for-charge-request-2)
   3. [Step 3: Sending the Charge Quix Service Request and Retrieve it](#step-3-sending-the-charge-quix-service-request-and-retrieve-it)
6. [Use Case 2: JavaScript Quix Flights Request](#use-case-2-javascript-quix-flights-request)
   1. [Step 1: Refer to Common Prerequisite](#step-1-refer-to-common-prerequisite-4)
   2. [Step 2: Setting Payment Parameters for Charge Request](#step-2-setting-payment-parameters-for-charge-request-3)
   3. [Step 3: Sending the Charge Quix Flights Request and Retrieve it](#step-3-sending-the-charge-quix-flights-request-and-retrieve-it)
7. [Complete Example](#complete-example)

### Common Prerequisite: Creating Credentials Object

First, instantiate the Credentials object with your merchant details. This includes your Merchant ID and Merchant Pass which are essential for authenticating requests to the AddonPayments API. In this section, we set up the necessary credentials for the payment service. The credentials include the merchant ID, merchant password, environment, product ID, and API version.

**Steps**

1. **Initialize Credentials Object:** Create a new instance of the Credentials class to hold the authentication and configuration details.
2. **Set Merchant ID:** Assign the merchant ID using the `SetMerchantId` method. This ID is provided by the payment service provider and identifies the merchant account.
3. **Set Merchant Key:** Assign the merchant key using the `SetMerchantKey` method. This key is provided by the payment service provider and is used for authentication.
4. **Set Environment:** Specify the environment (e.g., STAGING, PRODUCTION) using the `SetEnvironment` method. This determines the endpoint URL for the payment requests.
5. **Set Product ID:** Assign the product ID using the `SetProductId` method. This ID identifies the specific product or service being paid for.
6. **Set API Version:** Specify the API version using the `SetApiVersion` method. This ensures compatibility with the payment service's API.
7. **Assign Credentials to Payment Service:** Finally, assign the configured credentials object to the `Credentials` property of the paymentService. This step is crucial as it links the payment service instance with the necessary authentication and configuration details, allowing it to authenticate and process payment requests.

```java
Credentials credentials = new();
credentials.SetMerchantId("114658");
credentials.SetEnvironment(EnvironmentEnum.STAGING);
credentials.SetProductId("1146580001");
credentials.SetApiVersion(5);
credentials.SetMerchantKey("3535457e-qe21-40t7-863e-e5838e53499e");
paymentService.Credentials = credentials;
```

### Use Case 1: JavaScript Authentication Request

#### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the ["Common Prerequisite: Creating Credentials Object"](https://redmine.dev.mindfulpayments.com/projects/net-payment-sdk/wiki/JavaScript#Common-Prerequisite-Creating-Credentials-Object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

#### Step 2: Setting Payment Parameters

In this step, we will provide the SDK with the payment parameters:
- Amount
- Currency
- Country
- Customer ID

```csharp
JSAuthorizationRequestParameters jSAuthPaymentParameters = new();
jSAuthPaymentParameters.SetCustomerId("8881");
jSAuthPaymentParameters.SetCurrency(Currency.EUR);
jSAuthPaymentParameters.SetCountry(CountryCodeAlpha2.ES);
jSAuthPaymentParameters.SetOperationType(OperationTypes.DEBIT);
```

#### Step 3: Sending the Authentication Request and Retrieve it

The response from the payment service is handled using a custom `JSResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `JSResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

**JSResponseListener Class Implementation**

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

**Code Snippet for Using the JSResponseListener**

```csharp
var tcs = new TaskCompletionSource<(string?, string?)>();

await Task.Run(() =>
{
    paymentService.SendJSAuthorizationRequest(jSAuthPaymentParameters, new JSResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnAuthorizationResponseReceivedAction = (string rawResponse, JSAuthorizationResponse jsAuthorizationResponse) =>
        {
            tcs.SetResult((jsAuthorizationResponse.AuthToken, null));
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
    return Ok(new { auth_token = result.Item1 });
}
```

### Use Case 2: JavaScript Quix Items Request

#### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the ["Common Prerequisite: Creating Credentials Object"](https://redmine.dev.mindfulpayments.com/projects/net-payment-sdk/wiki/Quix_Js#Common-Prerequisite-Creating-Credentials-Object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

#### Step 2: Setting Payment Parameters for Charge Request

In this step, we will provide the SDK with the payment parameters:

```csharp
JSQuixProduct jsQuixItem = new();
jsQuixItem.SetPrepayToken("5133c-ad3391-ziwu313-n9513z-3weq");
jsQuixItem.SetAmount("50");
jsQuixItem.SetCustomerId("55");
jsQuixItem.SetStatusURL("https://test.com/status");
jsQuixItem.setCancelURL("https://test.com/cancel");
jsQuixItem.setErrorURL("https://test.com/error");
jsQuixItem.setSuccessURL("https://test.com/success");
jsQuixItem.SetAwaitingURL("https://test.com/awaiting");
jsQuixItem.SetCustomerEmail("test@mail.com");
jsQuixItem.SetCustomerNationalId("99999999R");
jsQuixItem.SetDob("01-12-1999");
jsQuixItem.SetFirstName("Name");
jsQuixItem.SetLastName("Last Name");
jsQuixItem.SetIpAddress("0.0.0.0");

QuixArticleProduct quixArticleProduct = new();
quixArticleProduct.SetName("Nombre del servicio 2");
quixArticleProduct.SetReference("4912345678903");
quixArticleProduct.SetUnitPriceWithTax("50");
quixArticleProduct.SetCategory(CategoryEnum.digital);

QuixItemCartItemProduct quixItemCartItemProduct = new QuixItemCartItemProduct();
quixItemCartItemProduct.setArticle(quixArticleProduct);
quixItemCartItemProduct.SetUnits(1);
quixItemCartItemProduct.SetAutoShipping(true);
quixItemCartItemProduct.SetTotalPriceWithTax("50");

List<QuixItemCartItemProduct> items = [];
items.Add(quixItemCartItemProduct);

QuixCartProduct quixCartProduct = new QuixCartProduct();
quixCartProduct.SetCurrency(Currency.EUR);
quixCartProduct.SetItems(items);
quixCartProduct.SetTotalPriceWithTax("50");

QuixAddress quixAddress = new QuixAddress();
quixAddress.SetCity("Barcelona");
quixAddress.SetCountry(CountryCodeAlpha3.ESP);
quixAddress.SetStreetAddress("Nombre de la vía y nº");
quixAddress.SetPostalCode("08003");

QuixBilling quixBilling = new QuixBilling();
quixBilling.SetAddress(quixAddress);
quixBilling.SetFirstName("Nombre");
quixBilling.SetLastName("Apellido");

QuixProductPaySolExtendedData quixItemPaySolExtendedData = new();
quixItemPaySolExtendedData.SetCart(quixCartProduct);
quixItemPaySolExtendedData.SetBilling(quixBilling);
quixItemPaySolExtendedData.SetProduct("instalments");

jsQuixItem.SetPaySolExtendedData(quixItemPaySolExtendedData);
```

#### Step 3: Sending the Charge Quix Items Request and Retrieve it

The response from the payment service is handled using a custom `ResponseListener` class. This class

 is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

**ResponseListener Class Implementation**

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

**Code Snippet for Using the JSResponseListener**

```csharp
var tcs = new TaskCompletionSource<(Notification?, string?)>();

await Task.Run(() =>
{
    paymentService.SendJSQuixProductRequest(jsQuixItem, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification, null));
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
    return Ok(new
    {
        nemuru_cart_hash = result.Item1.GetNemuruCartHash(),
        nemuru_auth_token = result.Item1.GetNemuruAuthToken(),
    });
}
```

Use `nemuru_cart_hash` and `nemuru_auth_token` to render Quix dialog.

### Use Case 2: JavaScript Quix Accommodation Request

#### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the ["Common Prerequisite: Creating Credentials Object"](https://redmine.dev.mindfulpayments.com/projects/net-payment-sdk/wiki/Quix_Js#Common-Prerequisite-Creating-Credentials-Object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

#### Step 2: Setting Payment Parameters for Charge Request

In this step, we will provide the SDK with the payment parameters:

```csharp
JSQuixAccommodation jsQuixAccommodation = new();
jsQuixAccommodation.SetPrepayToken("5133c-ad3391-ziwu313-n9513z-3weq");
jsQuixAccommodation.SetAmount("50");
jsQuixAccommodation.SetCustomerId("55");
jsQuixAccommodation.SetStatusURL("https://test.com/status");
jsQuixAccommodation.setCancelURL("https://test.com/cancel");
jsQuixAccommodation.setErrorURL("https://test.com/error");
jsQuixAccommodation.setSuccessURL("https://test.com/success");
jsQuixAccommodation.SetAwaitingURL("https://test.com/awaiting");
jsQuixAccommodation.SetCustomerEmail("test@mail.com");
jsQuixAccommodation.SetCustomerNationalId("99999999R");
jsQuixAccommodation.SetDob("01-12-1999");
jsQuixAccommodation.SetFirstName("Name");
jsQuixAccommodation.SetLastName("Last Name");
jsQuixAccommodation.SetIpAddress("0.0.0.0");

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

QuixItemCartItemAccommodation quixItemCartItemAccommodation = new QuixItemCartItemAccommodation();
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

jsQuixAccommodation.SetPaySolExtendedData(quixAccommodationPaySolExtendedData);
```

#### Step 3: Sending the Charge Quix Accommodation Request and Retrieve it

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

**ResponseListener Class Implementation**

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

**Code Snippet for Using the JSResponseListener**

```csharp
var tcs = new TaskCompletionSource<(Notification?, string?)>();

await Task.Run(() =>
{
    paymentService.SendJSQuixAccommodationRequest(jsQuixAccommodation, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification, null));
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
    return Ok(new
    {
        nemuru_cart_hash = result.Item1.GetNemuruCartHash(),
        nemuru_auth_token = result.Item1.GetNemuruAuthToken(),
    });
}
```

Use `nemuru_cart_hash` and `nemuru_auth_token` to render Quix dialog.

### Use Case 2: JavaScript Quix Service Request

#### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the ["Common Prerequisite: Creating Credentials Object"](https://redmine.dev.mindfulpayments.com/projects/net-payment-sdk/wiki/Quix_Js#Common-Prerequisite-Creating-Credentials-Object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

#### Step 2: Setting Payment Parameters for Charge Request

In this step, we will provide the SDK with the payment parameters:

```csharp
JSQuixService jsQuixService = new();
jsQuixService.SetPrepayToken("5133c-ad3391-ziwu313-n9513z-3weq");
jsQuixService.SetAmount("50");
jsQuixService.SetCustomerId("55");
jsQuixService.SetStatusURL("https://test.com/status");
jsQuixService.setCancelURL("https://test.com/cancel");
jsQuixService.setErrorURL("https://test.com/error");
jsQuixService.setSuccessURL("https://test.com/success");
jsQuixService.SetAwaitingURL("https://test.com/awaiting");
jsQuixService.SetCustomerEmail("test@mail.com

");
jsQuixService.SetCustomerNationalId("99999999R");
jsQuixService.SetDob("01-12-1999");
jsQuixService.SetFirstName("Name");
jsQuixService.SetLastName("Last Name");
jsQuixService.SetIpAddress("0.0.0.0");

QuixArticleService quixArticleService = new();
quixArticleService.SetName("Nombre del servicio 2");
quixArticleService.SetReference("4912345678903");
quixArticleService.SetEndDate("2024-12-31T23:59:59+01:00");
quixArticleService.SetUnitPriceWithTax("50");
quixArticleService.SetCategory(CategoryEnum.digital);

QuixItemCartItemService quixItemCartItemService = new();
quixItemCartItemService.SetArticle(quixArticleService);
quixItemCartItemService.SetUnits(1);
quixItemCartItemService.SetAutoShipping(true);
quixItemCartItemService.SetTotalPriceWithTax("50");

List<QuixItemCartItemService> items = [];
items.Add(quixItemCartItemService);

QuixCartService quixCartService = new QuixCartService();
quixCartService.SetCurrency(Currency.EUR);
quixCartService.SetItems(items);
quixCartService.SetTotalPriceWithTax("50");

QuixAddress quixAddress = new QuixAddress();
quixAddress.SetCity("Barcelona");
quixAddress.SetCountry(CountryCodeAlpha3.ESP);
quixAddress.SetStreetAddress("Nombre de la vía y nº");
quixAddress.SetPostalCode("28003");

QuixBilling quixBilling = new QuixBilling();
quixBilling.SetAddress(quixAddress);
quixBilling.SetFirstName("Nombre");
quixBilling.SetLastName("Apellido");

QuixServicePaySolExtendedData quixServicePaySolExtendedData = new QuixServicePaySolExtendedData();
quixServicePaySolExtendedData.SetCart(quixCartService);
quixServicePaySolExtendedData.SetBilling(quixBilling);
quixServicePaySolExtendedData.SetProduct("instalments");

jsQuixService.SetPaySolExtendedData(quixServicePaySolExtendedData);
```

#### Step 3: Sending the Charge Quix Service Request and Retrieve it

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

**ResponseListener Class Implementation**

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

**Code Snippet for Using the JSResponseListener**

```csharp
var tcs = new TaskCompletionSource<(Notification?, string?)>();

await Task.Run(() =>
{
    paymentService.SendJSQuixServiceRequest(jsQuixService, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification, null));
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
    return Ok(new
    {
        nemuru_cart_hash = result.Item1.GetNemuruCartHash(),
        nemuru_auth_token = result.Item1.GetNemuruAuthToken(),
    });
}
```

Use `nemuru_cart_hash` and `nemuru_auth_token` to render Quix dialog.

### Use Case 2: JavaScript Quix Flights Request

#### Step 1: Refer to Common Prerequisite

Before proceeding with the Hosted Request, please refer to the ["Common Prerequisite: Creating Credentials Object"](https://redmine.dev.mindfulpayments.com/projects/net-payment-sdk/wiki/Quix_Js#Common-Prerequisite-Creating-Credentials-Object) section at the beginning of this documentation for the initial setup of the SDK credentials. Ensure you have correctly configured your credentials as described there.

#### Step 2: Setting Payment Parameters for Charge Request

In this step, we will provide the SDK with the payment parameters:

```csharp
JSQuixFlight jsQuixFlight = new();
jsQuixFlight.SetPrepayToken("5133c-ad3391-ziwu313-n9513z-3weq");
jsQuixFlight.SetAmount("50");
jsQuixFlight.SetCustomerId("55");
jsQuixFlight.SetStatusURL("https://test.com/status");
jsQuixFlight.setCancelURL("https://test.com/cancel");
jsQuixFlight.setErrorURL("https://test.com/error");
jsQuixFlight.setSuccessURL("https://test.com/success");
jsQuixFlight.SetAwaitingURL("https://test.com/awaiting");
jsQuixFlight.SetCustomerEmail("test@mail.com");
jsQuixFlight.SetCustomerNationalId("99999999R");
jsQuixFlight.SetDob("01-12-1999");
jsQuixFlight.SetFirstName("Name");
jsQuixFlight.SetLastName("Last Name");
jsQuixFlight.SetIpAddress("0.0.0.0");

QuixPassengerFlight quixPassengerFlight = new();
quixPassengerFlight.SetFirstName("Pablo");
quixPassengerFlight.SetLastName("Navvaro");

List<QuixPassengerFlight> passengers = [];
passengers.Add(quixPassengerFlight);

QuixSegmentFlight quixSegmentFlight = new();
quixSegmentFlight.SetIataDepartureCode("MAD");
quixSegmentFlight.SetIataDestinationCode("BCN");

List<QuixSegmentFlight> segments = [];
segments.Add(quixSegmentFlight);

QuixArticleFlight quixArticleFlight = new();
quixArticleFlight.SetName("Nombre del servicio 2");
quixArticleFlight.SetReference("4912345678903");
quixArticleFlight.SetDepartureDate("2024-12-31T23:59:59+01:00");
quixArticleFlight.SetPassengers(passengers);
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

QuixFlightPaySolExtendedData quixFlightPaySolExtendedData = new()
{
    Cart = quixCartFlight,
    Billing = quixBilling,
    Product = "instalments"
};

jsQuixFlight.SetPaySolExtendedData(quixFlightPaySolExtendedData);
```

#### Step 3: Sending the Charge Quix Flights Request and Retrieve it

The response from the payment service is handled using a custom `ResponseListener` class. This class is part of the integration project and not included in the SDK itself. The `ResponseListener` class defines actions for handling errors and received responses, including redirection URLs.

**ResponseListener Class Implementation**

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

        public void OnResponseReceived(string rawResponse, Notification

 notification, TransactionResult transactionResult)
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

**Code Snippet for Using the JSResponseListener**

```csharp
var tcs = new TaskCompletionSource<(Notification?, string?)>();

await Task.Run(() =>
{
    paymentService.SendJSQuixFlightRequest(jsQuixFlight, new ResponseListener()
    {
        OnErrorAction = (ErrorsEnum error, string message) =>
        {
            tcs.SetResult((null, message));
        },
        OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
        {
            tcs.SetResult((notification, null));
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
    return Ok(new
    {
        nemuru_cart_hash = result.Item1.GetNemuruCartHash(),
        nemuru_auth_token = result.Item1.GetNemuruAuthToken(),
    });
}
```

Use `nemuru_cart_hash` and `nemuru_auth_token` to render Quix dialog.

### Complete Example

```csharp
[Route("Quix/[controller]")]
public class JSController : Controller
{
    private readonly QUIXJSService paymentService;
    private readonly JSService jsPaymentService;
    public JSController(QUIXJSService paymentService, JSService jsPaymentService)
    {
        this.paymentService = paymentService;         
        this.jsPaymentService = jsPaymentService;
    }
    

    [HttpPost("items/auth")]
    public async Task<IActionResult> QuixItemsAuthRequest()
    {
       Credentials credentials = new();
        credentials.SetMerchantId("114658");
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId("1146580001");
        credentials.SetApiVersion(5);
        credentials.SetMerchantKey("3535457e-qe21-40t7-863e-e5838e53499e");
        paymentService.Credentials = credentials;

        JSAuthorizationRequestParameters jsAuthorizationRequest = new();
        jsAuthorizationRequest.SetCountry(CountryCodeAlpha2.ES);
        jsAuthorizationRequest.SetCustomerId("55");
        jsAuthorizationRequest.SetCurrency(Currency.EUR);
        jsAuthorizationRequest.SetOperationType(OperationTypes.DEBIT);
        jsAuthorizationRequest.SetAnonymousCustomer(true);

        var tcs = new TaskCompletionSource<(string?, string?)>();

        await Task.Run(() =>
        {
            jsPaymentService.SendJSAuthorizationRequest(jsAuthorizationRequest, new JSResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnAuthorizationResponseReceivedAction = (string rawResponse, JSAuthorizationResponse jsAuthorizationResponse) =>
                {
                    tcs.SetResult((jsAuthorizationResponse.AuthToken, null));
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
            return Ok(new { auth_token = result.Item1 });
        }
    }

    [HttpPost("items/charge")]
    public async Task<IActionResult> SendJSChargeRequest([FromQuery] string amount, [FromQuery] string prepayToken)
    {
        Credentials credentials = new();
        credentials.SetMerchantId("114658");
        credentials.SetEnvironment(EnvironmentEnum.STAGING);
        credentials.SetProductId("1146580001");
        credentials.SetApiVersion(5);
        credentials.SetMerchantKey("3535457e-qe21-40t7-863e-e5838e53499e");
        paymentService.Credentials = credentials;

        // region Step 2 - Configure Payment Parameters
        JSQuixProduct jsQuixItem = new();
        jsQuixItem.SetPrepayToken("5133c-ad3391-ziwu313-n9513z-3weq");
        jsQuixItem.SetAmount("50");
        jsQuixItem.SetCustomerId("55");
        jsQuixItem.SetStatusURL("https://test.com/status");
        jsQuixItem.setCancelURL("https://test.com/cancel");
        jsQuixItem.setErrorURL("https://test.com/error");
        jsQuixItem.setSuccessURL("https://test.com/success");
        jsQuixItem.SetAwaitingURL("https://test.com/awaiting");
        jsQuixItem.SetCustomerEmail("test@mail.com");
        jsQuixItem.SetCustomerNationalId("99999999R");
        jsQuixItem.SetDob("01-12-1999");
        jsQuixItem.SetFirstName("Name");
        jsQuixItem.SetLastName("Last Name");
        jsQuixItem.SetIpAddress("0.0.0.0");

        QuixArticleProduct quixArticleProduct = new();
        quixArticleProduct.SetName("Nombre del servicio 2");
        quixArticleProduct.SetReference("4912345678903");
        quixArticleProduct.SetUnitPriceWithTax("50");
        quixArticleProduct.SetCategory(CategoryEnum.digital);

        QuixItemCartItemProduct quixItemCartItemProduct = new QuixItemCartItemProduct();
        quixItemCartItemProduct.setArticle(quixArticleProduct);
        quixItemCartItemProduct.SetUnits(1);
        quixItemCartItemProduct.SetAutoShipping(true);
        quixItemCartItemProduct.SetTotalPriceWithTax("50");

        List<QuixItemCartItemProduct> items = [];
        items.Add(quixItemCartItemProduct);

        QuixCartProduct quixCartProduct = new QuixCartProduct();
        quixCartProduct.SetCurrency(Currency.EUR);
        quixCartProduct.SetItems(items);
        quixCartProduct.SetTotalPriceWithTax("50");

        QuixAddress quixAddress = new QuixAddress();
        quixAddress.SetCity("Barcelona");
        quixAddress.SetCountry(CountryCodeAlpha3.ESP);
        quixAddress.SetStreetAddress("Nombre de la vía y nº");
        quixAddress.SetPostalCode("08003");

        QuixBilling quixBilling = new QuixBilling();
        quixBilling.SetAddress(quixAddress);
        quixBilling.SetFirstName("Nombre");
        quixBilling.SetLastName("Apellido");

        QuixProductPaySolExtendedData quixItemPaySolExtendedData = new();
        quixItemPaySolExtendedData.SetCart(quixCartProduct);
        quixItemPaySolExtendedData.SetBilling(quixBilling);
        quixItemPaySolExtendedData.SetProduct("instalments");

        jsQuixItem.SetPaySolExtendedData(quixItemPaySolExtendedData);

        var tcs = new TaskCompletionSource<(Notification?, string?)>();

        await Task.Run(() =>
        {
            paymentService.SendJSQuixProductRequest(jsQuixItem, new ResponseListener()
            {
                OnErrorAction = (ErrorsEnum error, string message) =>
                {
                    tcs.SetResult((null, message));
                },
                OnResponseReceivedAction = (string rawResponse, Notification notification, TransactionResult transactionResult) =>
                {
                    tcs.SetResult((notification, null));
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
            return Ok(new
            {
                nemuru_cart_hash = result.Item1.GetNemuruCartHash(),
                nemuru_auth_token = result.Item1.GetNemuruAuthToken(),
            });
        }
    }

}
```