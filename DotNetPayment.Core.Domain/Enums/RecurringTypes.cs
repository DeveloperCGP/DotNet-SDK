namespace DotNetPaymentSDK.Config.Enums
{
    public static class RecurringTypes
    {
        public const string NEW_COF = "newCof";
        public const string COF = "cof";
        public const string NEW_SUBSCRIPTION = "newSubscription";
        public const string SUBSCRIPTION = "subscription";
        public const string NEW_INSTALLMENT = "newInstallment";
        public const string INSTALLMENT = "installment";

        // ... add other country codes as needed

        public static bool IsValid(string type)
        {
            return Array.Exists(typeof(RecurringTypes).GetFields(), field => field.GetValue(null).Equals(type));
        }
    }
}
