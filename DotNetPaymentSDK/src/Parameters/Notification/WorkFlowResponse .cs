using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DotNetPaymentSDK.src.Parameters.Notification
{
    [DataContract]
    [XmlRoot(ElementName = "workFlowResponse")]
    public class WorkFlowResponse 
    {
        [DataMember(Name = "id", IsRequired = false)]
        [XmlElement(ElementName = "id", IsNullable = true)]
        public string Id { get; set; }

        [DataMember(Name = "name", IsRequired = false)]
        [XmlElement(ElementName = "name", IsNullable = true)]
        public string Name { get; set; }

        [DataMember(Name = "version", IsRequired = false)]
        [XmlElement(ElementName = "version", IsNullable = true)]
        public string Version { get; set; }
    }
}