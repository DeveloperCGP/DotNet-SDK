using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight
{
    public class QuixArticleFlight
    {
        [JsonProperty("name")]
        public string Name { get; private set; } = null;
        [JsonProperty("type")]
        public readonly string Type = "flight";
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("category")]
        public CategoryEnum? Category { get; private set; } = null;
        [JsonProperty("reference")]
        public string Reference { get; private set; } = null;
        [JsonProperty("unit_price_with_tax")]
        public double UnitPriceWithTax { get; private set; } = -1;
        [JsonProperty("departure_date")]
        public string DepartureDate = null;
        [JsonProperty("passengers")]
        public List<QuixPassengerFlight> Passengers { get; private set; } = null;
        [JsonProperty("segments")]
        public List<QuixSegmentFlight> Segments { get; private set; } = null;


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

        public void SetDepartureDate(string departureDate)
        {
            DepartureDate = departureDate;
        }

        public void SetPassengers(List<QuixPassengerFlight> passengers)
        {
            Passengers = passengers;
        }

        public void SetSegments(List<QuixSegmentFlight> segments)
        {
            Segments = segments;
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
                { "departureDate", DepartureDate },
            };

            var missingField = GeneralUtils.ContainsNull(mandatoryFields);
            if (missingField.Item1)
            {
                return missingField;
            }

            if (Passengers.Count == 0)
            {
                return new(true, "passengers");
            }
            if (Segments.Count == 0)
            {
                return new(true, "segments");
            }

            foreach (QuixPassengerFlight item in Passengers)
            {
                missingField = item.IsMissingFields();
                if (missingField.Item1)
                {
                    return missingField;
                }
            }

            foreach (QuixSegmentFlight item in Segments)
            {
                missingField = item.IsMissingFields();
                if (missingField.Item1)
                {
                    return missingField;
                }
            }
            return new(false, null);
        }
    }

}