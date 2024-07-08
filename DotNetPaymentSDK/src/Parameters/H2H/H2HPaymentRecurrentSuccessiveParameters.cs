using DotNetPayment.Core.Domain.Enums;

namespace DotNetPaymentSDK.src.Parameters.H2H
{
    public class H2HPaymentRecurrentSuccessiveParameters : H2HRedirectionParameters
    {
        private string SubscriptionPlan = null;
        private PaymentRecurringType PaymentRecurringType = PaymentRecurringType.cof;
        private MerchantExemptionsScaEnum? MerchantExemptionsSca = null;

        public H2HPaymentRecurrentSuccessiveParameters() : base()
        {
        }


        public string GetSubscriptionPlan()
        {
            return SubscriptionPlan;
        }

        public void SetSubscriptionPlan(string subscriptionPlan)
        {
            SubscriptionPlan = subscriptionPlan;
        }

        public void SetPaymentRecurringType(PaymentRecurringType paymentRecurringType)
        {
            PaymentRecurringType = paymentRecurringType;
        }

        public PaymentRecurringType? GetPaymentRecurringType()
        {
            return PaymentRecurringType;
        }

        public MerchantExemptionsScaEnum? GetMerchantExemptionsSca()
        {
            return MerchantExemptionsSca;
        }

        public void SetMerchantExemptionsSca(MerchantExemptionsScaEnum merchantExemptionsSca)
        {
            MerchantExemptionsSca = merchantExemptionsSca;
        }

        public override Tuple<bool, string> IsMissingField()
        {
            var mandatoryFields = new Dictionary<string, string>()
            {
                {"cardNumberToken", GetCardNumberToken()},
                {"subscriptionPlan", SubscriptionPlan},
                {"paymentRecurringType", PaymentRecurringType.ToString()},
                {"merchantExemptionsSca", MerchantExemptionsSca?.ToString()}
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