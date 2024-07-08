using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight
{
    public class QuixFlightPaySolExtendedData
    {
        [JsonProperty("product")]
        private string product;
        [JsonProperty("disableFormEdition")]
        private bool disableFormEdition;
        [JsonProperty("confirmation_card_data")]
        private ConfirmationCartData confirmationCardData;
        [JsonProperty("customer")]
        private Customer customer;
        [JsonProperty("billing")]
        private QuixBilling billing;
        [JsonProperty("cart")]
        private QuixCartFlight cart;

        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        public bool DisableFormEdition
        {
            get { return disableFormEdition; }
            set { disableFormEdition = value; }
        }

        public ConfirmationCartData ConfirmationCardData
        {
            get { return confirmationCardData; }
            set { confirmationCardData = value; }
        }

        public Customer Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        public QuixBilling Billing
        {
            get { return billing; }
            set { billing = value; }
        }

        public QuixCartFlight Cart
        {
            get { return cart; }
            set { cart = value; }
        }

        public Tuple<bool, string> IsMissingField()
        {
            var mandatoryFields = new Dictionary<string, object>()
            {
                { "product", Product },
                { "billing", Billing },
                { "cart", Cart }
            };

            var missingField = GeneralUtils.ContainsNull(mandatoryFields);
            if (missingField.Item1)
            {
                return missingField;
            }
            
            missingField = Billing.IsMissingField();
            if (missingField.Item1)
            {
                return missingField;
            }
            
            return Cart.IsMissingFields();
        }
    }

}