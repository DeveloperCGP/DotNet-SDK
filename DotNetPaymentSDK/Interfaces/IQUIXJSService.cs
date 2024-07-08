using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.Quix_JS;

namespace DotNetPaymentSDK.Interfaces
{
    public interface IQUIXJSService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; }
        public Task SendJSQuixServiceRequest(JSQuixService parameters, IResponseListener responseListener);
        public Task SendJSQuixFlightRequest(JSQuixFlight parameters, IResponseListener responseListener);
        public Task SendJSQuixProductRequest(JSQuixProduct parameters, IResponseListener responseListener);
        public Task SendJSQuixAccommodationRequest(JSQuixAccommodation parameters, IResponseListener responseListener);
    }
}