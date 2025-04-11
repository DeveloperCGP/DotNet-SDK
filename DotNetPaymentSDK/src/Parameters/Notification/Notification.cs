using System.Xml.Serialization;
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Enums;
using DotNetPaymentSDK.src.Parameters.Notification;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Notification
{
    [XmlRoot(ElementName = "respCode")]
    public class RespCode
    {
        [XmlElement(ElementName = "code", IsNullable = true)]
        public string? Code { get; set; }

        [XmlElement(ElementName = "message", IsNullable = true)]
        public string? Message { get; set; }

        [XmlElement(ElementName = "uuid", IsNullable = true)]
        public string? UUID { get; set; }
    }

    [XmlRoot(ElementName = "operation")]
    public class Operation
    {
        [XmlElement(ElementName = "amount", IsNullable = true)]
        public string? Amount { get; set; }

        [XmlElement(ElementName = "currency", IsNullable = true)]
        public Currency? Currency { get; set; }

        [XmlElement(ElementName = "details",IsNullable = true)] 
        public string? Details { get; set; }
        [XmlElement(ElementName = "merchantTransactionId", IsNullable = true)]
        public string? MerchantTransactionId { get; set; }

        [XmlElement(ElementName = "operationType", IsNullable = true)]
        public OperationTypes? OperationType { get; set; }

        [XmlElement(ElementName = "paySolTransactionId", IsNullable = true)]
        public string? PaySolTransactionId { get; set; }

        [XmlElement(ElementName = "service", IsNullable = true)]
        public string? Service { get; set; }

        [XmlElement(ElementName = "status", IsNullable = true)]
        public string? Status { get; set; }
        [JsonProperty("operationsArray")]
        public List<Operation>? OperationsArray { get; set; } = null;
        [JsonProperty("workFlowResponse")]
        public WorkFlowResponse? WorkFlowResponse { get; set; }

        [XmlElement(ElementName = "transactionId", IsNullable = true)]
        public string? TransactionId { get; set; }

        [XmlElement(ElementName = "respCode", IsNullable = true)]
        public RespCode RespCode { get; set; }

        [XmlElement(ElementName = "message", IsNullable = true)]
        public string? Message { get; set; }

        [XmlElement(ElementName = "originalTransactionId", IsNullable = true)]
        public string? OriginalTransactionId { get; set; }

        [XmlElement(ElementName = "paymentDetails", IsNullable = true)]
        public PaymentDetails? PaymentDetails { get; set; }

        [XmlElement(ElementName = "redirectionResponse",IsNullable = true)]
        public string? RedirectionResponse { get; set; }
        
        [XmlElement(ElementName ="paymentMethod", IsNullable = true)]
        public string? PaymentMethod { get; set; }
        [XmlElement(ElementName = "paymentSolution", IsNullable = true)]
        public PaymentSolutions? PaymentSolution { get; set; }

        [XmlElement(ElementName = "authCode", IsNullable = false)]
        public string? AuthCode { get; set; }

        [XmlElement(ElementName ="rad", IsNullable = true)]
        public string? Rad { get; set; }

        [XmlElement(ElementName = "radMessage", IsNullable = true)]
        public string? RadMessage { get; set; }
        [XmlElement(ElementName = "subscriptionPlan", IsNullable = true)]
        public string? SubscriptionPlan { get; set; }
        [XmlElement(ElementName = "mpi", IsNullable = true)]
        public Mpi? MPI { get; set; }
        [XmlElement(ElementName = "paymentCode", IsNullable = true)]
        public string? PaymentCode { get; set; }
        [XmlElement(ElementName = "paymentMessage", IsNullable = true)]
        public string? PaymentMessage { get; set; }

        [XmlElement(ElementName = "optionalTransactionParams", IsNullable = true)]
        public OptionalTransactionParams? OptionalTransactionParams { get; set; }

    }

    [XmlRoot(ElementName = "paymentDetails")]
    public class PaymentDetails
    {
        [XmlElement(ElementName = "cardHolderName", IsNullable = true)]
        public string? CardHolderName { get; set; }
        
        [XmlElement(ElementName = "account", IsNullable = true)]
        public string? Account { get; set; }
        [XmlElement(ElementName = "cardNumberToken", IsNullable = false)]
        public string? CardNumberToken { get; set; }
        [XmlElement(ElementName = "cardNumber", IsNullable = true)]
        public string? CardNumber { get; set; }
        [XmlElement(ElementName = "cardType", IsNullable = true)]
        public string? CardType { get; set; }
        [XmlElement(ElementName = "expDate", IsNullable = true)]
        public string? ExpDate { get; set; }
        [XmlElement(ElementName = "issuerBank", IsNullable = true)]
        public string? IssuerBank { get; set; }
        [XmlElement(ElementName = "issuerCountry", IsNullable = true)]
        public string? IssuerCountry { get; set; }
        [XmlElement(ElementName = "extraDetails", IsNullable = true)]
        public ExtraDetails? ExtraDetails { get; set; }
    }

    [XmlRoot(ElementName = "operations")]
    public class Operations
    {
        [XmlElement(ElementName = "operation", IsNullable = true)]
        public List<Operation>? OperationList { get; set; }
    }
    
    [XmlRoot(ElementName = "entry")]
    public class Entry
    {
        [XmlElement(ElementName = "key", IsNullable = true)]
        public string Key { get; set; }
        [XmlElement(ElementName = "value", IsNullable = true)]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "extraDetails")]
    public class ExtraDetails
    {
        [XmlElement(ElementName = "entry", IsNullable = true)]
        public List<Entry>? Entry { get; set; }

        public string GetNemuruTxnId()
        {
            foreach (var entry in Entry)
            {
                if (entry.Key == "nemuruTxnId")
                {
                    return entry.Value;
                }
            }
            return null;
        }

        public string GetNemuruCartHash()
        {
            foreach (var entry in Entry)
            {
                if (entry.Key == "nemuruCartHash")
                {
                    return entry.Value;
                }
            }
            return null;
        }

        public string GetNemuruAuthToken()
        {
            foreach (var entry in Entry)
            {
                if (entry.Key == "nemuruAuthToken")
                {
                    return entry.Value;
                }
            }
            return null;
        }

        public string GetNemuruDisableFormEdition()
        {
            foreach (var entry in Entry)
            {
                if (entry.Key == "nemuruDisableFormEdition")
                {
                    return entry.Value;
                }
            }
            return null;
        }

        public string GetStatus()
        {
            foreach (var entry in Entry)
            {
                if (entry.Key == "status")
                {
                    return entry.Value;
                }
            }
            return null;
        }

        public string GetDisableFormEdition()
        {
            foreach (var entry in Entry)
            {
                if (entry.Key == "disableFormEdition")
                {
                    return entry.Value;
                }
            }
            return null;
        }
    }

    [XmlRoot(ElementName = "response")]
    public class Notification
    {
        [XmlElement(ElementName = "message", IsNullable = true)]
        public string? Message { get; set; }

        [XmlElement(ElementName = "operations", IsNullable = true)]
        public Operations? Operations { get; set; }

        [XmlElement(ElementName = "status", IsNullable = true)]
        public string? Status { get; set; }

        [XmlElement(ElementName = "workFlowResponse", IsNullable = true)]
        public WorkFlowResponse? WorkFlowResponse { get; set; }

        [XmlElement(ElementName = "optionalTransactionParams", IsNullable = true)]
        public OptionalTransactionParams? OptionalTransactionParams { get; set; }

        public string GetNemuruCartHash()
        {
            return Operations?.OperationList?.First()?.PaymentDetails?.ExtraDetails?.GetNemuruCartHash();
        }

        public string GetNemuruAuthToken()
        {
            return Operations?.OperationList?.First()?.PaymentDetails?.ExtraDetails?.GetNemuruAuthToken();
        }

        public string GetMerchantTransactionId()
        {
            return Operations?.OperationList?.Last()?.MerchantTransactionId;
        }

        public TransactionResult GetTransactionResult()
        {
            var status = Operations?.OperationList?.Last()?.Status;
            return EnumsUtils.GetTransactionResultEnum(status) ?? TransactionResult.ERROR;
        }

        public string GetDisableFormEdition()
        {
            return Operations?.OperationList?.Last()?.PaymentDetails?.ExtraDetails?.GetDisableFormEdition();
        }

        public bool IsLastnotification()
        {
            return Operations.OperationList.Last().Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) || Operations.OperationList.Last().Status.Equals("ERROR", StringComparison.OrdinalIgnoreCase);
        }

        public string GetRedirectUrl()
        {
            string redirectionUrl = Operations?.OperationList?.Last()?.RedirectionResponse;
            if (redirectionUrl != null)
            {
                return redirectionUrl.Replace("redirect:", "");
            }
            return null;
        }
    }

    [XmlRoot(ElementName = "payfrex-response")]
    public class PayfrexResponse: Notification
    { }

    [XmlRoot(ElementName = "mpi")]
    public class Mpi
    {
        [XmlElement(ElementName = "acsTransID", IsNullable = true)]
        public string? AcsTransID { get; set; }
        [XmlElement(ElementName = "authMethod", IsNullable = true)]
        public string? AuthMethod { get; set; }
        [XmlElement(ElementName = "authTimestamp", IsNullable = true)]
        public string? AuthTimestamp { get; set; }
        [XmlElement(ElementName = "authenticationStatus", IsNullable = true)]
        public string? AuthenticationStatus { get; set; }
        [XmlElement(ElementName = "cavv", IsNullable = true)]
        public string? Cavv { get; set; }
        [XmlElement(ElementName = "eci", IsNullable = true)]
        public string? Eci { get; set; }
        [XmlElement(ElementName = "messageVersion", IsNullable = true)]
        public string? MessageVersion { get; set; }
        [XmlElement(ElementName = "threeDSSessionData", IsNullable = true)]
        public string? ThreeDSSessionData { get; set; }
        [XmlElement(ElementName = "threeDSv2Token", IsNullable = true)]
        public string? ThreeDSv2Token { get; set; }
    }
}