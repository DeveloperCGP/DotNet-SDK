using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.H2H;

namespace DotNetPaymentSDK.Interfaces
{
    public interface IH2HService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; }
        public Task SendH2HRedirectionPaymentRequest(H2HRedirectionParameters h2hRedirectionParameters, IResponseListener responseListener);
        public Task SendH2hPreAuthorizationRequest(H2HPreAuthorizationParameters h2hPreAuthorizationParameters, IResponseListener responseListener);
        public Task SendH2hPreAuthorizationCapture(H2HPreAuthorizationCaptureParameters h2hPreAuthorizationCaptureParameters, IResponseListener responseListener);
        public Task SendH2hPaymentRecurrentInitial(H2HPaymentRecurrentInitialParameters h2hPaymentRecurrentInitialParameters, IResponseListener responseListener);
        public Task SendH2hPaymentRecurrentSuccessive(H2HPaymentRecurrentSuccessiveParameters h2hPaymentRecurrentSuccessiveParameters, IResponseListener responseListener);
        public Task SendH2hVoidRequest(H2HVoidParameters h2hVoidParameters, IResponseListener responseListener);
        public Task SendH2hRefundRequest(H2HRefundParameters h2hRefundParameters, IResponseListener responseListener);
        
    }
}