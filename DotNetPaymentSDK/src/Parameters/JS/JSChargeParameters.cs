using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.JS
{
    public class JSChargeParameters
    {
        [JsonProperty("prepayToken")]
        private string PrepayToken = null;

        [JsonProperty("merchantId")]
        private string MerchantId = null;

        [JsonProperty("productId")]
        private string ProductId = null;

        [JsonProperty("apiVersion")]
        private int ApiVersion = -1;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("paymentSolution")]
        private PaymentSolutions? PaymentSolution = null;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("operationType")]
        private OperationTypes OperationType = OperationTypes.DEBIT;

        [JsonProperty("merchantTransactionId")]
        private string MerchantTransactionId = GeneralUtils.RandomString();

        [JsonProperty("amount")]
        private string Amount = null;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("currency")]
        private Currency? Currency = null;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("country")]
        private CountryCodeAlpha2? Country = null;

        [JsonProperty("customerId")]
        private string CustomerId = null;

        [JsonProperty("statusURL")]
        private string StatusURL = null;

        [JsonProperty("successURL")]
        private string SuccessURL = null;

        [JsonProperty("errorURL")]
        private string ErrorURL = null;

        [JsonProperty("cancelURL")]
        private string CancelURL = null;

        [JsonProperty("awaitingURL")]
        private string AwaitingURL = null;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("language")]
        private Language? Language = null;

        [JsonProperty("referenceId")]
        private string ReferenceId = null;

        [JsonProperty("merchantParams")]
        private List<Tuple<string, string>> MerchantParams = null;

        [JsonProperty("forceTokenRequest")]
        private bool ForceTokenRequest = false;

        [JsonProperty("printReceipt")]
        private bool PrintReceipt = false;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        private TransactionType Type = TransactionType.ECOM;

        [JsonProperty("autoCapture")]
        private bool AutoCapture = true;

        [JsonProperty("description")]
        private string Description = null;

        public Currency? GetCurrency()
        {
            return Currency;
        }

        public void SetCurrency(Currency currency)
        {
            Currency = currency;
        }

        public string GetMerchantId()
        {
            return MerchantId;
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

        public string GetAmount()
        {
            return Amount;
        }

        public void SetAmount(string amount)
        {
            string parsedAmount = GeneralUtils.ParseAmount(amount);
            Amount = parsedAmount ?? throw new InvalidFieldException("amount: Should Follow Format #.#### And Be Between 0 And 1000000");
        }

        public string GetMerchantTransactionId()
        {
            return MerchantTransactionId;
        }

        public void SetMerchantTransactionId(string merchantTransactionId)
        {
            if (string.IsNullOrEmpty(merchantTransactionId) || merchantTransactionId.Length > 45)
            {
                throw new InvalidFieldException("merchantTransactionId: Invalid Size, size must be (0 < merchantTransactionId <= 45)");
            }
            MerchantTransactionId = merchantTransactionId;
        }

        public OperationTypes GetOperationType()
        {
            return OperationType;
        }

        public void SetOperationType(OperationTypes operationType)
        {
            OperationType = operationType;
        }

        public string GetPrepayToken()
        {
            return PrepayToken;
        }

        public void SetPrepayToken(string prepayToken)
        {
            PrepayToken = prepayToken;
        }

        public string GetStatusURL()
        {
            return StatusURL;
        }

        public void SetStatusURL(string statusURL)
        {
            if (!GeneralUtils.IsValidURL(statusURL))
            {
                throw new InvalidFieldException("statusURL");
            }
            StatusURL = statusURL;
        }

        public string GetSuccessURL()
        {
            return SuccessURL;
        }

        public void SetSuccessURL(string successURL)
        {
            if (!GeneralUtils.IsValidURL(successURL))
            {
                throw new InvalidFieldException("successURL");
            }
            SuccessURL = successURL;
        }

        public string GetErrorURL()
        {
            return ErrorURL;
        }

        public void SetErrorURL(string errorURL)
        {
            if (!GeneralUtils.IsValidURL(errorURL))
            {
                throw new InvalidFieldException("errorURL");
            }
            ErrorURL = errorURL;
        }

        public string GetCancelURL()
        {
            return CancelURL;
        }

        public void SetCancelURL(string cancelURL)
        {
            if (!GeneralUtils.IsValidURL(cancelURL))
            {
                throw new InvalidFieldException("cancelURL");
            }
            CancelURL = cancelURL;
        }

        public string GetAwaitingURL()
        {
            return AwaitingURL;
        }

        public void SetAwaitingURL(string awaitingURL)
        {
            if (!GeneralUtils.IsValidURL(awaitingURL))
            {
                throw new InvalidFieldException("awaitingURL");
            }
            AwaitingURL = awaitingURL;
        }

        public PaymentSolutions? GetPaymentSolution()
        {
            return PaymentSolution;
        }

        public void SetPaymentSolution(PaymentSolutions paymentSolution)
        {
            PaymentSolution = paymentSolution;
        }

        public int GetApiVersion()
        {
            return ApiVersion;
        }

        public void SetApiVersion(int apiVersion)
        {
            if (apiVersion < 0)
            {
                throw new InvalidFieldException("apiVersion: must be (apiVersion > 0)");
            }
            ApiVersion = apiVersion;
        }

        public bool IsForceTokenRequest()
        {
            return ForceTokenRequest;
        }

        public void SetForceTokenRequest(bool forceTokenRequest)
        {
            ForceTokenRequest = forceTokenRequest;
        }

        public Language? GetLanguage()
        {
            return Language;
        }

        public void SetLanguage(Language language)
        {
            Language = language;
        }

        public string GetReferenceId()
        {
            return ReferenceId;
        }

        public void SetReferenceId(string referenceId)
        {
            if (referenceId.Length != 12)
            {
                throw new InvalidFieldException("referenceId: Invalid Size, size must be (referenceId = 12)");
            }
            ReferenceId = referenceId;
        }

        public bool IsPrintReceipt()
        {
            return PrintReceipt;
        }

        public void SetPrintReceipt(bool printReceipt)
        {
            PrintReceipt = printReceipt;
        }

        public TransactionType GetTypeEnum()
        {
            return Type;
        }

        public void SetType(TransactionType type)
        {
            Type = type;
        }

        public bool IsAutoCapture()
        {
            return AutoCapture;
        }

        public void SetAutoCapture(bool autoCapture)
        {
            AutoCapture = autoCapture;
        }

        public string GetDescription()
        {
            return Description;
        }

        public void SetDescription(string description)
        {
            if (description.Length > 1000)
            {
                throw new InvalidFieldException("description: Invalid Size, size must be (description <= 1000)");
            }
            Description = description;
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
            MerchantParams.Add(new(key, value));
            if (GeneralUtils.MerchantParamsQuery(MerchantParams).Length > 500)
            {
                throw new InvalidFieldException("merchantParams: Invalid Size, Size Must Be merchantParams <= 100");
            }
        }

        public void SetCredentials(Credentials credentials)
        {
            MerchantId = credentials.GetMerchantId();
            ProductId = credentials.GetProductId();
        }

        public virtual Tuple<bool, string> IsMissingField()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                {"merchantId", MerchantId},
                {"productId", ProductId},
                {"merchantTransactionId", MerchantTransactionId},
                {"amount", Amount},
                {"currency", Currency?.ToString()},
                {"country", Country?.ToString()},
                {"paymentSolution", PaymentSolution?.ToString()},
                {"customerId", CustomerId},
                {"operationType", OperationType.ToString()},
                {"statusURL", StatusURL},
                {"successURL", SuccessURL},
                {"errorURL", ErrorURL},
                {"cancelURL", CancelURL},
                {"awaitingURL", AwaitingURL},
                {"prepayToken", PrepayToken}
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