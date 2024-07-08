using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetPaymentSDK.Interfaces
{
    public interface INetworkAdapter
    {
        public Task<Dictionary<string, string>> RequestHttpClient(string url, Dictionary<string, string> data, Dictionary<string, string> headers);
        public Task<Dictionary<string, string>> RequestHttpJSClient(string url, string authBodyString, Dictionary<string, string> headers);
        public Task<Dictionary<string, string>> RequestHttpChargeClient(string url, string authBodyString, Dictionary<string, string> headers);
    }
}