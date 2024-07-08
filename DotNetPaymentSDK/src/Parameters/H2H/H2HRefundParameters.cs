using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;

namespace DotNetPaymentSDK.src.Parameters.H2H
{
    public class H2HRefundParameters
    {
        private string Amount = null;
        private string MerchantId = null;
        private PaymentSolutions? PaymentSolution = null;
        private string TransactionId = null;
        private string MerchantTransactionId = GeneralUtils.RandomString();
        private string Description = null;

        public string GetAmount()
        {
            return Amount;
        }

        public void SetAmount(string amount)
        {
            string parsedAmount = GeneralUtils.ParseAmount(amount) ?? throw new InvalidFieldException("amount: Should Follow Format #.#### And Be Between 0 And 1000000");
            Amount = parsedAmount;
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
            if (merchantTransactionId.Trim().Length == 0 || merchantTransactionId.Length > 45)
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

        public string GetTransactionId()
        {
            return TransactionId;
        }

        public void SetTransactionId(string transactionId)
        {
            if (!GeneralUtils.IsNumbersOnly(transactionId) || transactionId.Length > 100)
            {
                throw new InvalidFieldException("transactionId: Must be numbers only with size (transactionId <= 100)");
            }
            TransactionId = transactionId;
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

        public void SetCredentials(Credentials credentials)
        {
            MerchantId = credentials.GetMerchantId();
        }

        public virtual Tuple<bool, string> IsMissingField()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                {"amount", Amount},
                {"merchantId", MerchantId},
                {"merchantTransactionId", MerchantTransactionId},
                {"paymentSolution", PaymentSolution?.ToString()},
                {"transactionId", TransactionId}
            };

            foreach (var field in mandatoryFields)
            {
                if (field.Value == null)
                {
                    return new(true, field.Key);
                }
            }
            return new(false, null);
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
                {"merchantPass", credentials.GetMerchantPass()},
                {"environment", credentials.GetEnvironment()?.ToString()}
            };

            foreach (var field in mandatoryFields)
            {
                if (field.Value == null)
                {
                    return new(true, field.Key);
                }
            }
            return new(false, null);
        }

    }
}