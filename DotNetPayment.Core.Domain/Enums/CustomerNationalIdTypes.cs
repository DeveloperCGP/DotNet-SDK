namespace DotNetPaymentSDK.Config.Enums
{
    public class CustomerNationalIdTypes
    {
        public const string DNI = "DNI";
        public const string ID = "ID";
        public const string CC = "CC";
        public const string NIT = "NIT";
        public const string CE = "CE";
        public const string PASS = "PASS";
        public const string RUC = "RUC";

        public static bool IsValid(string customerNationalIdType)
        {
            return Array.Exists(typeof(CustomerNationalIdTypes).GetFields(), field => field.GetValue(null).Equals(customerNationalIdType));
        }
    }
}
