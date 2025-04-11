using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Callbacks
{
    public interface IResponseListener
    {
        void OnError(ErrorsEnum error, string message);

        void OnResponseReceived(string rawResponse, Notification notification, TransactionResult transactionResult);

        void OnRedirectionURLReceived(string redirectionURL);
    }
}