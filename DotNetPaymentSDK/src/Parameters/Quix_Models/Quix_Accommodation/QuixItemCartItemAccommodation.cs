using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Accommodation
{
    public class QuixItemCartItemAccommodation
    {
        [JsonProperty("units")]
        public int Units { get; private set; } = -1;
        [JsonProperty("total_price_with_tax")]
        public double TotalPriceWithTax { get; private set; } = -1;
        [JsonProperty("auto_shipping")]
        public bool AutoShipping { get; private set; } = true;
        [JsonProperty("article")]
        public QuixArticleAccommodation Article { get; private set; } = null;

        public void SetArticle(QuixArticleAccommodation article)
        {
            Article = article;
        }

        public void SetUnits(int units)
        {
            Units = units;
        }

        public void SetTotalPriceWithTax(double totalPriceWithTax)
        {
            TotalPriceWithTax = GeneralUtils.ParseDoubleAmount(GeneralUtils.RoundAmount(totalPriceWithTax));
        }

        public void SetTotalPriceWithTax(string totalPriceWithTax)
        {
            TotalPriceWithTax = GeneralUtils.ParseDoubleAmount(GeneralUtils.ParseAmount(totalPriceWithTax));
        }

        public bool IsAutoShipping()
        {
            return AutoShipping;
        }

        public void SetAutoShipping(bool autoShipping)
        {
            AutoShipping = autoShipping;
        }

        public virtual Tuple<bool, string> IsMissingField()
        {
            if (Units <= 0)
            {
                return new(true, "units");
            }
            if (TotalPriceWithTax <= 0)
            {
                return new(true, "totalPriceWithTax");
            }
            if (Article == null)
            {
                return new(true, "article");
            }

            return Article.IsMissingField();
        }

    }
}