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
using DotNetPaymentSDK.src.Exceptions;

namespace DotNetPaymentSDK.Tests
{
    [TestClass]
    public class HostedServiceTests
    {
        private Mock<IConfiguration> _mockConfigurations;
        private IHostedService _realPaymentService;
        private Mock<INetworkAdapter> _mockNetworkAdapter;
        private Mock<ISecurityUtils> _mockSecurityUtils;
        private ISecurityUtils _realSecurityUtils;
        private string capturedUrl;
        private Dictionary<string, string> capturedData;
        private Dictionary<string, string> capturedHeaders;



        private readonly byte[] fixedIv = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 };

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

            _realPaymentService = new HostedService
            {
                NetworkAdapter = _mockNetworkAdapter.Object,
                SecurityUtils = _mockSecurityUtils.Object
            };

            _realPaymentService.Credentials = new Credentials();
            _realPaymentService.Credentials.SetMerchantId(_mockConfigurations.Object["merchantId"]);
            _realPaymentService.Credentials.SetMerchantPass(_mockConfigurations.Object["merchantPassword"]);
            _realPaymentService.Credentials.SetEnvironment(EnvironmentEnum.STAGING);
            _realPaymentService.Credentials.SetProductId(_mockConfigurations.Object["productId"]);
            _realPaymentService.Credentials.SetApiVersion(5);
        }

        [TestCategory("Hosted")]
        [TestMethod]
        public async Task SendHosted_ReturnsRedirectUrl_OnSuccess()
        {
            // Arrange
            var hostedPaymentRedirection = new HostedPaymentRedirection();
            hostedPaymentRedirection.SetAmount("50");
            hostedPaymentRedirection.SetCurrency(Currency.EUR);
            hostedPaymentRedirection.SetCountry(CountryCodeAlpha2.ES);
            hostedPaymentRedirection.SetCustomerId("903");
            hostedPaymentRedirection.SetMerchantTransactionId("3123123");
            hostedPaymentRedirection.SetPaymentSolution(PaymentSolutions.creditcards);
            hostedPaymentRedirection.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            hostedPaymentRedirection.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            hostedPaymentRedirection.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            hostedPaymentRedirection.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            hostedPaymentRedirection.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            hostedPaymentRedirection.SetForceTokenRequest(false);

            var tcs = new TaskCompletionSource<(string?, string?)>();

            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", "http://redirect.url" }, { "status_code", "200" } });


            // Act
            var result = await Task.Run(() =>
            {
                var responseListener = new Mock<IResponseListener>();
                responseListener.Setup(listener => listener.OnRedirectionURLReceived(It.IsAny<string>()))
                                .Callback<string>(url => tcs.SetResult((url, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendHostedPaymentRequest(hostedPaymentRedirection, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual("https://checkout-stg.addonpayments.com/EPGCheckout/rest/online/tokenize", capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "ir+VKs1qIMTtMKsk/6Iv9V0ciD2CItEkjqeWo0jY6dpphfK+BPGVB+rNNHCXIs0Shab/1B8xjiKtSOJqwNE8HiAj9AutqKFtJ0WLrEfxzO/bnzXiYfKZMrYErpI/wdKKY7j8kC/i7C12gyQ4pKO3j4gvZFUCduIlnhPuoWS5xKD9UR9CSftgwkoR+4n97km7rc48LaO77haCKl1HPHhdjGU/XRSNpRnP3d/r7L5QK57V+7jlsBD42cv5KZBqAH/8q7aRoYwtcLA1dts9qCqS5bK4zC1PP8aqydltTmdcY7LjUUorUoYumeoxWGhbaWlG8/ksGmvMGFWOAIm22edL7bBjYUT+ODD349cU8VTNVWq7li/9EgWUB7xOm5I64UrivyJgWBdRJ439aK9TBEn6UZQny9a/7KdYgELFgWLPI5NlFfO/ocrsAPZXA7rZLyXA84lURzRS9Cvzxe4ZXEJzqBcZosliwpOqrBXlicmyPzKmlmYsXvy6saiskN4cu3VuXvaLG/t+7CHVYYFuIJxEnIh1HzmsJf+GexZvys626JTHd0Z//usf3D8Slgf1EewejVy2Ew+jtvKmAvxMkF94PX62pNfZK56VMn/J0sZTVL6U9OWtzz2ckGzvwBJcxGjW",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "b80604005a181ffc146e2fe95e15e87d44c18ef0b8248197fd7b31e56851ad0e",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("http://redirect.url", result.Item1);
        }


        [TestCategory("Hosted")]
        [TestMethod]
        public async Task SendHostedRecurrentInitial_ReturnsRedirectUrl_OnSuccess()
        {
            // Arrange
            var hostedPaymentRedirection = new HostedPaymentRecurrentInitial();
            hostedPaymentRedirection.SetAmount("50");
            hostedPaymentRedirection.SetCurrency(Currency.EUR);
            hostedPaymentRedirection.SetCountry(CountryCodeAlpha2.ES);
            hostedPaymentRedirection.SetCustomerId("903");
            hostedPaymentRedirection.SetPaymentRecurringType(PaymentRecurringType.newCof);
            hostedPaymentRedirection.SetChallengeInd(ChallengeIndEnum._04);
            hostedPaymentRedirection.SetMerchantTransactionId("3123123");
            hostedPaymentRedirection.SetPaymentSolution(PaymentSolutions.creditcards);
            hostedPaymentRedirection.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            hostedPaymentRedirection.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            hostedPaymentRedirection.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            hostedPaymentRedirection.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            hostedPaymentRedirection.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            hostedPaymentRedirection.SetForceTokenRequest(false);

            var tcs = new TaskCompletionSource<(string?, string?)>();

            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", "http://redirect.url" }, { "status_code", "200" } });


            // Act
            var result = await Task.Run(() =>
            {
                var responseListener = new Mock<IResponseListener>();
                responseListener.Setup(listener => listener.OnRedirectionURLReceived(It.IsAny<string>()))
                                .Callback<string>(url => tcs.SetResult((url, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendHostedRecurrentInitial(hostedPaymentRedirection, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual("https://checkout-stg.addonpayments.com/EPGCheckout/rest/online/tokenize", capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "m//xJAl85GXjjlaL/ynPqXfV2tsQ6lq5uUSOLO6MsK53yfekgOlLtsvg8prGkqihWR0GDa3jIfndhckoDuXcO8F6kP9rau/vlYxlNC4lLJMk5gUVoXnaily/jwToRt++ino5XBmxvkzJI4Tv1WNddxHRA3LZCBhv14cfvP4tALBM+6Eh6vhApGSkz/d24s57oJ09PRrsxb+jn4Ke7Q+QzZToUn/3/PCosQCDyyCzdCIyFpnco/76b+TZFNHNSQbZgCf9BsYV42PX7NnW3nStuehIyXt56lfllNAjzLw8Wlbo8oyijl/e1vUBsVFGZXr8aRqpr1RzRpmn9P7RqL77NOmfXdFUpIn12sf+Rudjy50h4oJ2d6m5UQ5Q+xe+/oA91KQCkIIO0NeYmbr8CaQcQebWGto6y09r3iFc9LpucraO/iqUfQy/6N/qVSc+4N1hva6RRpmHiR/eRW2euF0M7HrXTaf/RnhKW2lqXr98GJZgXShRQc2koFlevB1PZVp6HUGs2N79bkKFTJNUYPu+BOEsXthY0p9PGSekzLRd33OTvuJggqOmO2hMTpr2E8Pq7YtDQzEiEKwieHThWXE67cNmGwXV8cwlnlpbgxMOS5mAUjWKc1P5qJ9F02lMdinr97kZq26M/8QPASMdOe2DEvzRC7FTOta0En6beedmrx0=",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "be32de3db1337695554af59891009e77c56ba4c1419053efa0b69bfc75a5f3b8",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("http://redirect.url", result.Item1);
        }


        [TestCategory("Hosted")]
        [TestMethod]
        public async Task SendHosted_MissingStatusURL_ThrowsException()
        {
            // Arrange
            var hostedPaymentRedirection = new HostedPaymentRedirection();
            hostedPaymentRedirection.SetAmount("50");
            hostedPaymentRedirection.SetCurrency(Currency.EUR);
            hostedPaymentRedirection.SetCountry(CountryCodeAlpha2.ES);
            hostedPaymentRedirection.SetCustomerId("903");
            hostedPaymentRedirection.SetMerchantTransactionId("3123123");
            hostedPaymentRedirection.SetPaymentSolution(PaymentSolutions.creditcards);
            // hostedPaymentRedirection.SetStatusURL(_mockConfigurations.Object["statusUrl"]); // StatusURL is intentionally not set
            hostedPaymentRedirection.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            hostedPaymentRedirection.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            hostedPaymentRedirection.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            hostedPaymentRedirection.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            hostedPaymentRedirection.SetForceTokenRequest(false);

            var tcs = new TaskCompletionSource<(string?, string?)>();

            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
                       {
                           capturedUrl = url;
                           capturedData = data;
                           capturedHeaders = headers;
                       })
                       .ReturnsAsync(new Dictionary<string, string> { { "response", "http://redirect.url" }, { "status_code", "200" } });

            // Act & Assert
            Exception exception = null;
            try
            {
                var responseListener = new Mock<IResponseListener>();
                responseListener.Setup(listener => listener.OnRedirectionURLReceived(It.IsAny<string>()))
                                .Callback<string>(url => tcs.SetResult((url, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                await _realPaymentService.SendHostedPaymentRequest(hostedPaymentRedirection, responseListener.Object);
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
