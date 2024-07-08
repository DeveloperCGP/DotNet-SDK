using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.Utilities;

namespace DotNetPaymentSDK.src.Parameters.Hosted
{
    public class HostedPaymentRedirection
    {
        private string MerchantId = null;
        private string ProductId = null;
        private PaymentSolutions? PaymentSolution = null;
        private OperationTypes OperationType = OperationTypes.DEBIT;
        private string MerchantTransactionId = GeneralUtils.RandomString();
        private string Amount = null;
        private Currency? Currency = null;
        private CountryCodeAlpha2? Country = null;
        private string CustomerId = null;
        private string StatusURL = null;
        private string SuccessURL = null;
        private string ErrorURL = null;
        private string CancelURL = null;
        private string AwaitingURL = null;
        private Language? Language = null;
        private string ReferenceId = null;
        private bool PrintReceipt = false;
        private TransactionType TransactionType = TransactionType.ECOM;
        private bool AutoCapture = true;
        private string Description = null;
        private bool ForceTokenRequest = false;
        private bool ShowRememberMe = false;
        private List<Tuple<string, string>> MerchantParams = null;

        public Currency? GetCurrency()
        {
            return Currency;
        }

        public void SetCurrency(Currency currency)
        {
            Currency = currency;
        }

        public string GetAmount()
        {
            return Amount;
        }

        public void SetAmount(string amount)
        {
            string parseAmount = GeneralUtils.ParseAmount(amount) ?? throw new InvalidFieldException("amount: Should Follow Format #.#### And Be Between 0 And 1000000");
            Amount = parseAmount;
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
            if (customerId.Length > 80)
            {
                throw new InvalidFieldException("customerId: Invalid Size, size must be (0 < customerId <= 80)");
            }
            CustomerId = customerId;
        }

        public string GetMerchantId()
        {
            return MerchantId;
        }

        public string GetMerchantTransactionId()
        {
            return MerchantTransactionId;
        }

        public void SetMerchantTransactionId(string merchantTransactionId)
        {
            if (string.IsNullOrWhiteSpace(merchantTransactionId) || merchantTransactionId.Length > 45)
            {
                throw new InvalidFieldException("merchantTransactionId: Invalid Size, size must be (0 < merchantTransactionId <= 45)");
            }
            MerchantTransactionId = merchantTransactionId;
        }

        public PaymentSolutions? GetPaymentSolution()
        {
            return PaymentSolution;
        }

        public void SetPaymentSolution(PaymentSolutions paymentSolution)
        {
            PaymentSolution = paymentSolution;
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

        public string GetProductId()
        {
            return ProductId;
        }

        public OperationTypes GetOperationType()
        {
            return OperationType;
        }

        public void SetOperationType(OperationTypes operationType)
        {
            OperationType = operationType;
        }

        public bool IsForceTokenRequest()
        {
            return ForceTokenRequest;
        }

        public void SetForceTokenRequest(bool forceTokenRequest)
        {
            ForceTokenRequest = forceTokenRequest;
        }

        public bool IsShowRememberMe()
        {
            return ShowRememberMe;
        }

        public void SetShowRememberMe(bool showRememberMe)
        {
            ShowRememberMe = showRememberMe;
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

        public TransactionType GetTransactionType()
        {
            return TransactionType;
        }

        public void SetTransactionType(TransactionType type)
        {
            TransactionType = type;
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
            MerchantParams.Add(new Tuple<string, string>(key, value));
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
            {"amount", Amount},
            {"country", Country?.ToString()},
            {"currency", Currency?.ToString()},
            {"customerId", CustomerId},
            {"merchantId", MerchantId},
            {"merchantTransactionId", MerchantTransactionId},
            {"paymentSolution", PaymentSolution?.ToString()},
            {"statusURL", StatusURL},
            {"successURL", SuccessURL},
            {"errorURL", ErrorURL},
            {"cancelURL", CancelURL},
            {"awaitingURL", AwaitingURL},
            {"productId", ProductId},
            {"operationType", OperationType.ToString()}
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
                {"merchantPass", credentials.GetMerchantPass()},
                {"environment", credentials.GetEnvironment()?.ToString()}
            };

            return GeneralUtils.ContainsNull(mandatoryFields);
        }
    }

}
