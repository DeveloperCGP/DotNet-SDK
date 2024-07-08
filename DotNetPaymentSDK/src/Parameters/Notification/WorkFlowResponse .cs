using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DotNetPaymentSDK.src.Parameters.Notification
{
    [DataContract]
    public class WorkFlowResponse 
    {
        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        [DataMember(Name = "name", IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Name = "version", IsRequired = false)]
        public string Version { get; set; }
    }
}