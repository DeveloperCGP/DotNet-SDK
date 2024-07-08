using DotNetPaymentSDK.src.Parameters.Notification.Operation;
using DotNetPaymentSDK.src.Parameters.Notification;
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Enums;
using Newtonsoft.Json;
using DotNetPaymentSDK.Utilities;

namespace DotNetPaymentSDK.src.Parameters.Nottification
{
    public class Notification
    {
        [JsonProperty("message")]
        public string? Message { get; set; }
        [JsonProperty("status")]
        public string? Status { get; set; }
        [JsonProperty("operationsArray")]
        public List<Operation>? OperationsArray { get; set; } = null;
        [JsonProperty("workFlowResponse")]
        public WorkFlowResponse? WorkFlowResponse { get; set; }

        public bool IsLastnotification()
        {
            return OperationsArray.Last().Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) || OperationsArray.Last().Status.Equals("ERROR", StringComparison.OrdinalIgnoreCase);
        }

        public string GetRedirectUrl()
        {
            string redirectionUrl = OperationsArray?.Last()?.RedirectionResponse;
            if (redirectionUrl != null)
            {
                return redirectionUrl.Replace("redirect:", "");
            }
            return null;
        }

        public string GetNemuruCartHash()
        {
            return OperationsArray?.First()?.PaymentDetails?.ExtraDetails?.NemuruCartHash;
        }

        public string GetNemuruAuthToken()
        {
            return OperationsArray?.First()?.PaymentDetails?.ExtraDetails?.NemuruAuthToken;
        }

        public string GetMerchantTransactionId()
        {
            return OperationsArray?.Last()?.MerchantTransactionId;
        }

        public TransactionResult GetTransactionResult()
        {
            var status = OperationsArray?.Last()?.Status;
            return EnumsUtils.GetTransactionResultEnum(status) ?? TransactionResult.ERROR;
        }

        public string GetDisableFormEdition()
        {
            return OperationsArray?.Last()?.PaymentDetails?.ExtraDetails?.DisableFormEdition;
        }
    }
}