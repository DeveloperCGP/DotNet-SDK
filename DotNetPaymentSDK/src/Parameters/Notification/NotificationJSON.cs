using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Enums;
using DotNetPaymentSDK.src.Parameters.Notification;
using DotNetPaymentSDK.src.Parameters.Notification.OperationsModels;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.NotificationJSON
{
    public class NotificationJSON
    {
        [JsonProperty("message")]
        public string? Message { get; set; }
        [JsonProperty("status")]
        public string? Status { get; set; }
        [JsonProperty("operationsArray")]
        public List<OperationJSON>? OperationsArray { get; set; } = null;
        [JsonProperty("workFlowResponse")]
        public WorkFlowResponse? WorkFlowResponse { get; set; }
    }
}