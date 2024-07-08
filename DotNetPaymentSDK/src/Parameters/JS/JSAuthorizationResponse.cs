using System.Runtime.Serialization;

namespace DotNetPaymentSDK.src.Parameters.JS
{
    [DataContract]
    public class JSAuthorizationResponse
    {
        [DataMember(Name = "authToken", IsRequired = false)]
        public string AuthToken { get; set; }
    }
}