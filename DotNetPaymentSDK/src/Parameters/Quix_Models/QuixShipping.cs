using DotNetPayment.Core.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models
{
    public class QuixShipping
    {
        [JsonProperty("name")]
        public string Name { get; private set; } = null;
        [JsonProperty("first_name")]
        public string FirstName { get; private set; } = null;
        [JsonProperty("last_name")]
        public string LastName { get; private set; } = null;
        [JsonProperty("company")]
        public string Company { get; private set; } = null;
        [JsonProperty("email")]
        public string Email { get; private set; } = null;
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; private set; } = null;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("method")]
        public MethodEnum? Method { get; private set; } = null;
        [JsonProperty("address")]
        public QuixAddress Address { get; private set; } = null;

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetFirstName(string firstName)
        {
            FirstName = firstName;
        }

        public void SetLastName(string lastName)
        {
            LastName = lastName;
        }

        public void SetCompany(string company)
        {
            Company = company;
        }

        public void SetEmail(string email)
        {
            Email = email;
        }

        public void SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public void SetMethod(MethodEnum method)
        {
            Method = method;
        }

        public void SetAddress(QuixAddress address)
        {
            Address = address;
        }
    }
}