using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Product
{
    public class QuixItemCartItemProduct
    {
        [JsonProperty("article")]
        public QuixArticleProduct Article { get; private set; } = null;
        [JsonProperty("units")]
        public int Units { get; private set; } = -1;
        [JsonProperty("total_price_with_tax")]
        public double TotalPriceWithTax { get; private set; } = -1;
        [JsonProperty("auto_shipping")]
        public bool AutoShipping { get; private set; } = true;

        public void setArticle(QuixArticleProduct article)
        {
            Article = article;
        }

        public void SetUnits(int units)
        {
            Units = units;
        }

        public void SetTotalPriceWithTax(double total_price_with_tax)
        {
            TotalPriceWithTax = GeneralUtils.ParseDoubleAmount(GeneralUtils.RoundAmount(total_price_with_tax));
        }

        public void SetTotalPriceWithTax(string totalPriceWithTax)
        {
            TotalPriceWithTax = GeneralUtils.ParseDoubleAmount(GeneralUtils.ParseAmount(totalPriceWithTax));
        }

        public void SetAutoShipping(bool autoShipping)
        {
            AutoShipping = autoShipping;
        }

        public Tuple<bool, string> IsMissingField()
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