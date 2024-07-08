using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Configuration;
using DotNetPaymentSDK.Services;
using DotNetPayment.Core.Domain.Enums;
using System.Threading.Tasks;

using DotNetPaymentSDK.src.Parameters.Hosted;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.Interfaces;
using DotNetPaymentSDK.src.Adapters;
using System.Collections.Generic;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Hosted;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Accommodation;
using DotNetPaymentSDK.src.Requests.Utils;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Product;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Service;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight;
using DotNetPaymentSDK.src.Exceptions;

namespace DotNetPaymentSDK.Tests
{
    [TestClass]
    public class QuixHostedServiceTests
    {
        private Mock<IConfiguration> _mockConfigurations;
        private IQUIXHostedService _realPaymentService;
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

            _realPaymentService = new QUIXHostedService
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

        [TestCategory("QuixHosted")]
        [TestMethod]
        public async Task SendHostedQuixAccommodation()
        {
            // Arrange
            HostedQuixAccommodation hostedQuixAccommodation = new();
            hostedQuixAccommodation.SetAmount("50");
            hostedQuixAccommodation.SetCustomerId("903");
            hostedQuixAccommodation.SetMerchantTransactionId("11111116");
            hostedQuixAccommodation.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            hostedQuixAccommodation.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            hostedQuixAccommodation.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            hostedQuixAccommodation.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            hostedQuixAccommodation.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
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

                _realPaymentService.SendHostedQuixAccommodationRequest(hostedQuixAccommodation, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.HOSTED_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "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",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "ea9df561e85af07cb363b3d6351b07bdf05b625b4672b1242b89734381dfa754",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("http://redirect.url", result.Item1);
        }

        [TestCategory("QuixHosted")]
        [TestMethod]
        public async Task SendHostedQuixItems()
        {
            // Arrange
            HostedQuixProduct hostedQuixProduct = new();
            hostedQuixProduct.SetAmount("50");
            hostedQuixProduct.SetCustomerId("903");
            hostedQuixProduct.SetMerchantTransactionId("11111116");
            hostedQuixProduct.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            hostedQuixProduct.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            hostedQuixProduct.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            hostedQuixProduct.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            hostedQuixProduct.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            hostedQuixProduct.SetCustomerEmail("test@mail.com");
            hostedQuixProduct.SetCustomerNationalId("99999999R");
            hostedQuixProduct.SetDob("01-12-1999");
            hostedQuixProduct.SetFirstName("Name");
            hostedQuixProduct.SetLastName("Last Name");
            hostedQuixProduct.SetIpAddress("0.0.0.0");

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

            hostedQuixProduct.SetPaySolExtendedData(quixItemPaySolExtendedData);

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

                _realPaymentService.SendHostedQuixProductRequest(hostedQuixProduct, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.HOSTED_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "ir+VKs1qIMTtMKsk/6Iv9V0ciD2CItEkjqeWo0jY6dpphfK+BPGVB+rNNHCXIs0S1Z94bfESA6pmuyozVuVQZG8IVJzpTsv75y2bEUx5P4SGYxKTshurwlrZdIn/fIwYK2q/YnAqYzWO2PoQAovJQhuIP73PzepqrPZOlIEFvE2tD7oI556IKrvJUX/yUOBAJ40KaozSgLDgrdeIQhOWhUfFCjuJRE9tsk2O3Sjr4OQWSAPr9xjE4Ky9WlI/mZk+WI2Na0KxbTo6gSTBZ2un5piNvzcCzjrmDLfbBVSFk+lK5g3n+7kGbWzzKGhBmBZX0FFO7oIfRl0vX07llgMxewHF8L83t5CGEk3eERhfy6zOqRdLfy9xqJhWjJmPK0F4DX9WUXk0DjTW0GuQuT9Ih0zaZAPX+Us8umdzKjTpUjs34ijhXgdGBec+NiMmzhUOiMeTTNrjcFSnBnoY9c5Hlj5zFLgUjeWYNI4xLaKqMS+KlJLnOMiZZmm4fAOQzOUJpyBz2/+Ln11rlggpWDrUqNXpB6dgoc+jexjmKyizRpMRyrmK8FtksCfo0+Typ/x/ag5MDfqL25nA8usNBHaf4+MQKALGeLK5niyFgb8CBB9Yi7Y4OEsY2zcarJQZJCELCsHZrZrLJDEmw/HrjClaSvTgC7PAbOmul4i9tpvDPeLxh1F1sXe/VG/MFs236fKw9W35BXKy8TsiJF1tg7bdilHwjp48dGbU9DgSyUyHjfoWjBMujhOEVOds1gbG6AnVad0rCYmZjmPs1kAAEa2nHpigmSDWmwZZZ2mgjNtCz/MH5LoBSmW3gU9Tf1NSwuB3NOS6hX53LD1dvii+7Ycfnx/EkmUAkPVcQpW7yJxewsaM6X/0pGEQ0ITh2191gsP++xtTP1MSeKXn4TEnGJs7+Ds1MJGaDYd2JhkvKPdAqeR+G0S+cydqf316emrbmejddYpmtQFE7itlifQBz7bv/JM0UWMw+xEG5zWIPY0WqhuMmMBnpi7QWxQgEd5wA8is0QW2Dt9skEvyxAzT7f7ppn3OGQOH5riiKpEML0vsBzSXROta/yrnAR7EofsKWpq8TqERIY9NewhnoCRzm/bKMFZlf1jXpVX+quDcUrMCwQYG2JqfV4Xc1O9t6YBy4cYQfZou3CmLdfTOzGXrhwukHwGLmCeB9q4gDDT3cdeNb+Xi2kMLiX7lhwWKn4A9W1z6n4Syf9Lsr1lpHMvleoZdah30u+KGqx7rY9hTfWsI9RbmSCgkZIEEvC2clXXjMSGS8kA2exhkuztVsCmJ9RJ/cqFCWLExsbsRlUUqSpCQpOcSQ17q6evXkV3twQkvKvHaezjE8AMcPIQfk5yESZKTFZgI/AD+RL9t0WPdMhYjWeZm6qlSlbOly+Eu4/E7wm9qcrVHAuvTyMezPVJTtW3kR6hInfbRhp3kd1D41eztd6Pe11fglhfoOYkvSXgs85M7K6sJXiGhEixtu9LH6vFCNd+BZNDsmqqyPo1o0oaERGdqsDWkcgU972TB/eCi8/nIgXCet6C1vyERA5NgVxDT4L3kcu6XQViz7wgumgvvTKmRnVnZEV625Qg+fv+qc8dp6TMOEQBcxyyQx/7CamepIJ26TOEVmDu8jjtW2oGMDzcvXGEZMWLUa6Rlvdx1xjHDGqNZrj4ovg9E8Z0DF5dY7m3Kral7sJTXLQW+sGMns3Zaw9VbBuIvLquGUet/o/Ik",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "d3818fbd2a1d6d2f2dd0bfdd2c9095d2609161b837c322db83563eff57dd2ff0",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("http://redirect.url", result.Item1);
        }


        [TestCategory("QuixHosted")]
        [TestMethod]
        public async Task SendHostedQuixItemsDisable()
        {
            // Arrange
            HostedQuixProduct hostedQuixProduct = new();
            hostedQuixProduct.SetAmount("50");
            hostedQuixProduct.SetCustomerId("903");
            hostedQuixProduct.SetMerchantTransactionId("11111116");
            hostedQuixProduct.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            hostedQuixProduct.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            hostedQuixProduct.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            hostedQuixProduct.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            hostedQuixProduct.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
            hostedQuixProduct.SetCustomerEmail("test@mail.com");
            hostedQuixProduct.SetCustomerNationalId("99999999R");
            hostedQuixProduct.SetDob("01-12-1999");
            hostedQuixProduct.SetFirstName("Name");
            hostedQuixProduct.SetLastName("Last Name");
            hostedQuixProduct.SetIpAddress("0.0.0.0");

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
            quixItemPaySolExtendedData.SetDisableFormEdition(true);

            hostedQuixProduct.SetPaySolExtendedData(quixItemPaySolExtendedData);

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

                _realPaymentService.SendHostedQuixProductRequest(hostedQuixProduct, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.HOSTED_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "ir+VKs1qIMTtMKsk/6Iv9V0ciD2CItEkjqeWo0jY6dpphfK+BPGVB+rNNHCXIs0S1Z94bfESA6pmuyozVuVQZG8IVJzpTsv75y2bEUx5P4SGYxKTshurwlrZdIn/fIwYK2q/YnAqYzWO2PoQAovJQhuIP73PzepqrPZOlIEFvE2tD7oI556IKrvJUX/yUOBAJ40KaozSgLDgrdeIQhOWhUfFCjuJRE9tsk2O3Sjr4OQWSAPr9xjE4Ky9WlI/mZk+WI2Na0KxbTo6gSTBZ2un5piNvzcCzjrmDLfbBVSFk+lK5g3n+7kGbWzzKGhBmBZX0FFO7oIfRl0vX07llgMxewHF8L83t5CGEk3eERhfy6zOqRdLfy9xqJhWjJmPK0F4DX9WUXk0DjTW0GuQuT9Ih0zaZAPX+Us8umdzKjTpUjs34ijhXgdGBec+NiMmzhUOiMeTTNrjcFSnBnoY9c5Hlj5zFLgUjeWYNI4xLaKqMS+KlJLnOMiZZmm4fAOQzOUJpyBz2/+Ln11rlggpWDrUqNXpB6dgoc+jexjmKyizRpMRyrmK8FtksCfo0+Typ/x/ag5MDfqL25nA8usNBHaf4+MQKALGeLK5niyFgb8CBB9Yi7Y4OEsY2zcarJQZJCELCsHZrZrLJDEmw/HrjClaSvTgC7PAbOmul4i9tpvDPeLxh1F1sXe/VG/MFs236fKw9W35BXKy8TsiJF1tg7bdilHwjp48dGbU9DgSyUyHjfoWjBMujhOEVOds1gbG6AnVo97hGy4Eu9x1tAr8VTaruXDlp3A8kZeY8PibgoVZXLPSD7bY46nQ3AhJK1zJ5M2G+v4pwXUoWezYzTXJXpOushAWSxzQSJljQ01B/GDt3YHjZ/qFAgMZ09tCPriJLyEcV/Gf7KasteAPKB9Bc13WFojn881ZvwjW/0A3PNGz4ETZyHfrBhqoGT8qPmJqLqCBCamHxWpnNtuwit/N5pa3sx4Q1Tn7OKP0w7JHA9lC6MgUfIPoOecY9ekBY1eZHUKbd1u1ZVtliWld71bKKL7f1WhkGQ8cJQZ5VLa4WDxgv1HZCHXbn0lTeAcn3j4exAMLUEiWUmxjNoXAuZni4sA2GqvFD+DO9ngIme2FqV0XIvNLzsqiT94ApQ1oeTJNl21+74XZSy+kJYQLTK+Ntigm2Qs4eLmHLJs5+5GB233YgOBQguZSMhlCJ8h8hsxU6340yAxIHzvtEvWRPnveI8/b7/WDUHryIejsgIfOadh2H0W1rR8Gi5oD5jctePEZceoC/NqCdSVTSTk7JM5ah1ia+kMZFbqJDjmCUmgOFbrP7W7LYYwXXvsoIO1UGMZnzaXVsYC6VG5qeODp4Lhu9RJ8obD/q0kPz2Cyw7k6E3g15b+matADahV4eSt8h/0HZIyCnYpn3yIOKa9o4DxgBViGN13qX1nY9IRPbgRR4g3O4sgl8ELBB5spyqiTRChaXP6dB2+HEiuRETUYJeeDgv/u9xF9CQTA+LKdXCJN5hMo00y4zWwoc1rRZr198JiDl07xapjQ/OKcQXRvE6JBkvlPqfBcEk19b1IhZcdgmU2I9Xl/jkcwMFjao3ebsNkTNKg+DWqKFXcVAr80VQOfUNem/gj3/QifKc3/oNQrj+eP+idTDcYOcRQYKxS3VMeTnjy3tvmagvyrKDodXewq+xWB0/vRJ/C8wzNScw7yFGJL5OYu3/PTxNbmFE1+1GOZpi5B",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "b4f4b0cebff68ce15b077d3535a06265600a175b023449ccfae38f9e37102e52",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("http://redirect.url", result.Item1);
        }

        [TestCategory("QuixHosted")]
        [TestMethod]
        public async Task SendHostedQuixService()
        {
            // Arrange
            HostedQuixService hostedQuixService = new();
            hostedQuixService.SetAmount("50");
            hostedQuixService.SetCustomerId("903");
            hostedQuixService.SetMerchantTransactionId("11111116");
            hostedQuixService.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            hostedQuixService.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            hostedQuixService.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            hostedQuixService.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            hostedQuixService.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
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

                _realPaymentService.SendHostedQuixServiceRequest(hostedQuixService, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.HOSTED_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "ir+VKs1qIMTtMKsk/6Iv9V0ciD2CItEkjqeWo0jY6dpphfK+BPGVB+rNNHCXIs0S1Z94bfESA6pmuyozVuVQZG8IVJzpTsv75y2bEUx5P4SGYxKTshurwlrZdIn/fIwYK2q/YnAqYzWO2PoQAovJQhuIP73PzepqrPZOlIEFvE2tD7oI556IKrvJUX/yUOBAJ40KaozSgLDgrdeIQhOWhUfFCjuJRE9tsk2O3Sjr4OQWSAPr9xjE4Ky9WlI/mZk+WI2Na0KxbTo6gSTBZ2un5piNvzcCzjrmDLfbBVSFk+lK5g3n+7kGbWzzKGhBmBZX0FFO7oIfRl0vX07llgMxewHF8L83t5CGEk3eERhfy6zOqRdLfy9xqJhWjJmPK0F4DX9WUXk0DjTW0GuQuT9Ih0zaZAPX+Us8umdzKjTpUjs34ijhXgdGBec+NiMmzhUOiMeTTNrjcFSnBnoY9c5Hlj5zFLgUjeWYNI4xLaKqMS+KlJLnOMiZZmm4fAOQzOUJpyBz2/+Ln11rlggpWDrUqNXpB6dgoc+jexjmKyizRpMRyrmK8FtksCfo0+Typ/x/ag5MDfqL25nA8usNBHaf4+MQKALGeLK5niyFgb8CBB9Yi7Y4OEsY2zcarJQZJCELCsHZrZrLJDEmw/HrjClaSvTgC7PAbOmul4i9tpvDPeLxh1F1sXe/VG/MFs236fKw9W35BXKy8TsiJF1tg7bdilHwjp48dGbU9DgSyUyHjfoWjBMujhOEVOds1gbG6AnVad0rCYmZjmPs1kAAEa2nHpigmSDWmwZZZ2mgjNtCz/MH5LoBSmW3gU9Tf1NSwuB3NOS6hX53LD1dvii+7Ycfnx/EkmUAkPVcQpW7yJxewsaM6X/0pGEQ0ITh2191gsP++xtTP1MSeKXn4TEnGJs7+Ds1MJGaDYd2JhkvKPdAqeR+G0S+cydqf316emrbmejddYpmtQFE7itlifQBz7bv/JM0UWMw+xEG5zWIPY0WqhuMmMBnpi7QWxQgEd5wA8is0QW2Dt9skEvyxAzT7f7ppn3OGQOH5riiKpEML0vsBzSXROta/yrnAR7EofsKWpq8TqERIY9NewhnoCRzm/bKMFZlf1jXpVX+quDcUrMCwQYG2JqfV4Xc1O9t6YBy4cYQfZou3CmLdfTOzGXrhwukH0PCA1WF7IEiMjxxs57IQKdPjZY3kRXAoP5yzdcNRuWQQHXWAcPen/7jowlyfe0ZHfWQAPKrCwC6iX7Fl8utjGkG354u1y4aw+3ggmzyFknDrfMubzestkZzs2hmKOOCW/TMRB2yn/cUkwNF7fPtRlCcr4qk2jWKOGX5R6z7Cq6ltNW6kx3iOpCnrt3mTG11KHa/1DIAHgAqtsofP4rAbbQpv9FFY5wzWyXgmFY3vZnYo1lnHYozMrj9oFfBY/CFETpJTI9Z8H0y9UGZvPiMHst+SX+6jFI/qif8T87uzfWA+tuiAXI/75jBgkZqlYutyIDriDr6218fRLN0NDPMMFzI4dZDLOC57iY0kSaXKYTL2a8mFC2DHlTMTWzcU2rXhu3GqiA2cgRwwBrapNdtQsk7rFGgIijS6s9tXFZJRZo5GmDAiQ6/OhUktbbLcVqo0+Q/nQSF31dJCU8k9YrItFXafQ5goqVV4uNgGcUyPKBV7mdV3XderY6lJ39iOw9i3hpwnVHpqMWgmSgAQDga4zG/MkBcxlh0pU8Mj+mL8F5oxQTpgI9T7QdyVCd3fhXRELwjVm3dEyPlJ4Xx1OAuzBTSkXboZYVUGOeyL4c9eIjqA5Pn5tA6NdEFyktVsD+h9w==",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "76a74b940bf8d6bfe0e79499e8f3d96b3234c0059c0f9ebe66fa8778a12b399c",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("http://redirect.url", result.Item1);
        }


        [TestCategory("QuixHosted")]
        [TestMethod]
        public async Task SendHostedQuixFlights()
        {
            // Arrange
            HostedQuixFlight hostedQuixFlight = new();
            hostedQuixFlight.SetAmount("50");
            hostedQuixFlight.SetCustomerId("903");
            hostedQuixFlight.SetMerchantTransactionId("11111116");
            hostedQuixFlight.SetStatusURL(_mockConfigurations.Object["statusUrl"]);
            hostedQuixFlight.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            hostedQuixFlight.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            hostedQuixFlight.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            hostedQuixFlight.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
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

                _realPaymentService.SendHostedQuixFlightRequest(hostedQuixFlight, responseListener.Object);
                return tcs.Task;
            });

            // Assert
            Assert.AreEqual(RequestsPaths.HOSTED_STG, capturedUrl);
            Assert.IsTrue(capturedData.ContainsKey("merchantId"));
            Assert.AreEqual(3, capturedHeaders.Count);
            Assert.AreEqual("5", capturedHeaders["apiVersion"]);
            Assert.AreEqual("CBC", capturedHeaders["encryptionMode"]);
            Assert.AreEqual("AQIDBAUGBwgJCgsMDQ4PEA==", capturedHeaders["iv"]);
            Assert.AreEqual("111222", capturedData["merchantId"]);
            Assert.AreEqual(
                "ir+VKs1qIMTtMKsk/6Iv9V0ciD2CItEkjqeWo0jY6dpphfK+BPGVB+rNNHCXIs0S1Z94bfESA6pmuyozVuVQZG8IVJzpTsv75y2bEUx5P4SGYxKTshurwlrZdIn/fIwYK2q/YnAqYzWO2PoQAovJQhuIP73PzepqrPZOlIEFvE2tD7oI556IKrvJUX/yUOBAJ40KaozSgLDgrdeIQhOWhUfFCjuJRE9tsk2O3Sjr4OQWSAPr9xjE4Ky9WlI/mZk+WI2Na0KxbTo6gSTBZ2un5piNvzcCzjrmDLfbBVSFk+lK5g3n+7kGbWzzKGhBmBZX0FFO7oIfRl0vX07llgMxewHF8L83t5CGEk3eERhfy6zOqRdLfy9xqJhWjJmPK0F4DX9WUXk0DjTW0GuQuT9Ih0zaZAPX+Us8umdzKjTpUjs34ijhXgdGBec+NiMmzhUOiMeTTNrjcFSnBnoY9c5Hlj5zFLgUjeWYNI4xLaKqMS+KlJLnOMiZZmm4fAOQzOUJpyBz2/+Ln11rlggpWDrUqNXpB6dgoc+jexjmKyizRpMRyrmK8FtksCfo0+Typ/x/ag5MDfqL25nA8usNBHaf4+MQKALGeLK5niyFgb8CBB9Yi7Y4OEsY2zcarJQZJCELCsHZrZrLJDEmw/HrjClaSvTgC7PAbOmul4i9tpvDPeLxh1F1sXe/VG/MFs236fKw9W35BXKy8TsiJF1tg7bdilHwjp48dGbU9DgSyUyHjfoWjBMujhOEVOds1gbG6AnVad0rCYmZjmPs1kAAEa2nHpigmSDWmwZZZ2mgjNtCz/MH5LoBSmW3gU9Tf1NSwuB3NOS6hX53LD1dvii+7Ycfnx/EkmUAkPVcQpW7yJxewsaM6X/0pGEQ0ITh2191gsP++xtTP1MSeKXn4TEnGJs7+Ds1MJGaDYd2JhkvKPdAqeR+G0S+cydqf316emrbmejddYpmtQFE7itlifQBz7bv/JM0UWMw+xEG5zWIPY0WqhuMmMBnpi7QWxQgEd5wA8is0QW2Dt9skEvyxAzT7f7ppn3OGQOH5riiKpEML0vsBzSXROta/yrnAR7EofsKWpq8TqERIY9NewhnoCRzm/bKMFZlf1jXpVX+quDcUrMCwQYG2JqfV4Xc1O9t6YBy4cYQfZou3CmLdfTOzGXrhwukH0PCA1WF7IEiMjxxs57IQKdPjZY3kRXAoP5yzdcNRuWQQHXWAcPen/7jowlyfe0ZHfWQAPKrCwC6iX7Fl8utjGkG354u1y4aw+3ggmzyFknDXnB5XvmfDlgfibpaOcyKDKhJEDIf/DSQvEdu68vtUVkPdU23esk3uV3XrZUjdl8cM6x58+eviDnb+JDJYGHEHbcQP7dyX7CCB5FcAGTRN0YQmZiJvbeGZNRqefBndLm/2tRhZREJ8ktcz4FbWXUkeQBAlH/rP5h5+Xr6nAG14IDSGj3L2IjYcuFAEdf1ztW41vbkCTp5TGUWfBfu7bLhlYVTXgIPQ5/Qr2WQDkH5pLtHlFONCYceFefqjLQNjXdSX0kDro6gdKI2jfkVzl2AM9Qgk2vUqw/moqA2nJ15QhqrCrGz34TDWGM42R2ZcejUzeeAAFTJNzZL0fWijl7NfPk/cQ7kiZaA46Sr5OVkng41+wdIrUK38SkOx8d45hj6nR8PEk3PCMViFitLLqzkZ+LyBqFnzcl0c3VlekGrq4CBux+p5t3kG/7fnXe1QEf0Yr+JahfAJwOsCvPfgry6gSeyeDkaw6ys5lFM3jTA7LWYqVgssTFJdXRjqecPnvjjy3SoKygIqbfF1XFBybOo/SJD+ZrQohUpj/C8qgsOgEBQGPhWiMA0Lh6nB5kiQewCz7BtLFApyh1V7gjEokuKtpd6kJrJ3cJ7RpD/jD0dVBq6GLMd2j304eIh1L9Kpa6GzfX6VveCRQxzcMc3AyQvOTjgOY+u1aIY0m7ISHO6jiANvIctkM+fzeK/U9Ru5kaUdJN7UkH2k0grmff0xNsioTxwnJQoskxHdv3/t2Ba4fipPUXMY9aCvbJabu/cxfuTMLBOi7i9Pe6GhKkboVI011PoZCY8rIfD4nnBcix3fDCqZRRIJI1L9trY+eE4m4Yc9mDZxsBW6izl7AVDLlvuERBxKBunaN+s3X3GCjp1pNAMSjD6T4sW7OiKWD9qV8XbbxZNvsDQ/gGbd+wnTvV0wJYIvdsJfMYZiLb1A4Bf7Nd72yk89LXsI8pc+bwc3jYtMCbMjmbonUEiWBrUzU4303qIE8tKR05Ahnw4UGSgltzTe2jnMTTOUyaMl0zpAhEcvXLAxQIICF7AcY56Z+BmAKFU0FDauaWCpaNPfBkC5R5p6dO4BMQC8m25OgUlk1GUkbwwI3jjbijgZv0F+jZcLaLtcbPJpGlvOvAui3tXARODXEgQE4mLIO7ECmFsqnfvb7Vx0vpXQiJb39XT05jagnr8U3ltMXlUcVUPQ+lcUvDwEK9/GeHRIcQrfgcd1vVSC4pCKc5GO3+eof3YwM7DRwJFGlU5ch9BZ7c8ah3VE6KLyHvP37VevEJGZkcM6qlG2j6qe+PRJBiF2agsnqH5NhmZzmniTUP93GpxrpZ6vsM0w/0R+Lw6a5g5rcbU2dTXSu/hddVt/hhCY7ZwDdWLIs9zJsh9161YOHX1BkYzEEdFUc/au0X4V58X5tbOVL/Jr2XgG8EmrpMZEgsusUNPAy94GrUuK8raH8yowhWuijIWPSb0BTVHHJEMIgkLPkEm4K7QHbna9pRTeIGmICgIel2xY98d/qMgjsRV7hi2hMz0ooOt/rrN//hVi1jU5duk0nrt3wzkp4oaA5wUtzf2hE1vpqZhAS+YexfUxJ5n7WLEexJY2WBrt0sVBSxXO919LiiJh3WB5UFGvE7rCl53AGgsjU5k87+QKPTqgpNXo1DX1xIHhBTkuJLiQJaotQsCc4BjUhuXkk81v+tvzxdIG17w99jpoCmAlpAhgLTtuEkpLJe1CNJTFnzVOHWhR3BSsTwAeciWaCO1gBMM/qZlke+IwQ7+dtxqfTsY/Q7SabM6EvNxeg1LQ5CpEesLC/1GQnrINS9JdIaNWPaZur+L9/9ZL1zvwJ+A5uxIES/3INgf7TrYAQjR8BWf8x7qMWAEP9qq9910Z9VQp5l/GrC0by7AJGUj5r+tJHlmmLNPfse3KLbvP9s+uvq/w8lLf9Nx97o1Ui4UI723DMCymjr31+hvMyU2c2SIWb1CFq4q86JCzE8o2Q85fdSSVTJIjp4aBmJekntKeVBipEeahGZ9wQA9jkORAS0Hmaiv/Cn3PyZ6wwm/75eo6T5eiXSkRgslKy6KFc+Pe6nKSr0ve92O8VcWfqhpqduU0UCZwhFCeYMhNWulbS/DxdKJP70EKUsg",
                capturedData["encrypted"]
            );
            Assert.AreEqual(
                "21353c09f408289e200750bdf8da39bcdee5816770fdd264494b2a0902856155",
                capturedData["integrityCheck"]
            );



            Assert.IsNotNull(result);
            Assert.IsNull(result.Item2); // No error message
            Assert.AreEqual("http://redirect.url", result.Item1);
        }



        [TestCategory("QuixHosted")]
        [TestMethod]
        public async Task SendHostedQuixAccommodation_MissingStatusURL_ThrowsException()
        {
            // Arrange
            HostedQuixAccommodation hostedQuixAccommodation = new();
            hostedQuixAccommodation.SetAmount("50");
            hostedQuixAccommodation.SetCustomerId("903");
            hostedQuixAccommodation.SetMerchantTransactionId("11111116");
            // hostedQuixAccommodation.SetStatusURL(_mockConfigurations.Object["statusUrl"]); // StatusURL is intentionally not set
            hostedQuixAccommodation.SetSuccessURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["successUrl"]);
            hostedQuixAccommodation.SetErrorURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["errorUrl"]);
            hostedQuixAccommodation.SetAwaitingURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["awaitingUrl"]);
            hostedQuixAccommodation.SetCancelURL(_mockConfigurations.Object["baseURL"] + _mockConfigurations.Object["cancelUrl"]);
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

            List<QuixItemCartItemAccommodation> items = new();
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

                await _realPaymentService.SendHostedQuixAccommodationRequest(hostedQuixAccommodation, responseListener.Object);
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
