using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Accommodation
{
    public class QuixArticleAccommodation
    {
        [JsonProperty("name")]
        public string Name { get; private set; } = null;
        [JsonProperty("type")]
        public readonly string Type = "accommodation";
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("category")]
        public CategoryEnum? Category { get; private set; } = null;
        [JsonProperty("reference")]
        public string Reference { get; private set; } = null;
        [JsonProperty("unit_price_with_tax")]
        public double UnitPriceWithTax { get; private set; } = -1;
        [JsonProperty("checkin_date")]
        public string CheckInDate { get; private set; } = null;
        [JsonProperty("checkout_date")]
        public string CheckOutDate { get; private set; } = null;
        [JsonProperty("establishment_name")]
        public string EstablishmentName { get; private set; } = null;
        [JsonProperty("address")]
        public QuixAddress Address { get; private set; } = null;
        [JsonProperty("guests")]
        public int Guests { get; private set; } = -1;
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

        public void SetCheckinDate(string checkinDate)
        {
            CheckInDate = checkinDate;
        }

        public void SetCheckoutDate(string checkoutDate)
        {
            CheckOutDate = checkoutDate;
        }

        public void SetEstablishmentName(String establishmentName)
        {
            EstablishmentName = establishmentName;
        }

        public void SetAddress(QuixAddress address)
        {
            Address = address;
        }

        public void SetGuests(int guests)
        {
            Guests = guests;
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
                { "name", Name },
                { "type", Type },
                { "category", Category?.ToString() },
                { "reference", Reference },
                { "unitPriceWithTax", UnitPriceWithTax.ToString() },
                { "checkinDate", CheckInDate },
                { "checkoutDate", CheckOutDate },
                { "establishmentName", EstablishmentName },
                { "guests", Guests.ToString() },
            };

            var missingField = GeneralUtils.ContainsNull(mandatoryFields);
            if (missingField.Item1)
            {
                return missingField;
            }

            return Address.IsMissingField();
        }
    }

}