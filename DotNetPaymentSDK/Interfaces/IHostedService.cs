using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.Hosted;

namespace DotNetPaymentSDK.Interfaces
{
    public interface IHostedService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; }
        public Task SendHostedPaymentRequest(HostedPaymentRedirection hostedPaymentParameters, IResponseListener responseListener);
        public Task SendHostedRecurrentInitial(HostedPaymentRecurrentInitial hostedPaymentRecurrentInitial, IResponseListener responseListener);
    }
}