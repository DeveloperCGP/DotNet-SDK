using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.Quix_Hosted;

namespace DotNetPaymentSDK.Interfaces
{
    public interface IQUIXHostedService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; }
        public Task SendHostedQuixServiceRequest(HostedQuixService parameters, IResponseListener responseListener);
        public Task SendHostedQuixFlightRequest(HostedQuixFlight parameters, IResponseListener responseListener);
        public Task SendHostedQuixAccommodationRequest(HostedQuixAccommodation parameters, IResponseListener responseListener);
        public Task SendHostedQuixProductRequest(HostedQuixProduct parameters, IResponseListener responseListener);
    }
}