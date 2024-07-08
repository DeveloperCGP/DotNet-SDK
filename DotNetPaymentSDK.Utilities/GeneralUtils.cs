using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.Utilities
{
    public static class GeneralUtils
    {
        public static readonly JsonSerializerSettings JSONSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        private static readonly string amountFormat = "0.0000";
        public static readonly Random random = new();

        public static readonly int MAX_MERCHANT_TRANSACTION_ID = 44;

        public static string RandomString()
        {
            int length = random.Next(8, MAX_MERCHANT_TRANSACTION_ID + 1);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ToQueryString(object obj)
        {
            Type type = obj.GetType();
            var queryString = new StringBuilder();

            while (type != null)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (FieldInfo field in fields)
                {
                    string name = field.Name.Replace("k__BackingField", "").Replace("<", "").Replace(">", "");
                    object value = field.GetValue(obj);
                    if (value != null)
                    {
                        if (queryString.Length > 0)
                        {
                            queryString.Append('&');
                        }

                        if (value is System.Collections.IEnumerable enumerable && !(value is string) && !string.Equals(name, "MerchantParams", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var item in enumerable)
                            {
                                queryString.AppendFormat("{0}={1}&", System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(name), System.Web.HttpUtility.UrlEncode(item.ToString()));
                            }
                            queryString.Length--; // Remove the last '&'
                        }
                        else
                        {
                            if (string.Equals(name, "MerchantParams", StringComparison.OrdinalIgnoreCase))
                            {
                                value = MerchantParamsQuery((List<Tuple<string, string>>) value);
                            }
                            queryString.AppendFormat("{0}={1}", System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(name), System.Web.HttpUtility.UrlEncode(value.ToString()));
                        }
                    }
                }

                type = type.BaseType;
            }

            return queryString.ToString().Replace("%3B", ";").Replace("%3b", ";").Replace("%3A", ":").Replace("%3a", ":").Replace("%2C", ",").Replace("%2c", ",");
        }

        public static string GetQUIXHttpQuery(Dictionary<string, string> dictionary)
        {
            var encodedPairs = new List<string>();
            foreach (var kvp in dictionary)
            {
                string encodedKey = HttpUtility.UrlEncode(kvp.Key);
                string encodedValue = HttpUtility.UrlEncode(kvp.Value);
                encodedPairs.Add($"{encodedKey}={encodedValue}");
            }
            return string.Join("&", encodedPairs);
        }

        public static string ParseAmount(string amount)
        {
            if (amount.Contains(','))
            {
                return null;
            }
            try
            {
                if (!double.TryParse(amount, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out double doubleAmount))
                {
                    return null;
                }
                if (doubleAmount < 0 || doubleAmount > 1000000)
                {
                    return null;
                }
                return RoundAmount(doubleAmount);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public static string RoundAmount(double doubleAmount)
        {
            return doubleAmount.ToString(amountFormat, CultureInfo.GetCultureInfo("en-US"));
        }

        public static double ParseDoubleAmount(string amountString)
        {
            return double.Parse(amountString, CultureInfo.GetCultureInfo("en-US"));
        }

        public static bool CheckLuhn(string cardNo)
        {
            int nDigits = cardNo.Length;

            int nSum = 0;
            bool isSecond = false;
            for (int i = nDigits - 1; i >= 0; i--)
            {

                int d = cardNo[i] - '0';

                if (isSecond == true)
                    d = d * 2;

                // We add two digits to handle
                // cases that make two digits 
                // after doubling
                nSum += d / 10;
                nSum += d % 10;

                isSecond = !isSecond;
            }
            return nSum % 10 == 0;
        }

        public static bool IsValidExpDate(string expDate)
        {
            if (string.IsNullOrEmpty(expDate) || expDate.Length != 4)
            {
                return false;
            }

            string month = expDate.Substring(0, 2);
            string year = expDate.Substring(2, 2);

            if (!IsNumbersOnly(month) || !IsNumbersOnly(year))
            {
                return false;
            }

            int monthInt = int.Parse(month);
            int yearInt = int.Parse(year);
            return monthInt >= 1 && monthInt <= 12 && yearInt >= 1;
        }

        public static bool IsNumbersOnly(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsValidURL(string url)
        {
            // Attempt to create a Uri from the provided string
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
            {
                // Check if the scheme is HTTP or HTTPS
                return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
            }
            return false;
        }

        public static string MerchantParamsQuery(List<Tuple<string, string>> merchantParams)
        {
            StringBuilder merchantParamsQuery = new();
            foreach (var parameter in merchantParams)
            {
                merchantParamsQuery.Append(parameter.Item1);
                merchantParamsQuery.Append(':');
                merchantParamsQuery.Append(parameter.Item2);
                merchantParamsQuery.Append(';');
            }

            // Remove the last semicolon
            if (merchantParamsQuery.Length > 0)
            {
                merchantParamsQuery.Length--;
            }

            return merchantParamsQuery.ToString();
        }

        public static Tuple<bool, string> ContainsNull<T>(Dictionary<string, T> values)
        {
            foreach (var field in values)
            {
                if (field.Value == null)
                {
                    return new(true, field.Key);
                }
            }
            return new(false, null);
        }

        public static bool IsValidIP(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }

        public static string EncodeUrl(string httpQuery)
        {
            string encodedQuery = HttpUtility.UrlEncode(httpQuery, Encoding.UTF8);
            return encodedQuery.Replace("%3D", "=")
                               .Replace("%26", "&")
                               .Replace("%3B", ";")
                               .Replace("%3A", ":")
                               .Replace("%2C", ",");
        }
    }
}