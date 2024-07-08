using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.Interfaces;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.JS;
using DotNetPaymentSDK.src.Parameters.Nottification;
using DotNetPaymentSDK.src.Requests.Utils;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.Services
{
    public class JSService: IJSService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; } = new NetworkAdapter();

        public ISecurityUtils SecurityUtils { get; set; } = new SecurityUtilsImpl();

        public async Task SendJSAuthorizationRequest(JSAuthorizationRequestParameters jsAuthParameters, IJSPaymentListener jsPaymentListener)
        {
            Tuple<bool, string> isMissingCred = jsAuthParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            jsAuthParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = jsAuthParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string authBodyString = JsonConvert.SerializeObject(jsAuthParameters);
            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.JS_AUTH_STG : RequestsPaths.JS_AUTH_PROD;

            try
            {
                var requestHttpClient = await NetworkAdapter.RequestHttpJSClient(url, authBodyString, new Dictionary<string, string>() { });
                var response = requestHttpClient["response"];
                _ = int.TryParse(requestHttpClient["status_code"], out int statusCode);

                if (StatusCodes.IsSuccess(statusCode))
                {
                    if (!string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine($"Response received: {response}");
                        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                        jsPaymentListener.OnAuthorizationResponseReceived(response, new() { AuthToken = dict["authToken"] });
                    }
                    else
                    {
                        jsPaymentListener.OnError(ErrorsEnum.INVALID_RESPONSE_RECEIVED, "Invalid Response Received");
                    }
                }
                else
                {
                    string errorMessage = "Status code is " + statusCode;
                    if (!string.IsNullOrEmpty(response))
                    {
                        errorMessage = response.ToString();
                    }
                    jsPaymentListener.OnError(StatusCodes.IsClientError(statusCode) ? ErrorsEnum.CLIENT_ERROR : ErrorsEnum.SERVER_ERROR, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                jsPaymentListener.OnError(ErrorsEnum.NETWORK_ERROR, ex.ToString());
            }
        }

        public async Task SendJSChargeRequest(JSChargeParameters jsChargeParameters, IResponseListener responseListener)
        {

            Tuple<bool, string> isMissingCred = jsChargeParameters.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            jsChargeParameters.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = jsChargeParameters.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.JS_CHARGE_STG : RequestsPaths.JS_CHARGE_PROD;
            var headers = new Dictionary<string, string>
                {
                    { "prepayToken", jsChargeParameters.GetPrepayToken() },
                    { "apiVersion", Credentials.GetApiVersion().ToString() }
                };
            string bodyString = JsonConvert.SerializeObject(jsChargeParameters);

            var requestHttpClient = await NetworkAdapter.RequestHttpChargeClient(url, bodyString, headers);

            try
            {
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

        public async Task SendJSPaymentRecurrentInitial(JSPaymentRecurrentInitial jsPaymentRecurrentInitial, IResponseListener responseListener)
        {

            Tuple<bool, string> isMissingCred = jsPaymentRecurrentInitial.CheckCredentials(Credentials);
            if (isMissingCred.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingCred.Item2, true));
            }

            jsPaymentRecurrentInitial.SetCredentials(Credentials);
            Tuple<bool, string> isMissingField = jsPaymentRecurrentInitial.IsMissingField();
            if (isMissingField.Item1)
            {
                throw new MissingFieldsException(MissingFieldsException.CreateMessage(isMissingField.Item2, false));
            }

            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.JS_CHARGE_STG : RequestsPaths.JS_CHARGE_PROD;
            var headers = new Dictionary<string, string>
                {
                    { "prepayToken", jsPaymentRecurrentInitial.GetPrepayToken() },
                    { "apiVersion", Credentials.GetApiVersion().ToString() }
                };
            string bodyString = JsonConvert.SerializeObject(jsPaymentRecurrentInitial);

            var requestHttpClient = await NetworkAdapter.RequestHttpChargeClient(url, bodyString, headers);

            try
            {
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