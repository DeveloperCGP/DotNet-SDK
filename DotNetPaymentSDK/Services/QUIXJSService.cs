using System.Net;
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.Callbacks;
using DotNetPaymentSDK.Interfaces;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.src.Parameters;
using DotNetPaymentSDK.src.Parameters.Nottification;
using DotNetPaymentSDK.src.Parameters.Quix_JS;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Accommodation;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Flight;
using DotNetPaymentSDK.src.Parameters.Quix_Models.Quix_Service;
using DotNetPaymentSDK.src.Requests.Utils;
using DotNetPaymentSDK.Utilities;
using Newtonsoft.Json;

namespace DotNetPaymentSDK.Services
{
    public class QUIXJSService : IQUIXJSService
    {
        public Credentials Credentials { get; set; }
        public INetworkAdapter NetworkAdapter { get; set; } = new NetworkAdapter();
        public ISecurityUtils SecurityUtils { get; set; } = new SecurityUtilsImpl();

        public async Task SendJSQuixServiceRequest(JSQuixService parameters, IResponseListener responseListener)
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

            List<QuixItemCartItemService> items = parameters.PaySolExtendedData.Cart.Items;
            foreach (QuixItemCartItemService item in items)
            {
                string endDate = item.Article.EndDate;
                if (endDate.Contains(':'))
                {
                    item.Article.SetEndDate(WebUtility.UrlEncode(endDate));
                }
                if (item.Article.StartDate != null)
                {
                    string startDate = item.Article.StartDate;
                    if (startDate.Contains(':'))
                    {
                        item.Article.SetStartDate(WebUtility.UrlEncode(endDate));
                    }
                }
            }
            string authBodyString = JsonConvert.SerializeObject(parameters);
            Dictionary<string, object> tempDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(authBodyString);
            tempDict["paysolExtendedData"] = JsonConvert.SerializeObject(tempDict["paysolExtendedData"]);
            tempDict.Remove("prepayToken");
            if (tempDict.ContainsKey("merchantParams"))
            {
                if (tempDict["merchantParams"] != null)
                {
                    tempDict["merchantParams"] = GeneralUtils.MerchantParamsQuery(parameters.GetMerchantParameters());
                }
                else
                {
                    tempDict.Remove("merchantParams");
                }
            }
            authBodyString = JsonConvert.SerializeObject(tempDict);

            var headers = new Dictionary<string, string>
                {
                    { "prepayToken", parameters.PrepayToken },
                    { "apiVersion", Credentials.GetApiVersion().ToString() }
                };
            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.JS_CHARGE_STG : RequestsPaths.JS_CHARGE_PROD;


            try
            {
                var requestHttpClient = await NetworkAdapter.RequestHttpJSClient(url, authBodyString, headers);
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

        public async Task SendJSQuixFlightRequest(JSQuixFlight parameters, IResponseListener responseListener)
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

            List<QuixItemCartItemFlight> items = parameters.PaySolExtendedData.Cart.Items;
            foreach (QuixItemCartItemFlight item in items)
            {
                string departureDate = item.Article.DepartureDate;
                if (departureDate.Contains(':'))
                {
                    item.Article.SetDepartureDate(WebUtility.UrlEncode(departureDate));
                }
            }
            string authBodyString = JsonConvert.SerializeObject(parameters);
            Dictionary<string, object> tempDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(authBodyString);
            tempDict["paysolExtendedData"] = JsonConvert.SerializeObject(tempDict["paysolExtendedData"]);
            tempDict.Remove("prepayToken");
            if (tempDict.ContainsKey("merchantParams"))
            {
                if (tempDict["merchantParams"] != null)
                {
                    tempDict["merchantParams"] = GeneralUtils.MerchantParamsQuery(parameters.GetMerchantParameters());
                }
                else
                {
                    tempDict.Remove("merchantParams");
                }
            }
            authBodyString = JsonConvert.SerializeObject(tempDict);

            var headers = new Dictionary<string, string>
                {
                    { "prepayToken", parameters.PrepayToken },
                    { "apiVersion", Credentials.GetApiVersion().ToString() }
                };
            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.JS_CHARGE_STG : RequestsPaths.JS_CHARGE_PROD;


            try
            {
                var requestHttpClient = await NetworkAdapter.RequestHttpJSClient(url, authBodyString, headers);
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

        public async Task SendJSQuixProductRequest(JSQuixProduct parameters, IResponseListener responseListener)
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

            string authBodyString = JsonConvert.SerializeObject(parameters);
            Dictionary<string, object> tempDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(authBodyString);
            tempDict["paysolExtendedData"] = JsonConvert.SerializeObject(tempDict["paysolExtendedData"]);
            tempDict.Remove("prepayToken");
            if (tempDict.ContainsKey("merchantParams"))
            {
                if (tempDict["merchantParams"] != null)
                {
                    tempDict["merchantParams"] = GeneralUtils.MerchantParamsQuery(parameters.GetMerchantParameters());
                }
                else
                {
                    tempDict.Remove("merchantParams");
                }
            }
            authBodyString = JsonConvert.SerializeObject(tempDict);

            var headers = new Dictionary<string, string>
                {
                    { "prepayToken", parameters.PrepayToken },
                    { "apiVersion", Credentials.GetApiVersion().ToString() }
                };
            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.JS_CHARGE_STG : RequestsPaths.JS_CHARGE_PROD;

            try
            {
                var requestHttpClient = await NetworkAdapter.RequestHttpJSClient(url, authBodyString, headers);
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

        public async Task SendJSQuixAccommodationRequest(JSQuixAccommodation parameters, IResponseListener responseListener)
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

            List<QuixItemCartItemAccommodation> items = parameters.PaySolExtendedData.Cart.Items;
            foreach (QuixItemCartItemAccommodation item in items)
            {
                string checkinDate = item.Article.CheckInDate;
                string checkoutDate = item.Article.CheckOutDate;
                if (checkinDate.Contains(':'))
                {
                    item.Article.SetCheckinDate(WebUtility.UrlEncode(checkinDate));
                }
                if (checkoutDate.Contains(':'))
                {
                    item.Article.SetCheckoutDate(WebUtility.UrlEncode(checkoutDate));
                }
            }

            string authBodyString = JsonConvert.SerializeObject(parameters);
            Dictionary<string, object> tempDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(authBodyString);
            tempDict["paysolExtendedData"] = JsonConvert.SerializeObject(tempDict["paysolExtendedData"]);
            tempDict.Remove("prepayToken");
            if (tempDict.ContainsKey("merchantParams"))
            {
                if (tempDict["merchantParams"] != null)
                {
                    tempDict["merchantParams"] = GeneralUtils.MerchantParamsQuery(parameters.GetMerchantParameters());
                }
                else
                {
                    tempDict.Remove("merchantParams");
                }
            }
            authBodyString = JsonConvert.SerializeObject(tempDict);

            var headers = new Dictionary<string, string>
                {
                    { "prepayToken", parameters.PrepayToken },
                    { "apiVersion", Credentials.GetApiVersion().ToString() }
                };
            string url = (Credentials.GetEnvironment()?.ToString()).Equals(EnvironmentEnum.STAGING.ToString(), StringComparison.CurrentCultureIgnoreCase) ? RequestsPaths.JS_CHARGE_STG : RequestsPaths.JS_CHARGE_PROD;

            try
            {
                var requestHttpClient = await NetworkAdapter.RequestHttpJSClient(url, authBodyString, headers);
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
