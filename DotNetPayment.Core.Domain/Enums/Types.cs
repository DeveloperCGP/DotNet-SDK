namespace DotNetPaymentSDK.Config.Enums
{
    public class Types
    {
        public const string MOTO = "MOTO";
        public const string ECOM = "ECOM";

        public static bool IsValid(string type)
        {
            return Array.Exists(typeof(Types).GetFields(), field => field.GetValue(null).Equals(type));
        }
    }
}
