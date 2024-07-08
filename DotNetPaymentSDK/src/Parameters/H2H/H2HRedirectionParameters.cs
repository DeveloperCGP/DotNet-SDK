using System.Runtime.Serialization;
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;

namespace DotNetPaymentSDK.src.Parameters.H2H
{
    [DataContract]
    public class H2HRedirectionParameters
    {
        [DataMember(Name = "amount")]
        private string Amount = null;
        [DataMember(Name = "country")]
        private CountryCodeAlpha2? Country = null;
        [DataMember(Name = "currency")]
        private Currency? Currency = null;
        [DataMember(Name = "customerId")]
        private string CustomerId = null;
        [DataMember(Name = "merchantId")]
        private string MerchantId = null;
        [DataMember(Name = "merchantTransactionId")]
        private string MerchantTransactionId = GeneralUtils.RandomString();
        [DataMember(Name = "paymentSolution")]
        private PaymentSolutions? PaymentSolution = null;
        [DataMember(Name = "chName")]
        private string ChName = null;
        [DataMember(Name = "cardNumber")]
        private string CardNumber = null;
        [DataMember(Name = "expDate")]
        private string ExpDate = null;
        [DataMember(Name = "cvnNumber")]
        private string CvnNumber = null;
        [DataMember(Name = "cardNumberToken")]
        private string CardNumberToken = null;
        [DataMember(Name = "statusURL")]
        private string StatusURL = null;
        [DataMember(Name = "successURL")]
        private string SuccessURL = null;
        [DataMember(Name = "errorURL")]
        private string ErrorURL = null;
        [DataMember(Name = "cancelURL")]
        private string CancelURL = null;
        [DataMember(Name = "awaitingURL")]
        private string AwaitingURL = null;
        [DataMember(Name = "productId")]
        private string ProductId = null;
        [DataMember(Name = "operationType")]
        private H2HOperationType OperationType = H2HOperationType.DEBIT;
        [DataMember(Name = "forceTokenRequest")]
        private bool ForceTokenRequest = false;
        [DataMember(Name = "language")]
        private Language? Language = null;
        [DataMember(Name = "referenceId")]
        private string ReferenceId = null;
        [DataMember(Name = "printReceipt")]
        private bool PrintReceipt = false;
        [DataMember(Name = "type")]
        private TransactionType Type = TransactionType.ECOM;
        [DataMember(Name = "autoCapture")]
        private bool AutoCapture = true;
        [DataMember(Name = "merchantParams", EmitDefaultValue = false)]
        private List<Tuple<string, string>> MerchantParams = null;

        public string GetAmount()
        {
            return Amount;
        }

        public void SetAmount(string amount)
        {
            string parseAmount = GeneralUtils.ParseAmount(amount) ?? throw new InvalidFieldException("amount: Should Follow Format #.#### And Be Between 0 And 1000000");
            Amount = parseAmount;
        }

        public Currency? GetCurrency()
        {
            return Currency;
        }

        public void SetCurrency(Currency currency)
        {
            Currency = currency;
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
            if (customerId == null || customerId.Length == 0 || customerId.Length > 80)
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

        public void SetMerchantTransactionId(String merchantTransactionId)
        {
            if (merchantTransactionId.Length == 0 || merchantTransactionId.Length > 45)
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

        public String GetChName()
        {
            return ChName;
        }

        public void SetChName(String chName)
        {
            if (chName.Length > 100)
            {
                throw new InvalidFieldException("chName: Invalid Size, Size Must Be chName <= 100");
            }
            ChName = chName;
        }

        public string GetCardNumber()
        {
            return CardNumber;
        }

        public void SetCardNumber(string cardNumber)
        {
            if (cardNumber.Length > 19 || !GeneralUtils.CheckLuhn(cardNumber))
            {
                throw new InvalidFieldException("cardNumber");
            }
            CardNumber = cardNumber;
        }

        public string GetExpDate()
        {
            return ExpDate;
        }

        public void SetExpDate(string expDate)
        {
            if (!GeneralUtils.IsValidExpDate(expDate))
            {
                throw new InvalidFieldException("expDate: Should Be In Format MMYY");
            }
            ExpDate = expDate;
        }

        public string GetCvnNumber()
        {
            return CvnNumber;
        }

        public void SetCvnNumber(string cvnNumber)
        {
            if (!GeneralUtils.IsNumbersOnly(cvnNumber) || cvnNumber.Length < 3 || cvnNumber.Length > 4)
            {
                throw new InvalidFieldException("expDate: Should Be Numerical 3 to 4 Digits");
            }
            CvnNumber = cvnNumber;
        }

        public string GetCardNumberToken()
        {
            return CardNumberToken;
        }

        public void SetCardNumberToken(string cardNumberToken)
        {
            if (cardNumberToken.Length < 16 || cardNumberToken.Length > 20)
            {
                throw new InvalidFieldException("cardNumberToken: Invalid Size, Size Must Be 16 <= cardNumberToken <= 20");
            }
            CardNumberToken = cardNumberToken;
        }

        public string GetStatusURL()
        {
            return StatusURL;
        }

        public void SetStatusURL(string statusURL)
        {
            if (!GeneralUtils.IsValidURL(statusURL))
            {
                throw new InvalidFieldException("statusURL: Must be a valid url");
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

        public string GetProductId()
        {
            return ProductId;
        }

        public H2HOperationType GetOperationType()
        {
            return OperationType;
        }

        public void SetOperationType(H2HOperationType operationType)
        {
            OperationType = operationType;
        }

        public TransactionType GetTransactionType()
        {
            return Type;
        }

        public void SetTransactionType(TransactionType type)
        {
            Type = type;
        }

        public bool IsForceTokenRequest()
        {
            return ForceTokenRequest;
        }

        public void SetForceTokenRequest(bool forceTokenRequest)
        {
            ForceTokenRequest = forceTokenRequest;
        }

        public bool IsAutoCapture()
        {
            return AutoCapture;
        }

        public void SetAutoCapture(bool autoCapture)
        {
            AutoCapture = autoCapture;
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

        public void SetMerchantParameter(string key, string value)
        {
            MerchantParams ??= [];
            MerchantParams.Add(new Tuple<string, string>(key, value));
            if (GeneralUtils.MerchantParamsQuery(MerchantParams).Length > 500)
            {
                throw new InvalidFieldException("merchantParams: Invalid Size, Size Must Be merchantParams <= 100");
            }
        }

        public List<Tuple<string, string>> GetMerchantParameters()
        {
            return MerchantParams;
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
            if (CardNumberToken == null)
            {
                mandatoryFields.Add("chName", ChName);
                mandatoryFields.Add("cardNumber", CardNumber);
                mandatoryFields.Add("expDate", ExpDate);
                mandatoryFields.Add("cvnNumber", CvnNumber);
            }

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