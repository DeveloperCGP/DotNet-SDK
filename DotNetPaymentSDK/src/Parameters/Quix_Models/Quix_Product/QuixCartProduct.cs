using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Product
{
    public class QuixCartProduct
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("currency")]
        public Currency? Currency { get; private set; } = null;
        [JsonProperty("total_price_with_tax")]
        public double TotalPriceWithTax { get; private set; } = 0;
        [JsonProperty("items")]
        public List<QuixItemCartItemProduct> Items { get; private set; } = [];
        [JsonProperty("reference")]
        public string Reference { get; private set; } = null;

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

        public void SetItems(List<QuixItemCartItemProduct> items)
        {
            Items = items;
        }

        public void SetReference(String reference)
        {
            Reference = reference;
        }

        public Tuple<bool, string> IsMissingField()
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

            foreach (QuixItemCartItemProduct item in Items)
            {
                var missingField = item.IsMissingField();
                if (missingField.Item1)
                {
                    return missingField;
                }
            }

            return new(false, null);
        }


    }
}