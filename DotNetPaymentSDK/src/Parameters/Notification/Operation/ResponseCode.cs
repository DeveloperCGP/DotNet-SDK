using System.Runtime.Serialization;

namespace DotNetPaymentSDK.src.Parameters.Notification.Operation
{
    [DataContract]
    public class ResponseCode
    {
        [DataMember(Name = "code", IsRequired = false)]
        public string? Code { get; set; }
        [DataMember(Name = "message", IsRequired = false)]
        public string? Message { get; set; }
        [DataMember(Name = "uuid", IsRequired = false)]
        public string? UUID { get; set; }
    }
}