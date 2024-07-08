using DotNetPayment.Core.Domain.Enums;

namespace DotNetPaymentSDK.src.Enums
{
    public class EnumsUtils
    {
        public static ChallengeIndEnum? GetChallengeIndEnum(string stringValue)
        {
            foreach (ChallengeIndEnum value in Enum.GetValues(typeof(ChallengeIndEnum)))
            {
                if(value.ToString() == stringValue || ((int)value).ToString("D2") == stringValue)
                {
                    return value;
                }
            }
            return null;
        }

        public static TransactionResult? GetTransactionResultEnum(string stringValue)
        {
            foreach (TransactionResult value in Enum.GetValues(typeof(TransactionResult)))
            {
                if(value.ToString() == stringValue)
                {
                    return value;
                }
            }
            return null;
        }
    }
}