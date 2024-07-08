namespace DotNetPayment.Core.Domain.Enums
{
    public class StatusCodes
    {
        public const int SUCCESS = 200;
        public const int MAX_SUCCESS = 299;
        public const int REDIRECT = 307;
        public const int MIN_CLIENT_ERROR = 400;
        public const int MAX_CLIENT_ERROR = 499;

        public static bool IsSuccess(int statusCode)
        {
            if ((statusCode >= SUCCESS && statusCode <= MAX_SUCCESS) || statusCode == REDIRECT)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsClientError(int statusCode)
        {
            if (statusCode >= MIN_CLIENT_ERROR && statusCode <= MAX_CLIENT_ERROR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}