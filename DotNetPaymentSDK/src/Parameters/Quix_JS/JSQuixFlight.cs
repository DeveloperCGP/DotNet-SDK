using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Accommodation;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_JS
{
    public class JSQuixFlight : QuixJSRequest
    {
        [JsonProperty("paysolExtendedData")]
        public QuixFlightPaySolExtendedData PaySolExtendedData { get; private set; } = null;


        public JSQuixFlight() : base()
        {
        }

        public void SetPaySolExtendedData(QuixFlightPaySolExtendedData paySolExtendedData)
        {
            PaySolExtendedData = paySolExtendedData;
        }

        public Tuple<bool, string> IsMissingField()
        {
            if (PaySolExtendedData == null)
            {
                return new(true, "paySolExtendedData");
            }

            var missingField = PaySolExtendedData.IsMissingField();
            if (missingField.Item1)
            {
                return missingField;
            }

            return base.IsMissingField();
        }
    }
}