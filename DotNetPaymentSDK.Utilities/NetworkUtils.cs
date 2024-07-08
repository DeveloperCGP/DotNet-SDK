namespace DotNetPaymentSDK.Utilities
{
    public static class NetworkUtils
    {
        public static async Task<Dictionary<string, string>> RequestHttpClient(string url, Dictionary<string, string> data, Dictionary<string, string> headers)
        {
            using HttpClient client = new();
            var content = new FormUrlEncodedContent(data);
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            try
            {
                var response = await client.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return new Dictionary<string, string>
                {
                    { "response", responseContent },
                    { "status_code", ((int)response.StatusCode).ToString() },
                    { "message", "" }
                };
            }
            catch (Exception wz)
            {
                throw;
            }
        }

        public static async Task<Dictionary<string, string>> RequestHttpJSClient(string url, string authBodyString, Dictionary<string, string> headers)
        {
            using HttpClient client = new();
            var builder = new UriBuilder();
            var stringContent = new StringContent(authBodyString, System.Text.Encoding.UTF8, "application/json");
            try
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                var response = await client.PostAsync(url, stringContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                return new Dictionary<string, string>
                {
                    { "response", responseContent },
                    { "status_code", ((int)response.StatusCode).ToString() },
                    { "message", "" }
                };
            }
            catch (Exception wz)
            {

                throw;
            }
        }

        public static async Task<Dictionary<string, string>> RequestHttpChargeClient(string url, string authBodyString, Dictionary<string, string> headers)
        {
            using HttpClient client = new();
            var builder = new UriBuilder();
            var stringContent = new StringContent(authBodyString, System.Text.Encoding.UTF8, "application/json");
            try
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                var response = await client.PostAsync(url, stringContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != System.Net.HttpStatusCode.RedirectKeepVerb)
                {
                    Console.WriteLine($"Communication error occurred while processing the request. Details: {responseContent}");
                    throw new Exception(responseContent);
                }
                else
                {
                    Console.WriteLine("Request processed successfully.");
                }

                return new Dictionary<string, string>
                {
                    { "response", responseContent },
                    { "status_code", ((int)response.StatusCode).ToString() },
                    { "message", "" }
                };
            }
            catch (Exception ex)
            { 
                throw ex;
            }
        }
    }
}