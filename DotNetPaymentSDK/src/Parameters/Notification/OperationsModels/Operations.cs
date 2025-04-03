using System.Runtime.Serialization;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.src.Parameters.Notification.OperationsModels
{
    [DataContract]
    public class Operations
    {
        [DataMember(Name = "operation")]
        public List<Operation> Operation { get; set; }
    }
}