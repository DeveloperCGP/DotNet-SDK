using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Accommodation;

namespace DotNetPaymentSDK.src.Parameters.Quix_Hosted
{
    public class HostedQuixAccommodation : QuixHostedRequest
    {
        public QuixAccommodationPaySolExtendedData PaysolExtendedData { get; private set; } = null;

        public HostedQuixAccommodation() : base()
        {
        }

        public void SetPaySolExtendedData(QuixAccommodationPaySolExtendedData paySolExtendedData)
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