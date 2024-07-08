using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.Interfaces;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.H2H;
using DotNetPaymentSDK.src.Parameters.Nottification;
using DotNetPaymentSDK.src.Requests.Utils;
using DotNetPaymentSDK.Utilities;
using System.Text;

namespace DotNetPaymentSDK.Services
{
    public class H2HService: IH2HService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; } = new NetworkAdapter();
        public ISecurityUtils SecurityUtils { get; set; } = new SecurityUtilsImpl();

        public async Task SendH2HRedirectionPaymentRequest(H2HRedirectionParameters h2hRedirectionParameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = h2hRedirectionParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            h2hRedirectionParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = h2hRedirectionParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string httpQuery = GeneralUtils.ToQueryString(h2hRedirectionParameters);
            Console.WriteLine("Clear Query = " + httpQuery);
            string formattedRequest = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF8.GetBytes(httpQuery))) ?? throw new Exception("Formatted request data is not a string.");
            byte[] iv = SecurityUtils.GenerateIV();
            string base64Iv = Convert.ToBase64String(iv);
            byte[] encryptedByteArray = SecurityUtils.CbcEncryption(formattedRequest, Credentials.GetMerchantPass(), iv);
            string encryptedRequest = Convert.ToBase64String(encryptedByteArray) ?? throw new Exception("Encryption failed.");
            string signature = SecurityUtils.Sha256Hash(formattedRequest) ?? throw new Exception("Signature failed.");

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.H2H_STG : RequestsPaths.H2H_PROD;

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
                    if (!string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine($"Response received: {response}");
                        Notification notification = NotificationAdapter.ParseNotification(response);
                        responseListener.OnResponseReceived(response, notification, notification.GetTransactionResult());
                        return;
                    }
                    else
                    {
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

        public async Task SendH2hPreAuthorizationRequest(H2HPreAuthorizationParameters h2hPreAuthorizationParameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = h2hPreAuthorizationParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            h2hPreAuthorizationParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = h2hPreAuthorizationParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string httpQuery = GeneralUtils.ToQueryString(h2hPreAuthorizationParameters);
            Console.WriteLine("Clear Query = " + httpQuery);
            string formattedRequest = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF8.GetBytes(httpQuery))) ?? throw new Exception("Formatted request data is not a string.");
            byte[] iv = SecurityUtils.GenerateIV();
            string base64Iv = Convert.ToBase64String(iv);
            byte[] encryptedByteArray = SecurityUtils.CbcEncryption(formattedRequest, Credentials.GetMerchantPass(), iv);
            string encryptedRequest = Convert.ToBase64String(encryptedByteArray) ?? throw new Exception("Encryption failed.");
            string signature = SecurityUtils.Sha256Hash(formattedRequest) ?? throw new Exception("Signature failed.");

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.H2H_STG : RequestsPaths.H2H_PROD;

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
                    if (!string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine($"Response received: {response}");
                        Notification notification = NotificationAdapter.ParseNotification(response);
                        responseListener.OnResponseReceived(response, notification, notification.GetTransactionResult());
                        return;
                    }
                    else
                    {
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

        public async Task SendH2hPreAuthorizationCapture(H2HPreAuthorizationCaptureParameters h2hPreAuthorizationCaptureParameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = h2hPreAuthorizationCaptureParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            h2hPreAuthorizationCaptureParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = h2hPreAuthorizationCaptureParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string httpQuery = GeneralUtils.ToQueryString(h2hPreAuthorizationCaptureParameters);
            Console.WriteLine("Clear Query = " + httpQuery);
            string formattedRequest = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF8.GetBytes(httpQuery))) ?? throw new Exception("Formatted request data is not a string.");
            byte[] iv = SecurityUtils.GenerateIV();
            string base64Iv = Convert.ToBase64String(iv);
            byte[] encryptedByteArray = SecurityUtils.CbcEncryption(formattedRequest, Credentials.GetMerchantPass(), iv);
            string encryptedRequest = Convert.ToBase64String(encryptedByteArray) ?? throw new Exception("Encryption failed.");
            string signature = SecurityUtils.Sha256Hash(formattedRequest) ?? throw new Exception("Signature failed.");

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.CAPTURE_STG : RequestsPaths.CAPTURE_PROD;

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
                    if (!string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine($"Response received: {response}");
                        Notification notification = NotificationAdapter.ParseNotification(response);
                        responseListener.OnResponseReceived(response, notification, notification.GetTransactionResult());
                        return;
                    }
                    else
                    {
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

        public async Task SendH2hPaymentRecurrentInitial(H2HPaymentRecurrentInitialParameters h2hPaymentRecurrentInitialParameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = h2hPaymentRecurrentInitialParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            h2hPaymentRecurrentInitialParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = h2hPaymentRecurrentInitialParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string httpQuery = GeneralUtils.ToQueryString(h2hPaymentRecurrentInitialParameters);
            Console.WriteLine("Clear Query = " + httpQuery);
            string formattedRequest = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF8.GetBytes(httpQuery))) ?? throw new Exception("Formatted request data is not a string.");
            byte[] iv = SecurityUtils.GenerateIV();
            string base64Iv = Convert.ToBase64String(iv);
            byte[] encryptedByteArray = SecurityUtils.CbcEncryption(formattedRequest, Credentials.GetMerchantPass(), iv);
            string encryptedRequest = Convert.ToBase64String(encryptedByteArray) ?? throw new Exception("Encryption failed.");
            string signature = SecurityUtils.Sha256Hash(formattedRequest) ?? throw new Exception("Signature failed.");

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.H2H_STG : RequestsPaths.H2H_PROD;

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
                    if (!string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine($"Response received: {response}");
                        Notification notification = NotificationAdapter.ParseNotification(response);
                        responseListener.OnResponseReceived(response, notification, notification.GetTransactionResult());
                        return;
                    }
                    else
                    {
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

        public async Task SendH2hPaymentRecurrentSuccessive(H2HPaymentRecurrentSuccessiveParameters h2hPaymentRecurrentSuccessiveParameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = h2hPaymentRecurrentSuccessiveParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            h2hPaymentRecurrentSuccessiveParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = h2hPaymentRecurrentSuccessiveParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string httpQuery = GeneralUtils.ToQueryString(h2hPaymentRecurrentSuccessiveParameters);
            Console.WriteLine("Clear Query = " + httpQuery);
            string formattedRequest = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF8.GetBytes(httpQuery))) ?? throw new Exception("Formatted request data is not a string.");
            byte[] iv = SecurityUtils.GenerateIV();
            string base64Iv = Convert.ToBase64String(iv);
            byte[] encryptedByteArray = SecurityUtils.CbcEncryption(formattedRequest, Credentials.GetMerchantPass(), iv);
            string encryptedRequest = Convert.ToBase64String(encryptedByteArray) ?? throw new Exception("Encryption failed.");
            string signature = SecurityUtils.Sha256Hash(formattedRequest) ?? throw new Exception("Signature failed.");

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.H2H_STG : RequestsPaths.H2H_PROD;

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
                    if (!string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine($"Response received: {response}");
                        Notification notification = NotificationAdapter.ParseNotification(response);
                        responseListener.OnResponseReceived(response, notification, notification.GetTransactionResult());
                        return;
                    }
                    else
                    {
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

        public async Task SendH2hVoidRequest(H2HVoidParameters h2hVoidParameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = h2hVoidParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            h2hVoidParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = h2hVoidParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string httpQuery = GeneralUtils.ToQueryString(h2hVoidParameters);
            Console.WriteLine("Clear Query = " + httpQuery);
            string formattedRequest = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF8.GetBytes(httpQuery))) ?? throw new Exception("Formatted request data is not a string.");
            byte[] iv = SecurityUtils.GenerateIV();
            string base64Iv = Convert.ToBase64String(iv);
            byte[] encryptedByteArray = SecurityUtils.CbcEncryption(formattedRequest, Credentials.GetMerchantPass(), iv);
            string encryptedRequest = Convert.ToBase64String(encryptedByteArray) ?? throw new Exception("Encryption failed.");
            string signature = SecurityUtils.Sha256Hash(formattedRequest) ?? throw new Exception("Signature failed.");

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.VOID_STG : RequestsPaths.VOID_PROD;

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
                    if (!string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine($"Response received: {response}");
                        Notification notification = NotificationAdapter.ParseNotification(response);
                        responseListener.OnResponseReceived(response, notification, notification.GetTransactionResult());
                        return;
                    }
                    else
                    {
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
        
        public async Task SendH2hRefundRequest(H2HRefundParameters h2hRefundParameters, IResponseListener responseListener)
        {
            Tuple<bool, string> isMissingCred = h2hRefundParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            h2hRefundParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = h2hRefundParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string httpQuery = GeneralUtils.ToQueryString(h2hRefundParameters);
            Console.WriteLine("Clear Query = " + httpQuery);
            string formattedRequest = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF8.GetBytes(httpQuery))) ?? throw new Exception("Formatted request data is not a string.");
            byte[] iv = SecurityUtils.GenerateIV();
            string base64Iv = Convert.ToBase64String(iv);
            byte[] encryptedByteArray = SecurityUtils.CbcEncryption(formattedRequest, Credentials.GetMerchantPass(), iv);
            string encryptedRequest = Convert.ToBase64String(encryptedByteArray) ?? throw new Exception("Encryption failed.");
            string signature = SecurityUtils.Sha256Hash(formattedRequest) ?? throw new Exception("Signature failed.");

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.REBATE_STG : RequestsPaths.REBATE_PROD;

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
                    if (!string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine($"Response received: {response}");
                        Notification notification = NotificationAdapter.ParseNotification(response);
                        responseListener.OnResponseReceived(response, notification, notification.GetTransactionResult());
                        return;
                    }
                    else
                    {
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