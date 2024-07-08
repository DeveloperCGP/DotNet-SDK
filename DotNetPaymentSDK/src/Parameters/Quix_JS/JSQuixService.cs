using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Service;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_JS
{
    public class JSQuixService : QuixJSRequest
    {
        [JsonProperty("paysolExtendedData")]
        public QuixServicePaySolExtendedData PaySolExtendedData { get; private set; } = null;

        public JSQuixService() : base()
        {
        }

        public void SetPaySolExtendedData(QuixServicePaySolExtendedData paySolExtendedData)
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