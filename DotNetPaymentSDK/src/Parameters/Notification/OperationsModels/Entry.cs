using System.Runtime.Serialization;

namespace DotNetPaymentSDK.src.Parameters.Notification.OperationsModels
{
    [DataContract]
    public class Entry
    {
        [DataMember(Name = "key", IsRequired = false)]
        public string Key { get; set; }
        [DataMember(Name = "value", IsRequired = false)]
        public string Value { get; set; }
    }
}