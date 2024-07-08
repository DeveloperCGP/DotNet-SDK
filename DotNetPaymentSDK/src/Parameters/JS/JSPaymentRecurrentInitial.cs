using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Enums;

namespace DotNetPaymentSDK.src.Parameters.JS
{
    public class JSPaymentRecurrentInitial : JSChargeParameters
    {
        private PaymentRecurringType PaymentRecurringType = PaymentRecurringType.newCof;
        private string ChallengeInd = ((int)ChallengeIndEnum._04).ToString("D2");

        public JSPaymentRecurrentInitial(): base()
        {
        }

        public PaymentRecurringType GetPaymentRecurringType()
        {
            return PaymentRecurringType;
        }

        public void SetPaymentRecurringType(PaymentRecurringType paymentRecurringType)
        {
            PaymentRecurringType = paymentRecurringType;
        }

        public ChallengeIndEnum? GetChallengeInd()
        {
            return EnumsUtils.GetChallengeIndEnum(ChallengeInd);
        }

        public void SetChallengeInd(ChallengeIndEnum challengeInd)
        {
            ChallengeInd = ((int)challengeInd).ToString("D2");
        }

        public override Tuple<bool, string> IsMissingField()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                {"challengeInd", ChallengeInd},
                {"paymentRecurringType", PaymentRecurringType.ToString()}
            };

            foreach (var field in mandatoryFields)
            {
                if (field.Value == null)
                {
                    return new(true, field.Key);
                }
            }
            return base.IsMissingField();
        }
    }
}