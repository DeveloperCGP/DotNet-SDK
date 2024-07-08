using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models
{
    public class QuixAddress
    {
        [JsonProperty("street_address")]
        public string StreetAddress { get; private set; } = null;
        [JsonProperty("street_address2")]
        public string StreetAddress2 { get; private set; } = null;
        [JsonProperty("postal_code")]
        public string PostalCode { get; private set; } = null;
        [JsonProperty("city")]
        public string City { get; private set; } = null;
        [JsonProperty("country")]
        public string Country { get; private set; } = null;

        public void SetStreetAddress(string streetAddress)
        {
            StreetAddress = streetAddress;
        }

        public void SetPostalCode(string postalCode)
        {
            PostalCode = postalCode;
        }

        public void SetCity(string city)
        {
            City = city;
        }

        public void SetCountry(CountryCodeAlpha3 country)
        {
            Country = country.ToString();
        }

        public void SetStreetAddress2(string streetAddress2)
        {
            StreetAddress2 = streetAddress2;
        }

        public virtual Tuple<bool, string> IsMissingField()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                {"streetAddress", StreetAddress},
                {"postalCode", PostalCode},
                {"city", City},
                {"country", Country}
            };

            return GeneralUtils.ContainsNull(mandatoryFields);
        }
    }
}