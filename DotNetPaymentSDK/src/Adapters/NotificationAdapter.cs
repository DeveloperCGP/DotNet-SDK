using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using DotNetPaymentSDK.src.Parameters.Nottification;
using DotNetPaymentSDK.src.Parameters.NotificationXML;
using Newtonsoft.Json;

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

            return JsonConvert.DeserializeObject<Notification>(notificationString);
        }

        private static Notification ParseXMLNotification(string xmlString)
        {
            var rootElementName = GetRootElementName(xmlString);

            if (rootElementName == "response" || rootElementName == "payfrex-response")
            {
                Response response = null;
                XmlSerializer serializer = new XmlSerializer(typeof(Response));

                if (rootElementName == "payfrex-response")
                {
                    serializer = new XmlSerializer(typeof(PayfrexResponse));
                }

                using (StringReader reader = new StringReader(xmlString))
                {
                    response = (Response)serializer.Deserialize(reader);
                }

                Notification notification = new()
                {
                    Message = response?.Message,
                    Status = response?.Status,
                    OperationsArray = mapResponse(response),
                    WorkFlowResponse = new()
                    {
                        Id = response?.WorkFlowResponse?.Id,
                        Name = response?.WorkFlowResponse?.Name,
                        Version = response?.WorkFlowResponse?.Version,
                    },
                };
                return notification;
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

        public static List<DotNetPaymentSDK.src.Parameters.Notification.Operation.Operation> mapResponse(Response response)
        {
            return response.Operations.Operation.Select(operation => new DotNetPaymentSDK.src.Parameters.Notification.Operation.Operation()
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
                    ExtraDetails = new()
                    {
                        NemuruAuthToken = operation?.PaymentDetails?.ExtraDetails?.GetNemuruAuthToken(),
                        NemuruTxnId = operation?.PaymentDetails?.ExtraDetails?.GetNemuruTxnId(),
                        NemuruCartHash = operation?.PaymentDetails?.ExtraDetails?.GetNemuruCartHash(),
                        NemuruDisableFormEdition = operation?.PaymentDetails?.ExtraDetails?.GetNemuruDisableFormEdition(),
                        Status = operation?.PaymentDetails?.ExtraDetails?.GetStatus(),
                        DisableFormEdition = operation?.PaymentDetails?.ExtraDetails?.GetDisableFormEdition()
                    }
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
            }).ToList();
        }
    }
}
