using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models
{
    public class QuixHostedRequest
    {
        [JsonProperty("merchantId")]
        private string MerchantId = null;
        [JsonProperty("productId")]
        private string ProductId = null;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("paymentSolution")]
        public PaymentSolutions? PaymentSolution { get; private set; } = PaymentSolutions.quix;
        [JsonProperty("merchantTransactionId")]
        public string MerchantTransactionId { get; private set; } = GeneralUtils.RandomString();
        [JsonProperty("amount")]
        public string Amount { get; private set; } = null;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("currency")]
        public Currency? Currency { get; private set; } = DotNetPayment.Core.Domain.Enums.Currency.EUR;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("country")]
        public CountryCodeAlpha2? Country { get; private set; } = CountryCodeAlpha2.ES;
        [JsonProperty("customerId")]
        public string CustomerId { get; private set; } = null;
        [JsonProperty("statusURL")]
        public string StatusURL { get; private set; } = null;
        [JsonProperty("successURL")]
        public string SuccessURL { get; private set; } = null;
        [JsonProperty("errorURL")]
        public string ErrorURL { get; private set; } = null;
        [JsonProperty("cancelURL")]
        public string CancelURL { get; private set; } = null;
        [JsonProperty("awaitingURL")]
        public string AwaitingURL { get; private set; } = null;
        [JsonProperty("firstName")]
        public string FirstName { get; private set; } = null;
        [JsonProperty("lastName")]
        public string LastName { get; private set; } = null;
        [JsonProperty("customerEmail")]
        public string CustomerEmail { get; private set; } = null;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("customerCountry")]
        public CountryCodeAlpha2? CustomerCountry { get; private set; } = CountryCodeAlpha2.ES;
        [JsonProperty("customerNationalId")]
        public string CustomerNationalId { get; private set; } = null;
        [JsonProperty("dob")]
        public string Dob { get; private set; } = null;
        [JsonProperty("ipAddress")]
        public string IpAddress { get; private set; } = null;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("operationType")]
        public OperationTypes? OperationType { get; private set; } = OperationTypes.DEBIT;
        [JsonProperty("paymentMethod ")]
        public string PaymentMethod { get; private set; } = null;
        [JsonProperty("telephone")]
        public string Telephone { get; private set; } = null;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("language")]
        public Language? Language { get; private set; } = null;
        [JsonProperty("merchantParams")]
        private List<Tuple<string, string>> MerchantParams = null;

        public void SetCurrency(Currency currency)
        {
            Currency = currency;
        }

        public void SetAmount(string amount)
        {
            string parsedAmount = GeneralUtils.ParseAmount(amount) ?? throw new InvalidFieldException("amount: Should Follow Format #.#### And Be Between 0 And 1000000");
            Amount = parsedAmount;
        }

        public void SetCountry(CountryCodeAlpha2 country)
        {
            Country = country;
        }

        public void SetCustomerId(string customerId)
        {
            if (string.IsNullOrEmpty(customerId?.Trim()) || customerId.Length > 80)
            {
                throw new InvalidFieldException("customerId: Invalid Size, size must be (0 < customerId <= 80)");
            }
            CustomerId = customerId;
        }

        public string GetMerchantId()
        {
            return MerchantId;
        }

        public void SetMerchantTransactionId(string merchantTransactionId)
        {
            if (string.IsNullOrEmpty(merchantTransactionId?.Trim()) || merchantTransactionId.Length > 45)
            {
                throw new InvalidFieldException("merchantTransactionId: Invalid Size, size must be (0 < merchantTransactionId <= 45)");
            }
            MerchantTransactionId = merchantTransactionId;
        }

        public PaymentSolutions? GetPaymentSolution()
        {
            return PaymentSolution;
        }

        public void SetStatusURL(string statusURL)
        {
            if (!GeneralUtils.IsValidURL(statusURL))
            {
                throw new InvalidFieldException("statusURL");
            }
            StatusURL = statusURL;
        }

        public void SetSuccessURL(string successURL)
        {
            if (!GeneralUtils.IsValidURL(successURL))
            {
                throw new InvalidFieldException("successURL");
            }
            SuccessURL = successURL;
        }

        public void SetAwaitingURL(string awaitingURL)
        {
            if (!GeneralUtils.IsValidURL(awaitingURL))
            {
                throw new InvalidFieldException("awaitingURL");
            }
            AwaitingURL = awaitingURL;
        }

        public void SetErrorURL(string errorURL)
        {
            if (!GeneralUtils.IsValidURL(errorURL))
            {
                throw new InvalidFieldException("errorURL");
            }
            ErrorURL = errorURL;
        }

        public void SetCancelURL(string cancelURL)
        {
            if (!GeneralUtils.IsValidURL(cancelURL))
            {
                throw new InvalidFieldException("cancelURL");
            }
            CancelURL = cancelURL;
        }

        public string GetFirstName()
        {
            return FirstName;
        }

        public void SetFirstName(string firstName)
        {
            FirstName = firstName;
        }

        public void SetLastName(string lastName)
        {
            LastName = lastName;
        }

        public string GetProductId()
        {
            return ProductId;
        }

        public void SetCustomerEmail(string customerEmail)
        {
            CustomerEmail = customerEmail;
        }

        public void SetDob(string dob)
        {
            Dob = dob;
        }

        public void SetCustomerNationalId(string customerNationalId)
        {
            if (customerNationalId.Length > 100)
            {
                throw new InvalidFieldException("customerNationalId: Invalid Size, size must be (customerNationalId <= 100)");
            }
            CustomerNationalId = customerNationalId;
        }

        public void SetIpAddress(string ipAddress)
        {
            if (ipAddress.Length > 45 || !GeneralUtils.IsValidIP(ipAddress))
            {
                throw new InvalidFieldException("ipAddress: must follow format IPv4 or IPv6 and max size is 45");
            }
            IpAddress = ipAddress;
        }

        public void SetOperationType(OperationTypes operationType)
        {
            OperationType = operationType;
        }

        public void SetPaymentMethod(string paymentMethod)
        {
            PaymentMethod = paymentMethod;
        }

        public void SetTelephone(string telephone)
        {
            if (telephone.Length > 45)
            {
                throw new InvalidFieldException("telephone: Invalid Size, size must be (telephone <= 45)");
            }
            Telephone = telephone;
        }

        public void SetLanguage(Language language)
        {
            Language = language;
        }

        public void SetCredentials(Credentials credentials)
        {
            MerchantId = credentials.GetMerchantId();
            ProductId = credentials.GetProductId();
        }

        public void SetMerchantParameters(List<Tuple<string, string>> merchantParams)
        {
            if (MerchantParams == null)
            {
                MerchantParams = merchantParams;
            }
            else
            {
                MerchantParams.AddRange(merchantParams);
            }
            if (GeneralUtils.MerchantParamsQuery(MerchantParams).Length > 500)
            {
                throw new InvalidFieldException("merchantParams: Invalid Size, Size Must Be merchantParams <= 100");
            }
        }

        public List<Tuple<string, string>> GetMerchantParameters()
        {
            return MerchantParams;
        }

        public void SetMerchantParameter(string key, string value)
        {
            MerchantParams ??= [];
            MerchantParams.Add(new Tuple<string, string>(key, value));
            if (GeneralUtils.MerchantParamsQuery(MerchantParams).Length > 500)
            {
                throw new InvalidFieldException("merchantParams: Invalid Size, Size Must Be merchantParams <= 100");
            }
        }

        public virtual Tuple<bool, string> IsMissingField()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                {"merchantId", MerchantId},
                {"productId", ProductId},
                {"paymentSolution", PaymentSolution?.ToString()},
                {"merchantTransactionId", MerchantTransactionId},
                {"amount", Amount},
                {"currency", Currency?.ToString()},
                {"country", Country?.ToString()},
                {"customerId", CustomerId},
                {"awaitingURL", AwaitingURL},
                {"statusURL", StatusURL},
                {"successURL", SuccessURL},
                {"errorURL", ErrorURL},
                {"cancelURL", CancelURL},
                {"firstName", FirstName},
                {"lastName", LastName},
                {"customerEmail", CustomerEmail},
                {"customerCountry", CustomerCountry?.ToString()},
                {"customerNationalId", CustomerNationalId},
                {"dob", Dob},
                {"ipAddress", IpAddress}
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
                {"environment", credentials.GetEnvironment()?.ToString()}
            };

            return GeneralUtils.ContainsNull(mandatoryFields);
        }
    }
}