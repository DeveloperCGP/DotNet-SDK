namespace DotNetPaymentSDK.Config.Enums
{
    public static class ItemTypes
    {
        public const string PRODUCT = "product";
        public const string SERVICE = "service";
        public const string FLIGHT = "flight";
        public const string ACCOMMODATION = "accommodation";

        // ... add other item types as needed

        public static bool IsValid(string type)
        {
            return Array.Exists(typeof(ItemTypes).GetFields(), field => field.GetValue(null).Equals(type));
        }
    }
}

