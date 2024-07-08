using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight;

namespace DotNetPaymentSDK.src.Parameters.Quix_Hosted
{
    public class HostedQuixFlight : QuixHostedRequest
    {
        public QuixFlightPaySolExtendedData PaysolExtendedData { get; private set; } = null;

        public HostedQuixFlight() : base()
        {
        }

        public void SetPaySolExtendedData(QuixFlightPaySolExtendedData paySolExtendedData)
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