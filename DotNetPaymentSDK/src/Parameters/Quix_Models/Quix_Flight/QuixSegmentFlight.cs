using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight
{

    public class QuixSegmentFlight
    {
        [JsonProperty("iaa_departure_code")]
        public string IataDepartureCode { get; set; } = null;
        [JsonProperty("iata_destination_code")]
        public string IataDestinationCode { get; set; } = null;

        public void SetIataDepartureCode(string iataDepartureCode)
        {
            IataDepartureCode = iataDepartureCode;
        }

        public void SetIataDestinationCode(string iataDestinationCode)
        {
            IataDestinationCode = iataDestinationCode;
        }

        public virtual Tuple<bool, string> IsMissingFields()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                { "iaaDepartureCode", IataDepartureCode },
                { "iataDestinationCode", IataDestinationCode },
            };

            return GeneralUtils.ContainsNull(mandatoryFields);
        }
    }
}