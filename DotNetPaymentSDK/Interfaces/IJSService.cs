using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.JS;

namespace DotNetPaymentSDK.Interfaces
{
    public interface IJSService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; }
        public Task SendJSAuthorizationRequest(JSAuthorizationRequestParameters jsAuthParameters, IJSPaymentListener jsPaymentListener);
        public Task SendJSChargeRequest(JSChargeParameters jsChargeParameters, IResponseListener responseListener);
        public Task SendJSPaymentRecurrentInitial(JSPaymentRecurrentInitial jsPaymentRecurrentInitial, IResponseListener responseListener);
    }
}