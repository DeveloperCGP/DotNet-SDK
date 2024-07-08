namespace DotNetPaymentSDK.src.Exceptions
{
    public class MissingFieldsException(string message) : FieldException(message)
    {
        public static string CreateMessage(string field, bool isCred)
        {
            if (isCred)
            {
                return "Mandatory credentials are missing. Please ensure you provide: " + field;
            }
            else
            {
                return "Missing " + field;
            }
        }

    }
}