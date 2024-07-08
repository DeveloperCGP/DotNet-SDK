using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Product
{
    public class QuixArticleProduct
    {
        [JsonProperty("name")]
        public string Name { get; private set; } = null;
        [JsonProperty("type")]
        public readonly string type = "product";
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("category")]
        public CategoryEnum? Category { get; private set; } = null;
        [JsonProperty("reference")]
        public string Reference { get; private set; } = null;
        [JsonProperty("unit_price_with_tax")]
        public double UnitPriceWithTax { get; private set; } = -1;
        [JsonProperty("description")]
        public string Description { get; private set; } = null;
        [JsonProperty("url")]
        public string Url { get; private set; } = null;
        [JsonProperty("image_url")]
        public string ImageUrl { get; private set; } = null;
        [JsonProperty("total_discount")]
        public double TotalDiscount { get; private set; } = 0;
        [JsonProperty("brand")]
        public string Brand { get; private set; } = null;
        [JsonProperty("mpn")]
        public string Mpn { get; private set; } = null;
        [JsonProperty("shipping")]
        public QuixShipping Shipping { get; private set; } = null;
        [JsonProperty("address")]
        public QuixAddress Address { get; private set; } = null;

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetCategory(CategoryEnum category)
        {
            Category = category;
        }

        public void SetReference(string reference)
        {
            Reference = reference;
        }

        public void SetUnitPriceWithTax(double unitPriceWithTax)
        {
            UnitPriceWithTax = GeneralUtils.ParseDoubleAmount(GeneralUtils.RoundAmount(unitPriceWithTax));
        }

        public void SetUnitPriceWithTax(string unitPriceWithTax)
        {
            UnitPriceWithTax = GeneralUtils.ParseDoubleAmount(GeneralUtils.ParseAmount(unitPriceWithTax));
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetUrl(string url)
        {
            Url = url;
        }

        public void SetImageUrl(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public void SetTotalDiscount(double totalDiscount)
        {
            TotalDiscount = totalDiscount;
        }

        public void SetBrand(string brand)
        {
            Brand = brand;
        }

        public void SetMpn(string mpn)
        {
            Mpn = mpn;
        }

        public void SetShipping(QuixShipping shipping)
        {
            Shipping = shipping;
        }

        public void SetAddress(QuixAddress address)
        {
            Address = address;
        }

        public Tuple<bool, string> IsMissingField()
        {
            if (UnitPriceWithTax <= 0)
            {
                return new(true, "unitPriceWithTax");
            }

            var mandatoryFields = new Dictionary<string, string>()
            {
                { "name", Name},
                { "category", Category?.ToString() },
                { "reference", Reference }
            };

            return GeneralUtils.ContainsNull(mandatoryFields);
        }
    }
}