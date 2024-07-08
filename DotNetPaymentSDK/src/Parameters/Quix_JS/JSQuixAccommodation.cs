using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Accommodation;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_JS
{
    public class JSQuixAccommodation : QuixJSRequest
    {
        [JsonProperty("paysolExtendedData")]
        public QuixAccommodationPaySolExtendedData PaySolExtendedData { get; private set; } = null;


        public JSQuixAccommodation() : base()
        {
        }

        public void SetPaySolExtendedData(QuixAccommodationPaySolExtendedData paySolExtendedData)
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