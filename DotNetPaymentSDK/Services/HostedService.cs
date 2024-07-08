using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.Interfaces;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.Hosted;
using DotNetPaymentSDK.src.Requests.Utils;
using DotNetPaymentSDK.Utilities;
using System.Text;

namespace DotNetPaymentSDK.Services
{
    public class HostedService : IHostedService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; } = new NetworkAdapter();
        public ISecurityUtils SecurityUtils { get; set; } = new SecurityUtilsImpl();
        public async Task SendHostedPaymentRequest(HostedPaymentRedirection hostedPaymentParameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = hostedPaymentParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            hostedPaymentParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = hostedPaymentParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string httpQuery = GeneralUtils.ToQueryString(hostedPaymentParameters);
            Console.WriteLine("Clear Query = " + httpQuery);
            string formattedRequest = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF8.GetBytes(httpQuery))) ?? throw new Exception("Formatted request data is not a string.");
            byte[] iv = SecurityUtils.GenerateIV();
            string base64Iv = Convert.ToBase64String(iv);
            byte[] encryptedByteArray = SecurityUtils.CbcEncryption(formattedRequest, Credentials.GetMerchantPass(), iv);
            string encryptedRequest = Convert.ToBase64String(encryptedByteArray) ?? throw new Exception("Encryption failed.");
            string signature = SecurityUtils.Sha256Hash(formattedRequest) ?? throw new Exception("Signature failed.");

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.HOSTED_STG : RequestsPaths.HOSTED_PROD;

            var data = new Dictionary<string, string>
                {
                    { "merchantId", Credentials.GetMerchantId() },
                    { "encrypted", encryptedRequest },
                    { "integrityCheck", signature }
                };

            var headers = new Dictionary<string, string>
                {
                    { "apiVersion", Credentials.GetApiVersion().ToString() },
                    { "encryptionMode", "CBC" },
                    { "iv", base64Iv }
                };

            try
            {
                var requestHttpClient = await NetworkAdapter.RequestHttpClient(url, data, headers);
                var response = requestHttpClient["response"];
                _ = int.TryParse(requestHttpClient["status_code"], out int statusCode);

                if (StatusCodes.IsSuccess(statusCode))
                {
                    try
                    {
                        string redirectionUrl = response;
                        Console.WriteLine(redirectionUrl);
                        if (GeneralUtils.IsValidURL(redirectionUrl))
                        {
                            responseListener.OnRedirectionURLReceived(redirectionUrl);
                        }
                        else
                        {
                            responseListener.OnError(ErrorsEnum.INVALID_RESPONSE_RECEIVED, "Invalid Response Received");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        responseListener.OnError(ErrorsEnum.INVALID_RESPONSE_RECEIVED, "Invalid Response Received");
                    }
                }
                else
                {
                    string errorMessage = "Status code is " + statusCode;
                    if (!string.IsNullOrEmpty(response))
                    {
                        errorMessage = response.ToString();
                    }
                    responseListener.OnError(StatusCodes.IsClientError(statusCode) ? ErrorsEnum.CLIENT_ERROR : ErrorsEnum.SERVER_ERROR, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                responseListener.OnError(ErrorsEnum.NETWORK_ERROR, ex.ToString());
            }
        }

        public async Task SendHostedRecurrentInitial(HostedPaymentRecurrentInitial hostedPaymentRecurrentInitial, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = hostedPaymentRecurrentInitial.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            hostedPaymentRecurrentInitial.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = hostedPaymentRecurrentInitial.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string httpQuery = GeneralUtils.ToQueryString(hostedPaymentRecurrentInitial);
            Console.WriteLine("Clear Query = " + httpQuery);
            string formattedRequest = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF8.GetBytes(httpQuery))) ?? throw new Exception("Formatted request data is not a string.");
            byte[] iv = SecurityUtils.GenerateIV();
            string base64Iv = Convert.ToBase64String(iv);
            byte[] encryptedByteArray = SecurityUtils.CbcEncryption(formattedRequest, Credentials.GetMerchantPass(), iv);
            string encryptedRequest = Convert.ToBase64String(encryptedByteArray) ?? throw new Exception("Encryption failed.");
            string signature = SecurityUtils.Sha256Hash(formattedRequest) ?? throw new Exception("Signature failed.");

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.HOSTED_STG : RequestsPaths.HOSTED_PROD;

            var data = new Dictionary<string, string>
                {
                    { "merchantId", Credentials.GetMerchantId() },
                    { "encrypted", encryptedRequest },
                    { "integrityCheck", signature }
                };

            var headers = new Dictionary<string, string>
                {
                    { "apiVersion", Credentials.GetApiVersion().ToString() },
                    { "encryptionMode", "CBC" },
                    { "iv", base64Iv }
                };

            try
            {
                var requestHttpClient = await NetworkAdapter.RequestHttpClient(url, data, headers);
                var response = requestHttpClient["response"];
                _ = int.TryParse(requestHttpClient["status_code"], out int statusCode);

                if (StatusCodes.IsSuccess(statusCode))
                {
                    try
                    {
                        string redirectionUrl = response;
                        Console.WriteLine(redirectionUrl);
                        if (GeneralUtils.IsValidURL(redirectionUrl))
                        {
                            responseListener.OnRedirectionURLReceived(redirectionUrl);
                        }
                        else
                        {
                            responseListener.OnError(ErrorsEnum.INVALID_RESPONSE_RECEIVED, "Invalid Response Received");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        responseListener.OnError(ErrorsEnum.INVALID_RESPONSE_RECEIVED, "Invalid Response Received");
                    }
                }
                else
                {
                    string errorMessage = "Status code is " + statusCode;
                    if (!string.IsNullOrEmpty(response))
                    {
                        errorMessage = response.ToString();
                    }
                    responseListener.OnError(StatusCodes.IsClientError(statusCode) ? ErrorsEnum.CLIENT_ERROR : ErrorsEnum.SERVER_ERROR, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                responseListener.OnError(ErrorsEnum.NETWORK_ERROR, ex.ToString());
            }
        }
    }
}