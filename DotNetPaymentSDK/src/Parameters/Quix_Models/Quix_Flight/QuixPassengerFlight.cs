using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight
{

    public class QuixPassengerFlight
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; } = null;
        [JsonProperty("last_name")]
        public string LastName { get; set; } = null;

        public void SetFirstName(string firstName)
        {
            FirstName = firstName;
        }

        public void SetLastName(string lastName)
        {
            LastName = lastName;
        }

        public virtual Tuple<bool, string> IsMissingFields()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                { "firstName", FirstName },
                { "lastName", LastName },
            };

            return GeneralUtils.ContainsNull(mandatoryFields);
        }
    }
}