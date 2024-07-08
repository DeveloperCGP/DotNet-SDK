using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models
{
    public class QuixJSRequest : QuixHostedRequest
    {
        [JsonProperty("prepayToken")]
        public string PrepayToken { get; private set; } = null;

        public void SetPrepayToken(string prepayToken)
        {
            PrepayToken = prepayToken;
        }

        public override Tuple<bool, string> IsMissingField()
        {
            if (string.IsNullOrEmpty(PrepayToken?.Trim()))
            {
                return new(true, "prepayToken");
            }
            return base.IsMissingField();
        }
    }
}