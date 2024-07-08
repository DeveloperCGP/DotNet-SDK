using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models
{
    public class QuixBilling
    {
        [JsonProperty("first_name")]
        public string FirstName { get; private set; } = null;
        [JsonProperty("last_name")]
        public string LastName { get; private set; } = null;
        [JsonProperty("address")]
        public QuixAddress Address { get; private set; } = null;
        [JsonProperty("corporate_id_number")]
        public string CorporateIdNumber { get; private set; } = null;

        public void SetFirstName(string firstName)
        {
            FirstName = firstName;
        }

        public void SetLastName(string lastName)
        {
            LastName = lastName;
        }

        public void SetAddress(QuixAddress address)
        {
            Address = address;
        }

        public void SetCorporateIdNumber(string corporateIdNumber)
        {
            CorporateIdNumber = corporateIdNumber;
        }

        public virtual Tuple<bool, string> IsMissingField()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                {"firstName", FirstName},
                {"lastName", LastName}
            };

            var containsNull = GeneralUtils.ContainsNull(mandatoryFields);
            if (containsNull.Item1)
            {
                return containsNull;
            }
            
            if (Address == null)
            {
                return new(true, "address");
            }
            else
            {
                return Address.IsMissingField();
            }
        }
    }
}