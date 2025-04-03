using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Configuration;
using DotNetPaymentSDK.Services;
using DotNetPayment.Core.Domain.Enums;
using System.Threading.Tasks;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.Hosted;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.Interfaces;
using DotNetPaymentSDK.src.Adapters;
using System.Collections.Generic;
using DotNetPaymentSDK.src.Parameters.JS;
using DotNetPaymentSDK.src.Requests.Utils;
using DotNetPaymentSDK.src.Parameters.Notification;
using DotNetPaymentSDK.src.Parameters.Quix_JS;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Service;
using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Product;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Accommodation;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight;
using DotNetPaymentSDK.src.Exceptions;

namespace DotNetPaymentSDK.Tests
{
    [TestClass]
    public class QuixJavascriptTests
    {
        private Mock<IConfiguration> _mockConfigurations;
        private IQUIXJSService _realPaymentService;
        private Mock<INetworkAdapter> _mockNetworkAdapter;
        private Mock<ISecurityUtils> _mockSecurityUtils;
        private ISecurityUtils _realSecurityUtils;
        private string capturedUrl;
        private string capturedData;
        private Dictionary<string, string> capturedHeaders;



        private readonly byte[] fixedIv = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 };

        private string ReadXmlContent(string fileName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.FullName;
            string xmlFilePath = Path.Combine(projectDirectory, "Notifications", fileName);

            if (!File.Exists(xmlFilePath))
            {
                throw new FileNotFoundException($"The file {xmlFilePath} was not found.");
            }

            return File.ReadAllText(xmlFilePath);
        }

        [TestInitialize]
        public void Setup()
        {
            _mockConfigurations = new Mock<IConfiguration>();
            _mockNetworkAdapter = new Mock<INetworkAdapter>();
            _mockSecurityUtils = new Mock<ISecurityUtils>();
            _realSecurityUtils = new SecurityUtilsImpl();

            _mockConfigurations.Setup(config => config["merchantId"]).Returns("111222");
            _mockConfigurations.Setup(config => config["merchantPassword"]).Returns("1234567890123456");
            _mockConfigurations.Setup(config => config["productId"]).Returns("11111111");
            _mockConfigurations.Setup(config => config["merchantKey"]).Returns("35354a8e-ce22-40e1-863a-e58a8e53488e");
            _mockConfigurations.Setup(config => config["statusUrl"]).Returns("https://test.com");
            _mockConfigurations.Setup(config => config["baseURL"]).Returns("https://test.com");
            _mockConfigurations.Setup(config => config["successUrl"]).Returns("/success");
            _mockConfigurations.Setup(config => config["errorUrl"]).Returns("/error");
            _mockConfigurations.Setup(config => config["awaitingUrl"]).Returns("/awaiting");
            _mockConfigurations.Setup(config => config["cancelUrl"]).Returns("/cancel");

            _mockSecurityUtils.Setup(utils => utils.GenerateIV()).Returns(fixedIv);
            _mockSecurityUtils.Setup(utils => utils.CbcEncryption(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>()))
                         .Returns((string data, string key, byte[] iv) => _realSecurityUtils.CbcEncryption(data, key, iv));
            _mockSecurityUtils.Setup(utils => utils.Sha256Hash(It.IsAny<string>()))
                              .Returns((string data) => _realSecurityUtils.Sha256Hash(data));

            _realPaymentService = new QUIXJSService
            {
                NetworkAdapter = _mockNetworkAdapter.Object,
                SecurityUtils = _mockSecurityUtils.Object
            };

            _realPaymentService.Credentials = new Credentials();
            _realPaymentService.Credentials.SetMerchantId(_mockConfigurations.Object["merchantId"]);
            _realPaymentService.Credentials.SetEnvironment(EnvironmentEnum.STAGING);
            _realPaymentService.Credentials.SetProductId(_mockConfigurations.Object["productId"]);
            _realPaymentService.Credentials.SetApiVersion(5);
            _realPaymentService.Credentials.SetMerchantKey(_mockConfigurations.Object["merchantKey"]);
        }




        [TestCategory("QuixJS")]
        [TestMethod]
        public async Task SendQuixServiceCharge()
        {
            // Arrange


            JSQuixService jsQuixService = new();
            jsQuixService.SetPrepayToken("eebbf0aa-3c88-40c1-845e-0cfea02ce9c1");
            jsQuixService.SetAmount("50");
            jsQuixService.SetCustomerId("55");
            jsQuixService.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            jsQuixService.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            jsQuixService.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            jsQuixService.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            jsQuixService.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            jsQuixService.SetCustomerEmail("test@mail.com");
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

            var tcs = new TaskCompletionSource<(Notification?, string?)>();
            var xmlContent = ReadXmlContent("javascript_quix.json");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpJSClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, string data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", xmlContent }, { "status_code", "200" } });


            // Act
            var result = await Task.Run(() =>
            {
                var responseListener = new Mock<IResponseListener>();
                responseListener.Setup(listener => listener.OnResponseReceived(It.IsAny<string>(), It.IsAny<Notification>(), It.IsAny<TransactionResult>()))
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendJSQuixServiceRequest(jsQuixService, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.JS_CHARGE_STG, capturedUrl);
            Assert.AreEqual(2, capturedHeaders.Count);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual(1, result.Item1.Operations?.OperationList?.Count);
            Assert.AreEqual("af24252b-e8c9-4fb2-9da2-7a476b2d8cd4", result.Item1.GetNemuruCartHash());
            Assert.AreEqual("62WBmZM44eDS2gZfVbgvEg5Cydea7IcY", result.Item1.GetNemuruAuthToken());
        }

        [TestCategory("QuixJS")]
        [TestMethod]
        public async Task SendQuixItemsCharge()
        {
            // Arrange


            JSQuixProduct jsQuixItem = new();
            jsQuixItem.SetPrepayToken("eebbf0aa-3c88-40c1-845e-0cfea02ce9c1");
            jsQuixItem.SetAmount("50");
            jsQuixItem.SetCustomerId("55");
            jsQuixItem.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            jsQuixItem.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            jsQuixItem.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            jsQuixItem.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            jsQuixItem.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
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
            quixAddress.SetPostalCode("08003");

            QuixBilling quixBilling = new();
            quixBilling.SetAddress(quixAddress);
            quixBilling.SetFirstName("Nombre");
            quixBilling.SetLastName("Apellido");

            QuixProductPaySolExtendedData quixItemPaySolExtendedData = new();
            quixItemPaySolExtendedData.SetCart(quixCartProduct);
            quixItemPaySolExtendedData.SetBilling(quixBilling);
            quixItemPaySolExtendedData.SetProduct("instalments");

            jsQuixItem.SetPaySolExtendedData(quixItemPaySolExtendedData);

            var tcs = new TaskCompletionSource<(Notification?, string?)>();
            var xmlContent = ReadXmlContent("javascript_quix.json");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpJSClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, string data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", xmlContent }, { "status_code", "200" } });


            // Act
            var result = await Task.Run(() =>
            {
                var responseListener = new Mock<IResponseListener>();
                responseListener.Setup(listener => listener.OnResponseReceived(It.IsAny<string>(), It.IsAny<Notification>(), It.IsAny<TransactionResult>()))
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendJSQuixProductRequest(jsQuixItem, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.JS_CHARGE_STG, capturedUrl);
            Assert.AreEqual(2, capturedHeaders.Count);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual(1, result.Item1.Operations?.OperationList?.Count);
            Assert.AreEqual("af24252b-e8c9-4fb2-9da2-7a476b2d8cd4", result.Item1.GetNemuruCartHash());
            Assert.AreEqual("62WBmZM44eDS2gZfVbgvEg5Cydea7IcY", result.Item1.GetNemuruAuthToken());
        }


        [TestCategory("QuixJS")]
        [TestMethod]
        public async Task SendQuixAccommodationCharge()
        {
            // Arrange


            JSQuixAccommodation jsQuixAccommodation = new();
            jsQuixAccommodation.SetPrepayToken("eebbf0aa-3c88-40c1-845e-0cfea02ce9c1");
            jsQuixAccommodation.SetAmount("50");
            jsQuixAccommodation.SetCustomerId("55");
            jsQuixAccommodation.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            jsQuixAccommodation.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            jsQuixAccommodation.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            jsQuixAccommodation.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            jsQuixAccommodation.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            jsQuixAccommodation.SetCustomerEmail("test@mail.com");
            jsQuixAccommodation.SetCustomerNationalId("99999999R");
            jsQuixAccommodation.SetDob("01-12-1999");
            jsQuixAccommodation.SetFirstName("Name");
            jsQuixAccommodation.SetLastName("Last Name");
            jsQuixAccommodation.SetIpAddress("0.0.0.0");

            QuixAddress quixAddress = new();
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

            QuixCartAccommodation quixCartAccommodation = new();
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

            var tcs = new TaskCompletionSource<(Notification?, string?)>();
            var xmlContent = ReadXmlContent("javascript_quix.json");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpJSClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, string data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", xmlContent }, { "status_code", "200" } });


            // Act
            var result = await Task.Run(() =>
            {
                var responseListener = new Mock<IResponseListener>();
                responseListener.Setup(listener => listener.OnResponseReceived(It.IsAny<string>(), It.IsAny<Notification>(), It.IsAny<TransactionResult>()))
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendJSQuixAccommodationRequest(jsQuixAccommodation, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.JS_CHARGE_STG, capturedUrl);
            Assert.AreEqual(2, capturedHeaders.Count);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual(1, result.Item1.Operations?.OperationList?.Count);
            Assert.AreEqual("af24252b-e8c9-4fb2-9da2-7a476b2d8cd4", result.Item1.GetNemuruCartHash());
            Assert.AreEqual("62WBmZM44eDS2gZfVbgvEg5Cydea7IcY", result.Item1.GetNemuruAuthToken());
        }

        [TestCategory("QuixJS")]
        [TestMethod]
        public async Task SendQuixFlightsCharge()
        {
            // Arrange


            JSQuixFlight jsQuixFlight = new();
            jsQuixFlight.SetPrepayToken("eebbf0aa-3c88-40c1-845e-0cfea02ce9c1");
            jsQuixFlight.SetAmount("50");
            jsQuixFlight.SetCustomerId("55");
            jsQuixFlight.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            jsQuixFlight.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            jsQuixFlight.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            jsQuixFlight.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            jsQuixFlight.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
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

            QuixItemCartItemFlight quixItemCartItemFlight = new();
            quixItemCartItemFlight.SetArticle(quixArticleFlight);
            quixItemCartItemFlight.SetUnits(1);
            quixItemCartItemFlight.SetAutoShipping(true);
            quixItemCartItemFlight.SetTotalPriceWithTax("50");

            List<QuixItemCartItemFlight> items = [];
            items.Add(quixItemCartItemFlight);

            QuixCartFlight quixCartFlight = new();
            quixCartFlight.SetCurrency(Currency.EUR);
            quixCartFlight.SetItems(items);
            quixCartFlight.SetTotalPriceWithTax("50");

            QuixAddress quixAddress = new();
            quixAddress.SetCity("Barcelona");
            quixAddress.SetCountry(CountryCodeAlpha3.ESP);
            quixAddress.SetStreetAddress("Nombre de la vía y nº");
            quixAddress.SetPostalCode("28003");

            QuixBilling quixBilling = new();
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

            var tcs = new TaskCompletionSource<(Notification?, string?)>();
            var xmlContent = ReadXmlContent("javascript_quix.json");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpJSClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, string data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", xmlContent }, { "status_code", "200" } });


            // Act
            var result = await Task.Run(() =>
            {
                var responseListener = new Mock<IResponseListener>();
                responseListener.Setup(listener => listener.OnResponseReceived(It.IsAny<string>(), It.IsAny<Notification>(), It.IsAny<TransactionResult>()))
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendJSQuixFlightRequest(jsQuixFlight, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.JS_CHARGE_STG, capturedUrl);
            Assert.AreEqual(2, capturedHeaders.Count);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual(1, result.Item1.Operations?.OperationList?.Count);
            Assert.AreEqual("af24252b-e8c9-4fb2-9da2-7a476b2d8cd4", result.Item1.GetNemuruCartHash());
            Assert.AreEqual("62WBmZM44eDS2gZfVbgvEg5Cydea7IcY", result.Item1.GetNemuruAuthToken());
        }

        [TestCategory("QuixJS")]
        [TestMethod]
        public async Task SendQuixServiceCharge_MissingStatusURL_ThrowsException()
        {
            // Arrange
            JSQuixService jsQuixService = new();
            jsQuixService.SetPrepayToken("eebbf0aa-3c88-40c1-845e-0cfea02ce9c1");
            jsQuixService.SetAmount("50");
            jsQuixService.SetCustomerId("55");
            // jsQuixService.SetStatusURL(_mockConfigurations.Object["statusUrl"]); // StatusURL is intentionally not set
            jsQuixService.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            jsQuixService.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            jsQuixService.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            jsQuixService.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            jsQuixService.SetCustomerEmail("test@mail.com");
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

            List<QuixItemCartItemService> items = new();
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

            var tcs = new TaskCompletionSource<(Notification?, string?)>();
            var xmlContent = ReadXmlContent("javascript_quix.json");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpJSClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, string data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", xmlContent }, { "status_code", "200" } });

            // Act & Assert
            Exception exception = null;
            try
            {
                var responseListener = new Mock<IResponseListener>();
                responseListener.Setup(listener => listener.OnResponseReceived(It.IsAny<string>(), It.IsAny<Notification>(), It.IsAny<TransactionResult>()))
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                await _realPaymentService.SendJSQuixServiceRequest(jsQuixService, responseListener.Object);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsNotNull(exception, "Expected exception was not thrown.");
            Assert.IsInstanceOfType(exception, typeof(MissingFieldsException));
            Assert.AreEqual("Missing statusURL", exception.Message);
        }



    }
}
