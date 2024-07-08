using DotNetPaymentSDK.src.Parameters.Quix_Models;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Product;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_JS
{
    public class JSQuixProduct : QuixJSRequest
    {
        [JsonProperty("paysolExtendedData")]
        public QuixProductPaySolExtendedData PaySolExtendedData { get; private set; } = null;

        public JSQuixProduct() : base()
        {
        }

        public void SetPaySolExtendedData(QuixProductPaySolExtendedData paySolExtendedData)
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