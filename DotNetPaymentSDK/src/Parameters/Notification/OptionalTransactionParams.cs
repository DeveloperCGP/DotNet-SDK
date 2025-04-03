using System.Xml.Serialization;
using DotNetPaymentSDK.src.Parameters.Notification.OperationsModels;

namespace DotNetPaymentSDK.src.Parameters.Notification
{
    public class OptionalTransactionParams
    {
        [XmlElement(ElementName = "entry", IsNullable = true)]
        public List<Entry>? Entry { get; set; }
    }
}