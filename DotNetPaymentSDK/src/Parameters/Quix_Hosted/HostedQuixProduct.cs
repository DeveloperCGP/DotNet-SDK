using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Product;

namespace DotNetPaymentSDK.src.Parameters.Quix_Hosted
{
    public class HostedQuixProduct : QuixHostedRequest
    {
        public QuixProductPaySolExtendedData PaysolExtendedData { get; private set; } = null;

        public HostedQuixProduct() : base()
        {
        }

        public void SetPaySolExtendedData(QuixProductPaySolExtendedData paySolExtendedData)
        {
            PaysolExtendedData = paySolExtendedData;
        }

        public Tuple<bool, string> IsMissingField()
        {
            if (PaysolExtendedData == null)
            {
                return new(true, "paySolExtendedData");
            }

            var missingField = PaysolExtendedData.IsMissingField();
            if (missingField.Item1)
            {
                return missingField;
            }

            return base.IsMissingField();
        }
    }
}