using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.JS
{
    public class JSAuthorizationRequestParameters
    {
        [JsonProperty("merchantId")]
        private string MerchantId = null;
        [JsonProperty("merchantKey")]
        private string MerchantKey = null;
        [JsonProperty("productId")]
        private string ProductId = null;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("currency")]
        private Currency? Currency = null;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("country")]
        private CountryCodeAlpha2? Country = null;
        [JsonProperty("customerId")]
        private string CustomerId = null;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("operationType")]
        private OperationTypes? OperationType = null;
        [JsonProperty("anonymousCustomer")]
        private bool AnonymousCustomer = false;

        public Currency? GetCurrency()
        {
            return Currency;
        }

        public void SetCurrency(Currency currency)
        {
            Currency = currency;
        }

        public string GetProductId()
        {
            return ProductId;
        }

        public CountryCodeAlpha2? GetCountry()
        {
            return Country;
        }

        public void SetCountry(CountryCodeAlpha2 country)
        {
            Country = country;
        }

        public string GetCustomerId()
        {
            return CustomerId;
        }

        public void SetCustomerId(string customerId)
        {
            if (string.IsNullOrEmpty(customerId) || customerId.Length > 80)
            {
                throw new InvalidFieldException("customerId: Invalid Size, size must be (0 < customerId <= 80)");
            }
            CustomerId = customerId;
        }

        public OperationTypes? GetOperationType()
        {
            return OperationType;
        }

        public void SetOperationType(OperationTypes operationType)
        {
            OperationType = operationType;
        }

        public string GetMerchantId()
        {
            return MerchantId;
        }

        public string GetMerchantKey()
        {
            return MerchantKey;
        }

        public bool IsAnonymousCustomer()
        {
            return AnonymousCustomer;
        }

        public void SetAnonymousCustomer(bool anonymousCustomer)
        {
            AnonymousCustomer = anonymousCustomer;
        }

        public void SetCredentials(Credentials credentials)
        {
            MerchantId = credentials.GetMerchantId();
            ProductId = credentials.GetProductId();
            MerchantKey = credentials.GetMerchantKey();
        }

        public virtual Tuple<bool, string> IsMissingField()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                {"merchantId", MerchantId},
                {"productId", ProductId},
                {"merchantKey", MerchantKey},
                {"currency", Currency?.ToString()},
                {"country", Country?.ToString()},
                {"customerId", CustomerId},
                {"operationType", OperationType?.ToString()}
            };

            return GeneralUtils.ContainsNull(mandatoryFields);
        }

        public Tuple<bool, string> CheckCredentials(Credentials credentials)
        {
            if (credentials.GetApiVersion() < 0)
            {
                return new(true, "apiVersion");
            }
            var mandatoryFields = new Dictionary<string, string>()
            {
                {"merchantId", credentials.GetMerchantId()},
                {"productId", credentials.GetProductId()},
                {"merchantKey", credentials.GetMerchantKey()},
                {"environment", credentials.GetEnvironment()?.ToString()}
            };

            return GeneralUtils.ContainsNull(mandatoryFields);
        }
    }
}