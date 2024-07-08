using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Parameters.JS;

namespace DotNetPaymentSDK.Callbacks
{
    public interface IJSPaymentListener 
    {
        void OnError(ErrorsEnum error, string message);

        void OnAuthorizationResponseReceived(string rawResponse, JSAuthorizationResponse jsAuthorizationResponse);
    }
}