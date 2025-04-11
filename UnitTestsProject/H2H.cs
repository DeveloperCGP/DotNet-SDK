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
using DotNetPaymentSDK.src.Parameters.H2H;
using DotNetPaymentSDK.src.Requests.Utils;
using DotNetPaymentSDK.src.Parameters.Notification;
using DotNetPaymentSDK.src.Exceptions;

namespace DotNetPaymentSDK.Tests
{
    [TestClass]
    public class H2HTests
    {
        private Mock<IConfiguration> _mockConfigurations;
        private IH2HService _realPaymentService;
        private Mock<INetworkAdapter> _mockNetworkAdapter;
        private Mock<ISecurityUtils> _mockSecurityUtils;
        private ISecurityUtils _realSecurityUtils;
        private string capturedUrl;
        private Dictionary<string, string> capturedData;
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

            _realPaymentService = new H2HService
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

        [TestCategory("H2H")]
        [TestMethod]
        public async Task SendH2H()
        {
            // Arrange
            H2HRedirectionParameters h2HRedirection = new();
            h2HRedirection.SetAmount("50");
            h2HRedirection.SetCurrency(Currency.EUR);
            h2HRedirection.SetCountry(CountryCodeAlpha2.ES);
            h2HRedirection.SetCustomerId("903");
            h2HRedirection.SetCardNumber("4907270002222227");
            h2HRedirection.SetMerchantTransactionId("11111111");
            h2HRedirection.SetChName("Pablo Navarrow");
            h2HRedirection.SetCvnNumber("123");
            h2HRedirection.SetExpDate("0230");
            h2HRedirection.SetPaymentSolution(PaymentSolutions.creditcards);
            h2HRedirection.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            h2HRedirection.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            h2HRedirection.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            h2HRedirection.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            h2HRedirection.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            h2HRedirection.SetForceTokenRequest(false);

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("h2h_response.xml");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
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

                _realPaymentService.SendH2HRedirectionPaymentRequest(h2HRedirection, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.H2H_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "2mj81PHCaggkLNpl/1sOjGoKCnwXCVMBd1jWlkyxVx3sjsZZNTd/sVcozi0BzsK49AtAqN5iCYEME2fltryuNRp10/LvYyjustNOmkEvqGPbgMruMGuywTrXxzXnM5sPPvN/FXjqNjERnee0DQQfRXtMLzUJPinZ1D0VlLVllfjuMQl4KdFJGTRanmqMstVcCWtVW+6/RDPl18nLgCEpzKAYA6N+UOTnmey1b1qnPj4Ue9zK1TgX2BTm7Qf338vqIIJFrs/PkzAlbzeXHDzhEt45k79pmJBDMLLiA3i/kEz9k59Ha/g8w7wCKvi8dxfvGXOeSKGFUD0lfZb9zenlwikrGSmaxtuuKU9LaZS+1k9moy5XxyAu5CjFa5uhFXgFkj5R4F/y7V8a5t1a2rUpS6fkNx52/rQryyF6PLRm0BSrTJRs6kD0z89GyH/eOBJk6RCScqUcpn+8NaGl/JmYHEepPh4akKKDY02zFgVNK1mtu4P2LeKPra4PusEURPp9yFZs7YR3D+5tlbzJXh8zXxH5fr6q6LK3uBHArgQkHdRx2kzEMZ9Hr7PBL6I2mRP9h9KMcqq5V74iMFVIR3R6PYGntOQDsowCX7UAAn7wt+Y8WlEhZpLiig3ktDWWZWLRDGvfG8yNXaLPMCzeE7d09oUggBBuzxkLMa4dmPDQLV8=",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "29927caf50f8cc67a54484a30c80c36fa7556d521245f51f5925838b39bc8d82",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("https://checkout.stg-eu-west3.epgint.com/EPGCheckout/rest/online/3dsv2/redirect?action=gatherdevice&params=eyJ0aHJlZURTdjJUb2tlbiI6ImQ1NmEzNTVlLTQ4NjktNDFhNy1iOTRiLTAxNWMwYzVkZTIwNiIsInRocmVlRFNNZXRob2RVcmwiOiJodHRwczovL21vY2stZHMuc3RnLWV1LXdlc3QzLmVwZ2ludC5jb20vcHVibGljL21ldGhvZC1kYXRhLyIsInRocmVlRFNNZXRob2REYXRhIjoiZXlKMGFISmxaVVJUVTJWeWRtVnlWSEpoYm5OSlJDSTZJbVExTm1Fek5UVmxMVFE0TmprdE5ERmhOeTFpT1RSaUxUQXhOV013WXpWa1pUSXdOaUlzSUNKMGFISmxaVVJUVFdWMGFHOWtUbTkwYVdacFkyRjBhVzl1VlZKTUlqb2dJbWgwZEhCek9pOHZZMmhsWTJ0dmRYUXVjM1JuTFdWMUxYZGxjM1F6TG1Wd1oybHVkQzVqYjIwdlJWQkhRMmhsWTJ0dmRYUXZZMkZzYkdKaFkyc3ZaMkYwYUdWeVJHVjJhV05sVG05MGFXWnBZMkYwYVc5dUwzQmhlWE52YkM4elpITjJNaTh4TURJeE5qTTJJbjA9IiwiYnJhbmQiOm51bGwsInJlc3VtZUF1dGhlbnRpY2F0aW9uIjoiaHR0cHM6Ly9jaGVja291dC5zdGctZXUtd2VzdDMuZXBnaW50LmNvbS9FUEdDaGVja291dC9yZXR1cm51cmwvZnJpY3Rpb25sZXNzL3BheXNvbC8zZHN2Mi8xMDIxNjM2P3RocmVlRFN2MlRva2VuPWQ1NmEzNTVlLTQ4NjktNDFhNy1iOTRiLTAxNWMwYzVkZTIwNiIsInJlbmRlckNhc2hpZXJMb2NhdGlvbiI6Imh0dHBzOi8vZXBnanMtcmVuZGVyY2FzaGllci1zdGcuZWFzeXBheW1lbnRnYXRld2F5LmNvbSIsImNoYWxsZW5nZVdpbmRvd3NTaXplIjoiMDUifQ==", result.Item1);
        }








        [TestCategory("H2H")]
        [TestMethod]
        public async Task SendH2HPaymentRecurringRequest()
        {
            // Arrange
            H2HPaymentRecurrentInitialParameters h2hPaymentRecurrentInitialParameters = new();
            h2hPaymentRecurrentInitialParameters.SetAmount("50");
            h2hPaymentRecurrentInitialParameters.SetCurrency(Currency.EUR);
            h2hPaymentRecurrentInitialParameters.SetCountry(CountryCodeAlpha2.ES);
            h2hPaymentRecurrentInitialParameters.SetCustomerId("903");
            h2hPaymentRecurrentInitialParameters.SetCardNumber("4907270002222227");
            h2hPaymentRecurrentInitialParameters.SetMerchantTransactionId("11111111");
            h2hPaymentRecurrentInitialParameters.SetChName("Pablo Navarrow");
            h2hPaymentRecurrentInitialParameters.SetCvnNumber("123");
            h2hPaymentRecurrentInitialParameters.SetExpDate("0230");
            h2hPaymentRecurrentInitialParameters.SetPaymentRecurringType(PaymentRecurringType.newCof);
            h2hPaymentRecurrentInitialParameters.SetPaymentSolution(PaymentSolutions.creditcards);
            h2hPaymentRecurrentInitialParameters.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            h2hPaymentRecurrentInitialParameters.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            h2hPaymentRecurrentInitialParameters.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            h2hPaymentRecurrentInitialParameters.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            h2hPaymentRecurrentInitialParameters.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("h2h_recurring.xml");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
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

                _realPaymentService.SendH2hPaymentRecurrentInitial(h2hPaymentRecurrentInitialParameters, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.H2H_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "m//xJAl85GXjjlaL/ynPqXfV2tsQ6lq5uUSOLO6MsK7LgC1X+rqeiGfRZJ9UDLnvHzjPqrUV8H6s/lH3qTcd/r9bqnxpachlM1Zz/J+MDC4ylePXZSJI4R8YiuoRpZtFx+h4kuMITy0yKhBCy55fPuC34jygatKcIrTtR4PvDFIAlw6uVI7PMp5PrYNN5L0fAQ9tANKsjX8r6AqFxIK44W/mAR5fds2TWRBNvO9blciyZDY2iVIr5X2xM/yjY03NbfFg8OqVoRAyy2rFELXx3Zl2YoZAtoSQecymvq+sXwr+t62hFo7202rTclBAsU/kvF6XCyrWZoQN/AkVNRbY84VpHDBkqTMs8lVPUnmoKoZ7iOUJVqUCzmQewqN5Rv1vgf42Y3fDaOk78i6SgcVIgvAWLX1bsVURyFG4+dnTekhEbV8Q/UOITvYKV5kiOyycxCoH1NdpENis5VeNeXNv692rxuBkJVNBP4Qfq/VIQkbqrHtzUsyyuy/FWdtgcX9u+v+rWfVRuVpi0xQo6pnRD83w/BKqXRc5dJc/Qjt9ZzhI6ZOuKSs2AfJy2TuVaXSlq2HYq2P0xh2HDsGuZ4Sp7gCO7nvGvi0soIRhD3M1yAbkAIcSoak4sqe9YgrW58Qk6nOP1Z6e5tAGEJxE/Z8EcW/nNgBelsmNCzWVtm1R+CrkIVYafFrY+krxQ7+ttAOETfyMfPKgGquCibt++2Wg1GdE9bDQ5VueoYakezvNIUo=",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "e48e1469b17cd6972881da79a7ef930b36e95dad986956c48fc8130e8ff3cac3",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("https://checkout.stg-eu-west3.epgint.com/EPGCheckout/rest/online/3dsv2/redirect?action=gatherdevice&params=eyJ0aHJlZURTdjJUb2tlbiI6IjM2ODU5NGM2LTUzMDItNDlhOC04NThhLTFkNTgwOTM3N2Q0YSIsInRocmVlRFNNZXRob2RVcmwiOiJodHRwczovL21vY2stZHMuc3RnLWV1LXdlc3QzLmVwZ2ludC5jb20vcHVibGljL21ldGhvZC1kYXRhLyIsInRocmVlRFNNZXRob2REYXRhIjoiZXlKMGFISmxaVVJUVTJWeWRtVnlWSEpoYm5OSlJDSTZJak0yT0RVNU5HTTJMVFV6TURJdE5EbGhPQzA0TlRoaExURmtOVGd3T1RNM04yUTBZU0lzSUNKMGFISmxaVVJUVFdWMGFHOWtUbTkwYVdacFkyRjBhVzl1VlZKTUlqb2dJbWgwZEhCek9pOHZZMmhsWTJ0dmRYUXVjM1JuTFdWMUxYZGxjM1F6TG1Wd1oybHVkQzVqYjIwdlJWQkhRMmhsWTJ0dmRYUXZZMkZzYkdKaFkyc3ZaMkYwYUdWeVJHVjJhV05sVG05MGFXWnBZMkYwYVc5dUwzQmhlWE52YkM4elpITjJNaTh4TURJeE9EZzRJbjA9IiwiYnJhbmQiOm51bGwsInJlc3VtZUF1dGhlbnRpY2F0aW9uIjoiaHR0cHM6Ly9jaGVja291dC5zdGctZXUtd2VzdDMuZXBnaW50LmNvbS9FUEdDaGVja291dC9yZXR1cm51cmwvZnJpY3Rpb25sZXNzL3BheXNvbC8zZHN2Mi8xMDIxODg4P3RocmVlRFN2MlRva2VuPTM2ODU5NGM2LTUzMDItNDlhOC04NThhLTFkNTgwOTM3N2Q0YSIsInJlbmRlckNhc2hpZXJMb2NhdGlvbiI6Imh0dHBzOi8vZXBnanMtcmVuZGVyY2FzaGllci1zdGcuZWFzeXBheW1lbnRnYXRld2F5LmNvbSIsImNoYWxsZW5nZVdpbmRvd3NTaXplIjoiMDUifQ==", result.Item1);
        }

        [TestCategory("H2H")]
        [TestMethod]
        public async Task SendH2HPaymentRecurringSubsuqentRequest()
        {
            // Arrange
            H2HPaymentRecurrentSuccessiveParameters h2hPaymentRecurrentSuccessiveParameters = new();
            h2hPaymentRecurrentSuccessiveParameters.SetSubscriptionPlan("333311111111");
            h2hPaymentRecurrentSuccessiveParameters.SetPaymentRecurringType(PaymentRecurringType.cof);
            h2hPaymentRecurrentSuccessiveParameters.SetMerchantExemptionsSca(MerchantExemptionsScaEnum.MIT);
            h2hPaymentRecurrentSuccessiveParameters.SetCardNumberToken("33333331111127466");
            h2hPaymentRecurrentSuccessiveParameters.SetMerchantTransactionId("664448844");
            h2hPaymentRecurrentSuccessiveParameters.SetAmount("50");
            h2hPaymentRecurrentSuccessiveParameters.SetCountry(CountryCodeAlpha2.ES);
            h2hPaymentRecurrentSuccessiveParameters.SetCurrency(Currency.EUR);
            h2hPaymentRecurrentSuccessiveParameters.SetCustomerId("903");
            h2hPaymentRecurrentSuccessiveParameters.SetChName("First name Last name");
            h2hPaymentRecurrentSuccessiveParameters.SetPaymentSolution(PaymentSolutions.creditcards);
            h2hPaymentRecurrentSuccessiveParameters.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            h2hPaymentRecurrentSuccessiveParameters.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            h2hPaymentRecurrentSuccessiveParameters.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            h2hPaymentRecurrentSuccessiveParameters.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            h2hPaymentRecurrentSuccessiveParameters.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            h2hPaymentRecurrentSuccessiveParameters.SetForceTokenRequest(false);

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("h2h_recurring_subsuqent.xml");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
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
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification.Status, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendH2hPaymentRecurrentSuccessive(h2hPaymentRecurrentSuccessiveParameters, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.H2H_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "YyFpY244Ve+Ee0OlEfSpsCesal1+gF2EL++2TbqtAXB93BtfCc22k+Zg9vH7MitY2v3YmsPlH3AJrlUqBc05TLvHZhNqut9Kbe/eUN93uF/K4lXtCVVK9Rhj8TJwyLvXngtx0g5ef9YPjiofwNMlwEyZbEoLASGLv99PRqGF2YVuxJSB61FwOH94nLTPdFb8DdERegNgSOowTNcK7YiCscIfhDL5yYfO7h6KouIgRT3KGN5bwPftwNxHCmPPNbrv9RzpwbNrYayfYnOCPKrJpUjQMsdzbu4KR8UN5h5qYQdS7jxyDC9/bF8I/Sq3dvaBaK+tqsnbp9P91NMHSoDbTANkRJpEORbYI20ZvKDo74JiRpSD4en4qk80JQQWIY5XU8n8zzTgYa3DCrzPPzDJgXkV3bW0+icPIWP/6f9G+8D6j42m1lvTWTc9weQ71Bv7wQsqf9mNiS0xuzFoH11XJY1HmzFjckdXFekqdq7JFpzFq1ZLCV1SeUezzCr1nb7vAiFMgqgvj3ybuz1jKUSlHkgIdqYa7JrWiTZ4lTGzZt+q+iB+WwVTq92gqn5p40waYQJMcPbqdWBhVRtxyqSi+Z/wQ76KAKGERpbh64ecK+h5MJaZ6145qd6YgvFhJTg3C77fw6xURQYa8rS8XyWiAVZVvqMgVs5CE5xolJIjccCwSXcRlyBK3fB9YagUFUUVN8XnE3ZGzaBAzCC9x4SCMAMuA+G7VLUfiqSJW5vodobxTMY0km5+DlLbaMNG0pYoDzXH6i9E+l1LjcAQJIt6ww==",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "95eefce6ddbf14ff09bd0854b314e9ba634b4884439a03101237d05a67ec4dca",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("SUCCESS", result.Item1);
        }

        [TestCategory("H2H")]
        [TestMethod]
        public async Task SendH2HPaymentRecurringAuthorization()
        {
            // Arrange
            H2HPreAuthorizationParameters h2hPaymentAuthorizationParameters = new();
            h2hPaymentAuthorizationParameters.SetAmount("50");
            h2hPaymentAuthorizationParameters.SetCurrency(Currency.EUR);
            h2hPaymentAuthorizationParameters.SetCountry(CountryCodeAlpha2.ES);
            h2hPaymentAuthorizationParameters.SetCustomerId("903");
            h2hPaymentAuthorizationParameters.SetCardNumber("4907270002222227");
            h2hPaymentAuthorizationParameters.SetMerchantTransactionId("11111111");
            h2hPaymentAuthorizationParameters.SetChName("Pablo Navarrow");
            h2hPaymentAuthorizationParameters.SetCvnNumber("123");
            h2hPaymentAuthorizationParameters.SetExpDate("0230");
            h2hPaymentAuthorizationParameters.SetAutoCapture(false);
            h2hPaymentAuthorizationParameters.SetPaymentSolution(PaymentSolutions.creditcards);
            h2hPaymentAuthorizationParameters.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            h2hPaymentAuthorizationParameters.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            h2hPaymentAuthorizationParameters.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            h2hPaymentAuthorizationParameters.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            h2hPaymentAuthorizationParameters.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("h2h_authorization.xml");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
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

                _realPaymentService.SendH2hPreAuthorizationRequest(h2hPaymentAuthorizationParameters, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.H2H_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "2mj81PHCaggkLNpl/1sOjGoKCnwXCVMBd1jWlkyxVx3sjsZZNTd/sVcozi0BzsK49AtAqN5iCYEME2fltryuNRp10/LvYyjustNOmkEvqGPbgMruMGuywTrXxzXnM5sPPvN/FXjqNjERnee0DQQfRXtMLzUJPinZ1D0VlLVllfjuMQl4KdFJGTRanmqMstVcCWtVW+6/RDPl18nLgCEpzKAYA6N+UOTnmey1b1qnPj4Ue9zK1TgX2BTm7Qf338vqIIJFrs/PkzAlbzeXHDzhEt45k79pmJBDMLLiA3i/kEz9k59Ha/g8w7wCKvi8dxfvGXOeSKGFUD0lfZb9zenlwikrGSmaxtuuKU9LaZS+1k9moy5XxyAu5CjFa5uhFXgFkj5R4F/y7V8a5t1a2rUpS6fkNx52/rQryyF6PLRm0BSrTJRs6kD0z89GyH/eOBJk6RCScqUcpn+8NaGl/JmYHEepPh4akKKDY02zFgVNK1mtu4P2LeKPra4PusEURPp9yFZs7YR3D+5tlbzJXh8zXxH5fr6q6LK3uBHArgQkHdRx2kzEMZ9Hr7PBL6I2mRP9h9KMcqq5V74iMFVIR3R6PYGntOQDsowCX7UAAn7wt+Y8WlEhZpLiig3ktDWWZWLRDGvfG8yNXaLPMCzeE7d09rP5neslLBSIRleZ7pagOUd8J721EjgaBTj404oD6WzO",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "9c6496f499f566a8d69077219a5c1467d9225c213d6481b9babbb5002ccb76b3",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("https://checkout.stg-eu-west3.epgint.com/EPGCheckout/rest/online/3dsv2/redirect?action=gatherdevice&params=eyJ0aHJlZURTdjJUb2tlbiI6IjM2ODU5NGM2LTUzMDItNDlhOC04NThhLTFkNTgwOTM3N2Q0YSIsInRocmVlRFNNZXRob2RVcmwiOiJodHRwczovL21vY2stZHMuc3RnLWV1LXdlc3QzLmVwZ2ludC5jb20vcHVibGljL21ldGhvZC1kYXRhLyIsInRocmVlRFNNZXRob2REYXRhIjoiZXlKMGFISmxaVVJUVTJWeWRtVnlWSEpoYm5OSlJDSTZJak0yT0RVNU5HTTJMVFV6TURJdE5EbGhPQzA0TlRoaExURmtOVGd3T1RNM04yUTBZU0lzSUNKMGFISmxaVVJUVFdWMGFHOWtUbTkwYVdacFkyRjBhVzl1VlZKTUlqb2dJbWgwZEhCek9pOHZZMmhsWTJ0dmRYUXVjM1JuTFdWMUxYZGxjM1F6TG1Wd1oybHVkQzVqYjIwdlJWQkhRMmhsWTJ0dmRYUXZZMkZzYkdKaFkyc3ZaMkYwYUdWeVJHVjJhV05sVG05MGFXWnBZMkYwYVc5dUwzQmhlWE52YkM4elpITjJNaTh4TURJeE9EZzRJbjA9IiwiYnJhbmQiOm51bGwsInJlc3VtZUF1dGhlbnRpY2F0aW9uIjoiaHR0cHM6Ly9jaGVja291dC5zdGctZXUtd2VzdDMuZXBnaW50LmNvbS9FUEdDaGVja291dC9yZXR1cm51cmwvZnJpY3Rpb25sZXNzL3BheXNvbC8zZHN2Mi8xMDIxODg4P3RocmVlRFN2MlRva2VuPTM2ODU5NGM2LTUzMDItNDlhOC04NThhLTFkNTgwOTM3N2Q0YSIsInJlbmRlckNhc2hpZXJMb2NhdGlvbiI6Imh0dHBzOi8vZXBnanMtcmVuZGVyY2FzaGllci1zdGcuZWFzeXBheW1lbnRnYXRld2F5LmNvbSIsImNoYWxsZW5nZVdpbmRvd3NTaXplIjoiMDUifQ==", result.Item1);
        }

        [TestCategory("H2H")]
        [TestMethod]
        public async Task SendH2HPaymentCapture()
        {
            // Arrange
            H2HPreAuthorizationCaptureParameters h2hPreAuthorizationCaptureParameters = new();
            h2hPreAuthorizationCaptureParameters.SetMerchantTransactionId("3133311111");
            h2hPreAuthorizationCaptureParameters.SetPaymentSolution(PaymentSolutions.caixapucpuce);
            h2hPreAuthorizationCaptureParameters.SetTransactionId("1333322");

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("h2h_capture.xml");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
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
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification?.Operations?.OperationList?.Last().Status, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendH2hPreAuthorizationCapture(h2hPreAuthorizationCaptureParameters, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.CAPTURE_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "ir+VKs1qIMTtMKsk/6Iv9XZorsRviAs5nFIcvJXrG+neAnidR2JDdLUML5zQSdXb2YfnJt6HEE++6mKkERvmy4p0FEfFlVqzwno9GT3hYmYHTFY/hQIvH2LpGJE5lwIRtoLjaM+mGCDg1VpPNTmV6w==",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "84973258e5e5a1a02c980a5829bac1d8a892a97d8ef3f19ad53c02a57a820b3e",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("SUCCESS", result.Item1);
        }

        [TestCategory("H2H")]
        [TestMethod]
        public async Task SendH2HPaymentVoid()
        {
            // Arrange
            H2HVoidParameters h2HVoidParameters = new();
            h2HVoidParameters.SetMerchantTransactionId("3133311111");
            h2HVoidParameters.SetPaymentSolution(PaymentSolutions.caixapucpuce);
            h2HVoidParameters.SetTransactionId("1333322");

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("h2h_void.xml");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
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
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification?.Operations?.OperationList?.Last().Status, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendH2hVoidRequest(h2HVoidParameters, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.VOID_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "ir+VKs1qIMTtMKsk/6Iv9XZorsRviAs5nFIcvJXrG+neAnidR2JDdLUML5zQSdXb2YfnJt6HEE++6mKkERvmy4p0FEfFlVqzwno9GT3hYmYHTFY/hQIvH2LpGJE5lwIRtoLjaM+mGCDg1VpPNTmV6w==",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "84973258e5e5a1a02c980a5829bac1d8a892a97d8ef3f19ad53c02a57a820b3e",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("SUCCESS", result.Item1);
        }


        [TestCategory("H2H")]
        [TestMethod]
        public async Task SendH2HPaymentRefund()
        {
            // Arrange
            H2HRefundParameters h2HRefundParameters = new();
            h2HRefundParameters.SetAmount("10");
            h2HRefundParameters.SetMerchantTransactionId("3133311111");
            h2HRefundParameters.SetPaymentSolution(PaymentSolutions.caixapucpuce);
            h2HRefundParameters.SetTransactionId("1333322");

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("h2h_refund.xml");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
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
                                .Callback<string, Notification, TransactionResult>((raw, notification, transaction) => tcs.SetResult((notification?.Operations?.OperationList?.Last().Status, null)));

                responseListener.Setup(listener => listener.OnError(It.IsAny<ErrorsEnum>(), It.IsAny<string>()))
                                .Callback<ErrorsEnum, string>((error, message) => tcs.SetResult((null, message)));

                _realPaymentService.SendH2hRefundRequest(h2HRefundParameters, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.REBATE_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "CIfojyIgVbecGGC6y+rVjgWHUSU2ZIYIeK9bM3mXAYC8sgXIrCMakNMR2VkLC9XnyZkOIkp69ci9tBC13XBiwCxa/q1PHhFTacOfjVGXd1R2ugxe8wQYmcQy97TFIXGORLFdgDRWzChV254EMowa5KtARg5z3iFEEUbQ3A65nD0=",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "deb815cc9ce39b6fea81cc944c28acc0e179bda4b7c696ff3ed266641cabb2e0",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("SUCCESS", result.Item1);
        }


        [TestCategory("H2H")]
        [TestMethod]
        public async Task SendH2H_MissingStatusURL_ThrowsException()
        {
            // Arrange
            H2HRedirectionParameters h2HRedirection = new();
            h2HRedirection.SetAmount("50");
            h2HRedirection.SetCurrency(Currency.EUR);
            h2HRedirection.SetCountry(CountryCodeAlpha2.ES);
            h2HRedirection.SetCustomerId("903");
            h2HRedirection.SetCardNumber("4907270002222227");
            h2HRedirection.SetMerchantTransactionId("11111111");
            h2HRedirection.SetChName("Pablo Navarrow");
            h2HRedirection.SetCvnNumber("123");
            h2HRedirection.SetExpDate("0230");
            h2HRedirection.SetPaymentSolution(PaymentSolutions.creditcards);
            // h2HRedirection.SetStatusURL(_mockConfigurations.Object["statusUrl"]); // StatusURL is intentionally not set
            h2HRedirection.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            h2HRedirection.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            h2HRedirection.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            h2HRedirection.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            h2HRedirection.SetForceTokenRequest(false);

            var tcs = new TaskCompletionSource<(string?, string?)>();
            var xmlContent = ReadXmlContent("h2h_response.xml");
            _mockNetworkAdapter.Setup(adapter => adapter.RequestHttpClient(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Callback((string url, Dictionary<string, string> data, Dictionary<string, string> headers) =>
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

                await _realPaymentService.SendH2HRedirectionPaymentRequest(h2HRedirection, responseListener.Object);
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


