using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Service;

namespace DotNetPaymentSDK.src.Parameters.Quix_Hosted
{
    public class HostedQuixService : QuixHostedRequest
    {
        public QuixServicePaySolExtendedData PaysolExtendedData { get; private set; } = null;

        public HostedQuixService() : base()
        {
        }

        public void SetPaySolExtendedData(QuixServicePaySolExtendedData paySolExtendedData)
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