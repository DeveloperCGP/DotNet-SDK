using DotNetPaymentSDK.Interfaces;
using DotNetPaymentSDK.Utilities;

namespace DotNetPaymentSDK.src.Adapters
{
    public class NetworkAdapter : INetworkAdapter
    {
        public Task<Dictionary<string, string>> RequestHttpChargeClient(string url, string authBodyString, Dictionary<string, string> headers)
        {
            return NetworkUtils.RequestHttpChargeClient(url, authBodyString, headers);
        }

        public Task<Dictionary<string, string>> RequestHttpClient(string url, Dictionary<string, string> data, Dictionary<string, string> headers)
        {
            return NetworkUtils.RequestHttpClient(url, data, headers);
        }

        public Task<Dictionary<string, string>> RequestHttpJSClient(string url, string authBodyString, Dictionary<string, string> headers)
        {
            return NetworkUtils.RequestHttpJSClient(url, authBodyString, headers);
        }
    }
}