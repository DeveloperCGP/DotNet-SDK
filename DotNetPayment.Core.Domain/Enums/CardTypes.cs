namespace DotNetPaymentSDK.Config.Enums
{
    public static class CardTypes
    {
        public const string VISA = "VISA"; 

        public static bool IsValid(string cardType)
        {
            return typeof(CardTypes).GetField(cardType)?.GetValue(null) != null;
        }
    }
}