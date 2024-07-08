using System.Runtime.Serialization;

namespace DotNetPaymentSDK.src.Parameters.Notification
{
    [DataContract]
    public class NotificationInnerResponse
    {
        [DataMember(Name = "response", IsRequired = false)]
        public string Response { get; set; }
    }
}