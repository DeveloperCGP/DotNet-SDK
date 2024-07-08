using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models
{
    public class Customer
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("locale")]
        public Locale? Locale { get; private set; } = null;
        [JsonProperty("userAgent")]
        public string UserAgent { get; private set; } = null;
        [JsonProperty("title")]
        public string Title { get; private set; } = null;
        [JsonProperty("document_expiration_date")]
        public string DocumentExpirationDate { get; private set; } = null;
        [JsonProperty("logged_in")]
        public bool LoggedIn { get; private set; } = false;

        public void SetLocale(Locale locale)
        {
            Locale = locale;
        }

        public void SetUserAgent(string userAgent)
        {
            if (userAgent.Length > 256)
            {
                throw new InvalidFieldException("userAgent: Invalid Size, Size Must Be (userAgent <= 256)");
            }
            UserAgent = userAgent;
        }

        public void SetTitle(string title)
        {
            Title = title;
        }

        public void SetDocumentExpirationDate(string documentExpirationDate)
        {
            DocumentExpirationDate = documentExpirationDate;
        }

        public void SetLoggedIn(bool loggedIn)
        {
            LoggedIn = loggedIn;
        }
    }
}