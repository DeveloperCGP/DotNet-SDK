using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using DotNetPaymentSDK.src.Parameters.Notification;
using DotNetPaymentSDK.src.Parameters.Notification.OperationsModels;
using DotNetPaymentSDK.src.Parameters.NotificationJSON;
using DotNetPaymentSDK.src.Parameters.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetPaymentSDK.src.Adapters
{
    public class NotificationAdapter
    {
        public static Notification ParseNotification(string notificationString)
        {
            notificationString = notificationString.Trim();

            if (string.IsNullOrEmpty(notificationString))
            {
                throw new ArgumentException("The input notification string is null or empty.");
            }

            if (notificationString[0] != '[' && notificationString[0] != '{')
            {
                return ParseXMLNotification(notificationString);
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(notificationString);

            if (dict.ContainsKey("response"))
            {
                string responseDict = dict["response"].ToString().Trim();
                if (responseDict[0] != '[' && responseDict[0] != '{')
                {
                    return ParseXMLNotification(responseDict);
                }
                else
                {
                    notificationString = responseDict;
                }
            }

            return ParseJSONNotification(notificationString);
        }

        private static Notification ParseJSONNotification(string jsonString) 
        {
            NotificationJSON notificationJSON = JsonConvert.DeserializeObject<NotificationJSON>(jsonString);

            JObject jsonObject = JObject.Parse(jsonString);
            JToken optionalParamsToken = jsonObject["optionalTransactionParams"];

            Notification notification = new()
            {
                Message = notificationJSON.Message,
                Operations = new() { OperationList = mapResponse(notificationJSON.OperationsArray, jsonObject) },
                Status = notificationJSON.Status,
                WorkFlowResponse = notificationJSON.WorkFlowResponse,
                OptionalTransactionParams = ParseOptionalTransactionParams(optionalParamsToken)
            };

            return notification;
        }

        private static List<Operation> mapResponse(List<OperationJSON> operationJSONs, JObject jsonObject) 
        {
            return operationJSONs.Select((operation, i) => new Operation()
            {
                Amount = operation?.Amount,
                Currency = operation?.Currency,
                Details = operation?.Details,
                MerchantTransactionId = operation?.MerchantTransactionId,
                PaySolTransactionId = operation?.PaySolTransactionId,
                Service = operation?.Service,
                Status = operation?.Status,
                TransactionId = operation?.TransactionId,
                RespCode = new()
                {
                    Code = operation?.RespCode?.Code,
                    Message = operation?.RespCode?.Message,
                    UUID = operation?.RespCode?.UUID,
                },
                OperationType = operation?.OperationType,
                PaymentDetails = new()
                {
                    CardNumberToken = operation?.PaymentDetails?.CardNumberToken,
                    Account = operation?.PaymentDetails?.Account,
                    CardHolderName = operation?.PaymentDetails?.CardHolderName,
                    CardNumber = operation?.PaymentDetails?.CardNumber,
                    CardType = operation?.PaymentDetails?.CardType,
                    ExpDate = operation?.PaymentDetails?.ExpDate,
                    IssuerBank = operation?.PaymentDetails?.IssuerBank,
                    IssuerCountry = operation?.PaymentDetails?.IssuerCountry,
                    ExtraDetails = mapExtraDetails(jsonObject["operationsArray"][i]["paymentDetails"])
                },
                MPI = new()
                {
                    AcsTransID = operation?.MPI?.AcsTransID,
                    AuthMethod = operation?.MPI?.AuthMethod,
                    AuthTimestamp = operation?.MPI?.AuthTimestamp,
                    AuthenticationStatus = operation?.MPI?.AuthenticationStatus,
                    Cavv = operation?.MPI?.Cavv,
                    Eci = operation?.MPI?.Eci,
                    MessageVersion = operation?.MPI?.MessageVersion,
                    ThreeDSSessionData = operation?.MPI?.ThreeDSSessionData,
                    ThreeDSv2Token = operation?.MPI?.ThreeDSv2Token,
                },
                PaymentCode = operation?.PaymentCode,
                PaymentMessage = operation?.PaymentMessage,
                Message = operation?.Message,
                PaymentMethod = operation?.PaymentMethod,
                PaymentSolution = operation?.PaymentSolution,
                AuthCode = operation?.AuthCode,
                Rad = operation?.Rad,
                RadMessage = operation?.RadMessage,
                RedirectionResponse = operation?.RedirectionResponse,
                SubscriptionPlan = operation?.SubscriptionPlan,
                OptionalTransactionParams = ParseOptionalTransactionParams(jsonObject["operationsArray"][i]["optionalTransactionParams"])
            }).ToList();
        }

        private static OptionalTransactionParams ParseOptionalTransactionParams(JToken optionalParamsListToken) 
        {
            if (optionalParamsListToken == null || optionalParamsListToken.Type != JTokenType.Object) {
                return null;
            }

            var dictionary = optionalParamsListToken.ToObject<Dictionary<string, string>>() ?? [];

            OptionalTransactionParams optionalParams = new()
            {
                Entry = []
            };

            foreach (var kvp in dictionary)
            {
                optionalParams.Entry.Add(new() {
                    Key = kvp.Key,
                    Value = kvp.Value
                });
            }

            return optionalParams;
        }


        private static ExtraDetails mapExtraDetails(JToken jsonObject) 
        {
            if (jsonObject == null || jsonObject.Type != JTokenType.Object || jsonObject["extraDetails"] == null) 
            {
                return null;                    
            }

            JToken extraDetailsToken = jsonObject["extraDetails"];

            var dictionary = extraDetailsToken.ToObject<Dictionary<string, string>>() ?? [];

            ExtraDetails extraDetails = new()
            {
                Entry = []
            };

            foreach (var kvp in dictionary)
            {
                extraDetails.Entry.Add(new() {
                    Key = kvp.Key,
                    Value = kvp.Value
                });
            }
            return extraDetails;
        }


        private static Notification ParseXMLNotification(string xmlString)
        {
            var rootElementName = GetRootElementName(xmlString);

            if (rootElementName == "response" || rootElementName == "payfrex-response")
            {
                Notification response = null;
                XmlSerializer serializer = new XmlSerializer(typeof(Notification));

                if (rootElementName == "payfrex-response")
                {
                    serializer = new XmlSerializer(typeof(PayfrexResponse));
                }

                using (StringReader reader = new StringReader(xmlString))
                {
                    response = (Notification)serializer.Deserialize(reader);
                }
                
                return response;
            }
            else
            {
                throw new InvalidOperationException("Unknown XML format.");
            }
        }

        private static string GetRootElementName(string xml)
        {
            using (var stringReader = new StringReader(xml))
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                xmlReader.MoveToContent();
                return xmlReader.Name;
            }
        }

        public static string CleanInvalidXmlChars(string text)
        {
            string re = @"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]";
            return Regex.Replace(text, re, "");
        }

        public static string ConvertXmlToJson(string xmlString)
        {
            var xmlDoc = new XmlDocument();
            xmlString = CleanInvalidXmlChars(xmlString);
            xmlDoc.LoadXml(xmlString.Replace("\\", ""));
            return JsonConvert.SerializeXmlNode(xmlDoc);
        }
    }
}
