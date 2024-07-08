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
using DotNetPaymentSDK.src.Parameters.Nottification;
using DotNetPaymentSDK.src.Exceptions;

namespace DotNetPaymentSDK.Tests
{
    [TestClass]
    public class JavascriptTests
    {
        private Mock<IConfiguration> _mockConfigurations;
        private IJSService _realPaymentService;
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

            _realPaymentService = new JSService
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

        [TestCategory("JS")]
        [TestMethod]
        public async Task SendAuth()
        {
            // Arrange


            JSAuthorizationRequestParameters jSAuthPaymentParameters = new();
            jSAuthPaymentParameters.SetCustomerId("8881");
            jSAuthPaymentParameters.SetCurrency(Currency.EUR);
            jSAuthPaymentParameters.SetCountry(CountryCodeAlpha2.ES);
            jSAuthPaymentParameters.SetOperationType(OperationTypes.DEBIT);

            var tcs = new TaskCompletionSource<(string?, string?)>();

            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpJSClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, string data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", "{\"authToken\":\"96d1e757-b3ba-474a-a4da-9c771585fbe5\"}" }, { "status_code", "200" } });


            // Act
            var result = await Task.Run(() =>
            {
                var responseListener = new Mock<IJSPaymentListener>();
                responseListener.Setup(listener => listener.OnAuthorizationResponseReceived(It.IsAny<string>(), It.IsAny<JSAuthorizationResponse>()))
                                .Callback<string, JSAuthorizationResponse>((url, jsAuthorizationResponse) => tcs.SetResult((jsAuthorizationResponse.AuthToken, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendJSAuthorizationRequest(jSAuthPaymentParameters, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.JS_AUTH_STG, capturedUrl);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("96d1e757-b3ba-474a-a4da-9c771585fbe5", result.Item1);
        }

        [TestCategory("JS")]
        [TestMethod]
        public async Task SendCharge()
        {
            // Arrange


            JSChargeParameters jSChargeParameters = new JSChargeParameters();
            jSChargeParameters.SetAmount("50");
            jSChargeParameters.SetCustomerId("8881");
            jSChargeParameters.SetCurrency(Currency.EUR);
            jSChargeParameters.SetCountry(CountryCodeAlpha2.ES);
            jSChargeParameters.SetOperationType(OperationTypes.DEBIT);
            jSChargeParameters.SetPaymentSolution(PaymentSolutions.creditcards);
            jSChargeParameters.SetPrepayToken("eebbf0aa-3c88-40c1-845e-0cfea02ce9c1");
            jSChargeParameters.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            jSChargeParameters.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            jSChargeParameters.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            jSChargeParameters.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            jSChargeParameters.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("javascript_charge.json");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpChargeClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
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
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification.GetRedirectUrl(), null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendJSChargeRequest(jSChargeParameters, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.JS_CHARGE_STG, capturedUrl);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("https://checkout.stg-eu-west3.epgint.com/EPGCheckout/rest/online/3dsv2/redirect?action=gatherdevice&params=eyJ0aHJlZURTdjJUb2tlbiI6ImNjNWM1ODJjLWJiOTgtNGIyNS04NjA5LTZkZDI2YzM3MDgwNSIsInRocmVlRFNNZXRob2RVcmwiOiJodHRwczovL21vY2stZHMuc3RnLWV1LXdlc3QzLmVwZ2ludC5jb20vcHVibGljL21ldGhvZC1kYXRhLyIsInRocmVlRFNNZXRob2REYXRhIjoiZXlKMGFISmxaVVJUVTJWeWRtVnlWSEpoYm5OSlJDSTZJbU5qTldNMU9ESmpMV0ppT1RndE5HSXlOUzA0TmpBNUxUWmtaREkyWXpNM01EZ3dOU0lzSUNKMGFISmxaVVJUVFdWMGFHOWtUbTkwYVdacFkyRjBhVzl1VlZKTUlqb2dJbWgwZEhCek9pOHZZMmhsWTJ0dmRYUXVjM1JuTFdWMUxYZGxjM1F6TG1Wd1oybHVkQzVqYjIwdlJWQkhRMmhsWTJ0dmRYUXZZMkZzYkdKaFkyc3ZaMkYwYUdWeVJHVjJhV05sVG05MGFXWnBZMkYwYVc5dUwzQmhlWE52YkM4elpITjJNaTh4TURJek1ESTJJbjA9IiwiYnJhbmQiOiJWSVNBIiwicmVzdW1lQXV0aGVudGljYXRpb24iOiJodHRwczovL2NoZWNrb3V0LnN0Zy1ldS13ZXN0My5lcGdpbnQuY29tL0VQR0NoZWNrb3V0L3JldHVybnVybC9mcmljdGlvbmxlc3MvcGF5c29sLzNkc3YyLzEwMjMwMjY/dGhyZWVEU3YyVG9rZW49Y2M1YzU4MmMtYmI5OC00YjI1LTg2MDktNmRkMjZjMzcwODA1IiwicmVuZGVyQ2FzaGllckxvY2F0aW9uIjoiaHR0cHM6Ly9lcGdqcy1yZW5kZXJjYXNoaWVyLXN0Zy5lYXN5cGF5bWVudGdhdGV3YXkuY29tIiwiY2hhbGxlbmdlV2luZG93c1NpemUiOiIwNSJ9", result.Item1);
        }

        [TestCategory("JS")]
        [TestMethod]
        public async Task SendChargeRecurring()
        {
            // Arrange


            JSPaymentRecurrentInitial jsPaymentRecurrentInitial = new JSPaymentRecurrentInitial();
            jsPaymentRecurrentInitial.SetAmount("50");
            jsPaymentRecurrentInitial.SetCustomerId("8881");
            jsPaymentRecurrentInitial.SetCurrency(Currency.EUR);
            jsPaymentRecurrentInitial.SetCountry(CountryCodeAlpha2.ES);
            jsPaymentRecurrentInitial.SetPaymentRecurringType(PaymentRecurringType.newCof);
            jsPaymentRecurrentInitial.SetChallengeInd(ChallengeIndEnum._04);
            jsPaymentRecurrentInitial.SetOperationType(OperationTypes.DEBIT);
            jsPaymentRecurrentInitial.SetPaymentSolution(PaymentSolutions.creditcards);
            jsPaymentRecurrentInitial.SetPrepayToken("eebbf0aa-3c88-40c1-845e-0cfea02ce9c1");
            jsPaymentRecurrentInitial.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            jsPaymentRecurrentInitial.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            jsPaymentRecurrentInitial.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            jsPaymentRecurrentInitial.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            jsPaymentRecurrentInitial.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("javascript_charge.json");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpChargeClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
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
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification.GetRedirectUrl(), null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendJSChargeRequest(jsPaymentRecurrentInitial, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.JS_CHARGE_STG, capturedUrl);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("https://checkout.stg-eu-west3.epgint.com/EPGCheckout/rest/online/3dsv2/redirect?action=gatherdevice&params=eyJ0aHJlZURTdjJUb2tlbiI6ImNjNWM1ODJjLWJiOTgtNGIyNS04NjA5LTZkZDI2YzM3MDgwNSIsInRocmVlRFNNZXRob2RVcmwiOiJodHRwczovL21vY2stZHMuc3RnLWV1LXdlc3QzLmVwZ2ludC5jb20vcHVibGljL21ldGhvZC1kYXRhLyIsInRocmVlRFNNZXRob2REYXRhIjoiZXlKMGFISmxaVVJUVTJWeWRtVnlWSEpoYm5OSlJDSTZJbU5qTldNMU9ESmpMV0ppT1RndE5HSXlOUzA0TmpBNUxUWmtaREkyWXpNM01EZ3dOU0lzSUNKMGFISmxaVVJUVFdWMGFHOWtUbTkwYVdacFkyRjBhVzl1VlZKTUlqb2dJbWgwZEhCek9pOHZZMmhsWTJ0dmRYUXVjM1JuTFdWMUxYZGxjM1F6TG1Wd1oybHVkQzVqYjIwdlJWQkhRMmhsWTJ0dmRYUXZZMkZzYkdKaFkyc3ZaMkYwYUdWeVJHVjJhV05sVG05MGFXWnBZMkYwYVc5dUwzQmhlWE52YkM4elpITjJNaTh4TURJek1ESTJJbjA9IiwiYnJhbmQiOiJWSVNBIiwicmVzdW1lQXV0aGVudGljYXRpb24iOiJodHRwczovL2NoZWNrb3V0LnN0Zy1ldS13ZXN0My5lcGdpbnQuY29tL0VQR0NoZWNrb3V0L3JldHVybnVybC9mcmljdGlvbmxlc3MvcGF5c29sLzNkc3YyLzEwMjMwMjY/dGhyZWVEU3YyVG9rZW49Y2M1YzU4MmMtYmI5OC00YjI1LTg2MDktNmRkMjZjMzcwODA1IiwicmVuZGVyQ2FzaGllckxvY2F0aW9uIjoiaHR0cHM6Ly9lcGdqcy1yZW5kZXJjYXNoaWVyLXN0Zy5lYXN5cGF5bWVudGdhdGV3YXkuY29tIiwiY2hhbGxlbmdlV2luZG93c1NpemUiOiIwNSJ9", result.Item1);
        }


        [TestCategory("JS")]
        [TestMethod]
        public async Task SendAuth_MissingCustomerId_ThrowsException()
        {
            // Arrange
            JSAuthorizationRequestParameters jSAuthPaymentParameters = new();
            // jSAuthPaymentParameters.SetCustomerId("8881"); // CustomerId is intentionally not set
            jSAuthPaymentParameters.SetCurrency(Currency.EUR);
            jSAuthPaymentParameters.SetCountry(CountryCodeAlpha2.ES);
            jSAuthPaymentParameters.SetOperationType(OperationTypes.DEBIT);

            var tcs = new TaskCompletionSource<(string?, string?)>();

            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpJSClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, string data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", "{\"authToken\":\"96d1e757-b3ba-474a-a4da-9c771585fbe5\"}" }, { "status_code", "200" } });

            // Act & Assert
            Exception exception = null;
            try
            {
                var responseListener = new Mock<IJSPaymentListener>();
                responseListener.Setup(listener => listener.OnAuthorizationResponseReceived(It.IsAny<string>(), It.IsAny<JSAuthorizationResponse>()))
                                .Callback<string, JSAuthorizationResponse>((url, jsAuthorizationResponse) => tcs.SetResult((jsAuthorizationResponse.AuthToken, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                await _realPaymentService.SendJSAuthorizationRequest(jSAuthPaymentParameters, responseListener.Object);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsNotNull(exception, "Expected exception was not thrown.");
            Assert.IsInstanceOfType(exception, typeof(MissingFieldsException));
            Assert.AreEqual("Missing customerId", exception.Message);
        }


        [TestCategory("JS")]
        [TestMethod]
        public async Task SendCharge_MissingStatusURL_ThrowsException()
        {
            // Arrange
            JSChargeParameters jSChargeParameters = new JSChargeParameters();
            jSChargeParameters.SetAmount("50");
            jSChargeParameters.SetCustomerId("8881");
            jSChargeParameters.SetCurrency(Currency.EUR);
            jSChargeParameters.SetCountry(CountryCodeAlpha2.ES);
            jSChargeParameters.SetOperationType(OperationTypes.DEBIT);
            jSChargeParameters.SetPaymentSolution(PaymentSolutions.creditcards);
            jSChargeParameters.SetPrepayToken("eebbf0aa-3c88-40c1-845e-0cfea02ce9c1");
            // jSChargeParameters.SetStatusURL(_mockConfigurations.Object["statusUrl"]); // StatusURL is intentionally not set
            jSChargeParameters.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            jSChargeParameters.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            jSChargeParameters.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            jSChargeParameters.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("javascript_charge.json");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpChargeClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
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
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification.GetRedirectUrl(), null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                await _realPaymentService.SendJSChargeRequest(jSChargeParameters, responseListener.Object);
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
