namespace DotNetPaymentSDK.Config.Enums
{
    public static class LanguageCodes
    {
        public const string ES = "ES";
        public const string EN = "EN";

        // ... add other country codes as needed

        public static bool IsValid(string languageCodes)
        {
            return Array.Exists(typeof(LanguageCodes).GetFields(), field => field.GetValue(null).Equals(languageCodes));
        }
    }
}
