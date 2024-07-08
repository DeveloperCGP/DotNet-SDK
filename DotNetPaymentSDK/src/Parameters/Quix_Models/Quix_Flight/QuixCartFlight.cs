using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;



namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight
{
    public class QuixCartFlight
    {
        [JsonProperty("total_price_with_tax")]
        public double TotalPriceWithTax { get; private set; } = 0;
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("currency")]
        public Currency? Currency { get; private set; } = null;
        [JsonProperty("reference")]
        public string Reference { get; private set; } = null;
        [JsonProperty("items")]
        public List<QuixItemCartItemFlight> Items { get; private set; } = [];

        public void SetCurrency(Currency currency)
        {
            Currency = currency;
        }

        public void SetTotalPriceWithTax(double totalPriceWithTax)
        {
            TotalPriceWithTax = GeneralUtils.ParseDoubleAmount(GeneralUtils.RoundAmount(totalPriceWithTax));
        }

        public void SetTotalPriceWithTax(string totalPriceWithTax)
        {
            TotalPriceWithTax = GeneralUtils.ParseDoubleAmount(GeneralUtils.ParseAmount(totalPriceWithTax));
        }

        public void SetItems(List<QuixItemCartItemFlight> items)
        {
            Items = items;
        }

        public void SetReference(string reference)
        {
            Reference = reference;
        }

        public virtual Tuple<bool, string> IsMissingFields()
        {
            if (TotalPriceWithTax <= 0)
            {
                return new(true, "totalPriceWithTax");
            }
            if (Currency == null)
            {
                return new(true, "currency");
            }
            if (Items == null || Items.Count == 0)
            {
                return new(true, "items");
            }

            foreach (QuixItemCartItemFlight item in Items)
            {
                var missingField = item.IsMissingFields();
                if (missingField.Item1)
                {
                    return missingField;
                }
            }

            return new(false, null);
        }


    }

}