using System.Runtime.Serialization;

namespace DotNetPaymentSDK.src.Parameters.Notification.Operation
{
    [DataContract]
    public class Operations
    {
        [DataMember(Name = "operation")]
        public List<Operation> Operation { get; set; }
    }
}