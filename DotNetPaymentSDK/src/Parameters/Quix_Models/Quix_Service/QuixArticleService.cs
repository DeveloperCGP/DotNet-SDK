using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Service
{
    public class QuixArticleService
    {
        [JsonProperty("name")]
        public string Name { get; private set; } = null;
        [JsonProperty("type")]
        public readonly string Type = "service";
        [JsonProperty("start_date")]
        public string StartDate { get; private set; } = null;
        [JsonProperty("end_date")]
        public string EndDate { get; private set; } = null;
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
            if (unitPriceWithTax < 0)
            {
                throw new InvalidFieldException("unitPriceWithTax: Value must be (unitPriceWithTax >= 0)");
            }
            UnitPriceWithTax = GeneralUtils.ParseDoubleAmount(GeneralUtils.RoundAmount(unitPriceWithTax));
        }

        public void SetUnitPriceWithTax(string unitPriceWithTax)
        {
            var parsedAmount = GeneralUtils.ParseAmount(unitPriceWithTax) ?? throw new InvalidFieldException("unitPriceWithTax: Value must be (unitPriceWithTax >= 0)");
            var doubleValue = GeneralUtils.ParseDoubleAmount(parsedAmount);
            if (doubleValue < 0)
            {
                throw new InvalidFieldException("unitPriceWithTax: Value must be (unitPriceWithTax >= 0)");
            }
            UnitPriceWithTax = doubleValue;
        }

        public void SetStartDate(string startDate)
        {
            StartDate = startDate;
        }

        public void SetEndDate(string endDate)
        {
            EndDate = endDate;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Url = GeneralUtils.EncodeUrl(url);
            }
            else
            {
                Url = null;
            }
        }

        public void SetImageUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl.Trim()))
            {
                ImageUrl = GeneralUtils.EncodeUrl(imageUrl);
            }
            else
            {
                ImageUrl = null;
            }
        }

        public void SetTotalDiscount(double totalDiscount)
        {
            if (totalDiscount < 0)
            {
                throw new InvalidFieldException("totalDiscount: Value must be (totalDiscount >= 0)");
            }
            TotalDiscount = GeneralUtils.ParseDoubleAmount(GeneralUtils.RoundAmount(totalDiscount));
        }

        public void SetTotalDiscount(string totalDiscount)
        {
            var parsedAmount = GeneralUtils.ParseAmount(totalDiscount) ?? throw new InvalidFieldException("totalDiscount: Value must be (totalDiscount >= 0)");
            var doubleValue = GeneralUtils.ParseDoubleAmount(parsedAmount);
            if (doubleValue < 0)
            {
                throw new InvalidFieldException("totalDiscount: Value must be (totalDiscount >= 0)");
            }
            TotalDiscount = doubleValue;
        }

        public virtual Tuple<bool, string> IsMissingField()
        {
            if (UnitPriceWithTax <= 0)
            {
                return new(true, "unitPriceWithTax");
            }

            var mandatoryFields = new Dictionary<string, string>()
            {
                { "name", Name},
                { "category", Category?.ToString() },
                { "reference", Reference },
                { "endDate", EndDate }
            };

            return GeneralUtils.ContainsNull(mandatoryFields);
        }
    }
}