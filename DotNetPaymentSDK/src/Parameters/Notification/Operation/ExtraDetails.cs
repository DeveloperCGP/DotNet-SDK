using System.Runtime.Serialization;

namespace DotNetPaymentSDK.src.Parameters.Notification.Operation
{
    [DataContract]
    public class ExtraDetails
    {
        [DataMember(Name = "nemuruTxnId", IsRequired = false)]
        public string? NemuruTxnId { get; set; }
        [DataMember(Name = "nemuruCartHash", IsRequired = false)]
        public string? NemuruCartHash { get; set; }
        [DataMember(Name = "nemuruAuthToken", IsRequired = false)]
        public string? NemuruAuthToken { get; set; }
        [DataMember(Name = "nemuruDisableFormEdition", IsRequired = false)]
        public string? NemuruDisableFormEdition { get; set; }
        [DataMember(Name = "status", IsRequired = false)]
        public string? Status { get; set; }
        [DataMember(Name = "disableFormEdition", IsRequired = false)]
        public string? DisableFormEdition { get; set; }
    }
}