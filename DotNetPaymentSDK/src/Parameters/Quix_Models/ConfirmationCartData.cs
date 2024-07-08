using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models
{
    public class ConfirmationCartData
    {
        [JsonProperty("url")]
        public string URL { get; private set; } = null;

        public void SetUrl(string url)
        {
            if (!GeneralUtils.IsValidURL(url))
            {
                throw new InvalidFieldException("errorURL");
            }
            URL = url;
        }
    }
}