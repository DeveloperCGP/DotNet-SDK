using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Product
{
    public class QuixProductPaySolExtendedData
    {
        [JsonProperty("product")]
        public string Product { get; private set; } = null;
        [JsonProperty("disableFormEdition")]
        public bool DisableFormEdition { get; private set; } = false;
        [JsonProperty("confirmation_card_data")]
        public ConfirmationCartData ConfirmationCardData { get; private set; } = null;
        [JsonProperty("customer")]
        public Customer Customer { get; private set; } = null;
        [JsonProperty("billing")]
        public QuixBilling Billing { get; private set; } = null;
        [JsonProperty("cart")]
        public QuixCartProduct Cart { get; private set; } = null;

        public void SetConfirmationCardData(ConfirmationCartData confirmationCardData)
        {
            ConfirmationCardData = confirmationCardData;
        }

        public void SetCustomer(Customer customer)
        {
            Customer = customer;
        }

        public void SetProduct(string product)
        {
            Product = product;
        }

        public void SetBilling(QuixBilling billing)
        {
            Billing = billing;
        }

        public void SetCart(QuixCartProduct cart)
        {
            Cart = cart;
        }

        public void SetDisableFormEdition(bool disableFormEdition)
        {
            DisableFormEdition = disableFormEdition;
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
            return Cart.IsMissingField();
        }

    }
}