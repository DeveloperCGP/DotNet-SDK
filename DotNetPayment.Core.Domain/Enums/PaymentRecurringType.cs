namespace DotNetPayment.Core.Domain.Enums
{
    public enum PaymentRecurringType
    {
        cof,
        subscription,
        installment,
        delayed,
        resubmission,
        reauthorization,
        incremental,
        noShow,
        newCof,
        newSubscription,
        newInstallment
    }
}