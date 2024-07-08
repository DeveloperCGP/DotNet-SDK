using System.Runtime.Serialization;

namespace DotNetPaymentSDK.src.Parameters.Notification.Operation
{
    [DataContract]
    public class Mpi
    {
        [DataMember(Name = "acsTransID", IsRequired = false)]
        public string? AcsTransID { get; set; }
        [DataMember(Name = "authMethod", IsRequired = false)]
        public string? AuthMethod { get; set; }
        [DataMember(Name = "authTimestamp", IsRequired = false)]
        public string? AuthTimestamp { get; set; }
        [DataMember(Name = "authenticationStatus", IsRequired = false)]
        public string? AuthenticationStatus { get; set; }
        [DataMember(Name = "cavv", IsRequired = false)]
        public string? Cavv { get; set; }
        [DataMember(Name = "eci", IsRequired = false)]
        public string? Eci { get; set; }
        [DataMember(Name = "messageVersion", IsRequired = false)]
        public string? MessageVersion { get; set; }
        [DataMember(Name = "threeDSSessionData", IsRequired = false)]
        public string? ThreeDSSessionData { get; set; }
        [DataMember(Name = "threeDSv2Token", IsRequired = false)]
        public string? ThreeDSv2Token { get; set; }
    }
}