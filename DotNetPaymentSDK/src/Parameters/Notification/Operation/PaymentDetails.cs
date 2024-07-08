using System.Runtime.Serialization;

namespace DotNetPaymentSDK.src.Parameters.Notification.Operation
{
    [DataContract]
    public class PaymentDetails
    {
        [DataMember(Name = "cardNumberToken", IsRequired = false)]
        public string? CardNumberToken { get; set; }
        [DataMember(Name = "account", IsRequired = false)]
        public string? Account { get; set; }
        [DataMember(Name = "cardHolderName", IsRequired = false)]
        public string? CardHolderName { get; set; }
        [DataMember(Name = "cardNumber", IsRequired = false)]
        public string? CardNumber { get; set; }
        [DataMember(Name = "cardType", IsRequired = false)]
        public string? CardType { get; set; }
        [DataMember(Name = "expDate", IsRequired = false)]
        public string? ExpDate { get; set; }
        [DataMember(Name = "issuerBank", IsRequired = false)]
        public string? IssuerBank { get; set; }
        [DataMember(Name = "issuerCountry", IsRequired = false)]
        public string? IssuerCountry { get; set; }
        [DataMember(Name = "extraDetails", IsRequired = false)]
        public ExtraDetails? ExtraDetails { get; set; }
    }
}