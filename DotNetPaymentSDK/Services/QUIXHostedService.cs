using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.Interfaces;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.Quix_Hosted;
using DotNetPaymentSDK.src.Requests.Utils;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;
using System.Text;

namespace DotNetPaymentSDK.Services
{
    public class QUIXHostedService: IQUIXHostedService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; } = new NetworkAdapter();

        public ISecurityUtils SecurityUtils { get; set; } = new SecurityUtilsImpl();

        public async Task SendHostedQuixServiceRequest(HostedQuixService parameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = parameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            parameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = parameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }
            
            var extendData = parameters.PaysolExtendedData;
            var tempParameter = parameters;
            tempParameter.SetPaySolExtendedData(null);
            string httpQuery = GeneralUtils.ToQueryString(tempParameter);
            httpQuery += "&paysolExtendedData=" + GeneralUtils.EncodeUrl(JsonConvert.SerializeObject(extendData, GeneralUtils.JSONSettings));
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

        public async Task SendHostedQuixFlightRequest(HostedQuixFlight parameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = parameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            parameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = parameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }
            
            var extendData = parameters.PaysolExtendedData;
            var tempParameter = parameters;
            tempParameter.SetPaySolExtendedData(null);
            string httpQuery = GeneralUtils.ToQueryString(tempParameter);
            httpQuery += "&paysolExtendedData=" + GeneralUtils.EncodeUrl(JsonConvert.SerializeObject(extendData, GeneralUtils.JSONSettings));
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

        public async Task SendHostedQuixAccommodationRequest(HostedQuixAccommodation parameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = parameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            parameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = parameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }
            
            var extendData = parameters.PaysolExtendedData;
            var tempParameter = parameters;
            tempParameter.SetPaySolExtendedData(null);
            string httpQuery = GeneralUtils.ToQueryString(tempParameter);
            httpQuery += "&paysolExtendedData=" + GeneralUtils.EncodeUrl(JsonConvert.SerializeObject(extendData, GeneralUtils.JSONSettings));
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

        public async Task SendHostedQuixProductRequest(HostedQuixProduct parameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = parameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            parameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = parameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            var extendData = parameters.PaysolExtendedData;
            var tempParameter = parameters;
            tempParameter.SetPaySolExtendedData(null);
            string httpQuery = GeneralUtils.ToQueryString(tempParameter);
            httpQuery += "&paysolExtendedData=" + GeneralUtils.EncodeUrl(JsonConvert.SerializeObject(extendData, GeneralUtils.JSONSettings));
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
