using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Parameters.Notification;
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Parameters.Notification;

namespace DotNetPaymentSDK.Tests
{
    [TestClass]
    public class XMLNotificationTests
    {
        private string ReadXmlContent(string fileName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.FullName;
            string xmlFilePath = Path.Combine(projectDirectory, "Notifications", fileName);

            if (!File.Exists(xmlFilePath))
            {
                throw new FileNotFoundException($"The file {xmlFilePath} was not found.");
            }

            return File.ReadAllText(xmlFilePath);
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4907270002222227()
        {
            string xmlContent = ReadXmlContent("4907270002222227.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification?.Operations?.OperationList?.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8202", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Force to challange", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("7953639", notification?.Operations?.OperationList?.First().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'SUCCESS3DS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("7953639", notification?.Operations?.OperationList?[1].TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?[1].MerchantTransactionId);
            Assert.AreEqual("nsY1", notification?.Operations?.OperationList?[1].PaymentCode);
            Assert.AreEqual("9bc792e7-ba4b-4092-bf01-64bc26a573c3", notification?.Operations?.OperationList?[1].MPI.AcsTransID);
            Assert.AreEqual("AJkBB4OBmVFmgYFYFIGZAAAAAAA=", notification?.Operations?.OperationList?[1].MPI.Cavv);

            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.Last().Status, "PaymentSolution should be 'SUCCESS'");
            Assert.AreEqual("SUCCESS", notification.Status, "Transaction should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.Last().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.Last().Currency);
            Assert.AreEqual("7953639", notification?.Operations?.OperationList?.Last().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.Last().MerchantTransactionId);
            Assert.AreEqual("Pablo", notification?.Operations?.OperationList?.Last().PaymentDetails.CardHolderName);
            Assert.AreEqual("490727****2227", notification?.Operations?.OperationList?.Last().PaymentDetails.CardNumber);
            // Assert.AreEqual("false", notification?.Operations?.OperationList?.Last().PaymentDetails.ExtraDetails["rememberMe"]);
            Assert.AreEqual("000", notification?.Operations?.OperationList?.Last().PaymentCode);
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4907271141151707()
        {
            string xmlContent = ReadXmlContent("4907271141151707.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification?.Operations?.OperationList?.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8202", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Force to challange", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("7953645", notification?.Operations?.OperationList?.First().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'SUCCESS3DS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("7953645", notification?.Operations?.OperationList?[1].TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("nsY1", notification?.Operations?.OperationList?[1].PaymentCode);
            Assert.AreEqual("5579c69d-56dd-495f-864b-5665b0580010", notification?.Operations?.OperationList?[1].MPI.AcsTransID);
            Assert.AreEqual("AJkBB4OBmVFmgYFYFIGZAAAAAAA=", notification?.Operations?.OperationList?[1].MPI.Cavv);

            Assert.AreEqual("ERROR", notification?.Operations?.OperationList?.Last().Status, "PaymentSolution should be 'ERROR'");
            Assert.AreEqual("SUCCESS", notification.Status, "Transaction should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.Last().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.Last().Currency);
            Assert.AreEqual("7953645", notification?.Operations?.OperationList?.Last().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("Pablo", notification?.Operations?.OperationList?.Last().PaymentDetails.CardHolderName);
            Assert.AreEqual("190", notification?.Operations?.OperationList?.Last().PaymentCode);
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4907271141151715()
        {
            string xmlContent = ReadXmlContent("4907271141151715.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification?.Operations?.OperationList?.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8202", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Force to challange", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("7953663", notification?.Operations?.OperationList?.First().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'SUCCESS3DS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("7953663", notification?.Operations?.OperationList?[1].TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("nsY1", notification?.Operations?.OperationList?[1].PaymentCode);
            Assert.AreEqual("534354a9-4add-46b7-a94e-296f7fa4988a", notification?.Operations?.OperationList?[1].MPI.AcsTransID);
            Assert.AreEqual("AJkBB4OBmVFmgYFYFIGZAAAAAAA=", notification?.Operations?.OperationList?[1].MPI.Cavv);

            Assert.AreEqual("ERROR", notification?.Operations?.OperationList?.Last().Status, "PaymentSolution should be 'ERROR'");
            Assert.AreEqual("SUCCESS", notification.Status, "Transaction should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.Last().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.Last().Currency);
            Assert.AreEqual("7953663", notification?.Operations?.OperationList?.Last().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("Pablo", notification?.Operations?.OperationList?.Last().PaymentDetails.CardHolderName);
            Assert.AreEqual("195", notification?.Operations?.OperationList?.Last().PaymentCode);
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4012000000010080()
        {
            string xmlContent = ReadXmlContent("4012000000010080.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(2, notification?.Operations?.OperationList?.Count, "Operation Size should be '2'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8202", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Force to challange", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("7953615", notification?.Operations?.OperationList?.First().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("ERROR3DS", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'ERROR3DS'");
            Assert.AreEqual("Not authenticated", notification?.Operations?.OperationList?[1].PaymentMessage, "Payment message should be 'Not authenticated'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("7953615", notification?.Operations?.OperationList?[1].TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("nsN3", notification?.Operations?.OperationList?[1].PaymentCode);
            Assert.AreEqual("68a8f861-efb6-4240-ba7e-e37de03595cd", notification?.Operations?.OperationList?[1].MPI.AcsTransID);

            Assert.IsNull(notification?.Operations?.OperationList?.LastOrDefault()?.PaymentSolution, "PaymentSolution should be null");
            Assert.AreEqual("ERROR", notification.Status, "Transaction should be 'ERROR'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4012000000160083()
        {
            string xmlContent = ReadXmlContent("4012000000160083.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(2, notification?.Operations?.OperationList?.Count, "Operation Size should be '2'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8202", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Force to challange", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("7953624", notification?.Operations?.OperationList?.First().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("ERROR3DS", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'ERROR3DS'");
            Assert.AreEqual("Not authenticated because the issuer is rejecting authentication", notification?.Operations?.OperationList?[1].PaymentMessage, "Payment message should be 'Not authenticated because the issuer is rejecting authentication'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("7953624", notification?.Operations?.OperationList?[1].TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("nsR6", notification?.Operations?.OperationList?[1].PaymentCode);
            Assert.AreEqual("1f2244e5-3f03-49d0-aeac-0e71aa8fd2ff", notification?.Operations?.OperationList?[1]?.MPI?.AcsTransID);

            Assert.IsNull(notification?.Operations?.OperationList?.LastOrDefault()?.PaymentSolution, "PaymentSolution should be null");
            Assert.AreEqual("ERROR", notification.Status, "Transaction should be 'ERROR'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4012000000000081()
        {
            string xmlContent = ReadXmlContent("4012000000000081.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(2, notification?.Operations?.OperationList?.Count, "Operation Size should be '2'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8202", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Force to challange", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("7953612", notification?.Operations?.OperationList?.First().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("ERROR3DS", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'ERROR3DS'");
            Assert.AreEqual("Not authenticated due to technical or other issue", notification?.Operations?.OperationList?[1].PaymentMessage, "Payment message should be 'Not authenticated due to technical or other issue'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("7953612", notification?.Operations?.OperationList?[1].TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("nsU5", notification?.Operations?.OperationList?[1].PaymentCode);
            Assert.AreEqual("e7f5c306-beca-4806-bd66-de15a8d7e3aa", notification?.Operations?.OperationList?[1]?.MPI?.AcsTransID);

            Assert.IsNull(notification?.Operations?.OperationList?.LastOrDefault()?.PaymentSolution, "PaymentSolution should be null");
            Assert.AreEqual("ERROR", notification.Status, "Transaction should be 'ERROR'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4012000000150084()
        {
            string xmlContent = ReadXmlContent("4012000000150084.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification?.Operations?.OperationList?.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8202", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Force to challange", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("7953618", notification?.Operations?.OperationList?.First().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'SUCCESS3DS'");
            Assert.AreEqual("Challenge: Authenticated successfully", notification?.Operations?.OperationList?[1].Message, "ThreeDs Message should be 'Challenge: Authenticated successfully'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("7953618", notification?.Operations?.OperationList?[1].TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("nsY1", notification?.Operations?.OperationList?[1].PaymentCode);
            Assert.AreEqual("74be1aac-61a7-4059-9f6a-91d68d8c6427", notification?.Operations?.OperationList?[1]?.MPI?.AcsTransID);
            Assert.AreEqual("AJkBAlQ0Y1czBidENjRjAAAAAAA=", notification?.Operations?.OperationList?[1]?.MPI?.Cavv);

            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.Last().Status, "PaymentSolution should be 'SUCCESS'");
            Assert.AreEqual("SUCCESS", notification?.Status, "Transaction should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.Last().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.Last().Currency);
            Assert.AreEqual("7953618", notification?.Operations?.OperationList?.Last().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("Pablo", notification?.Operations?.OperationList?.Last()?.PaymentDetails?.CardHolderName);
            Assert.AreEqual("401200****0084", notification?.Operations?.OperationList?.Last()?.PaymentDetails?.CardNumber);
            // Assert.AreEqual("false", notification?.Operations?.OperationList?.Last().PaymentDetails.ExtraDetails["rememberMe"]);
            Assert.AreEqual("000", notification?.Operations?.OperationList?.Last().PaymentCode);
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4907271141151723()
        {
            string xmlContent = ReadXmlContent("4907271141151723.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification?.Operations?.OperationList?.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8202", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Force to challange", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("7953666", notification?.Operations?.OperationList?.First().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'SUCCESS3DS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("7953666", notification?.Operations?.OperationList?[1].TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("nsY1", notification?.Operations?.OperationList?[1].PaymentCode);
            Assert.AreEqual("d972be9e-8eb1-48ed-8c3c-c7a42fb8c440", notification?.Operations?.OperationList?[1]?.MPI?.AcsTransID);
            Assert.AreEqual("AJkBB4OBmVFmgYFYFIGZAAAAAAA=", notification?.Operations?.OperationList?[1]?.MPI?.Cavv);

            Assert.AreEqual("ERROR", notification?.Operations?.OperationList?.Last().Status, "PaymentSolution should be 'ERROR'");
            Assert.AreEqual("Denied 'Settle' operation with code: 180 message: Tarjeta ajena al servicio o no compatible.", notification?.Operations?.OperationList?.Last().Message, "ThreeDs Message should be 'Denied \'Settle\' operation with code: 180 message: Tarjeta ajena al servicio o no compatible.'");
            Assert.AreEqual("SUCCESS", notification?.Status, "Transaction should be 'SUCCESS'");
            Assert.AreEqual("12.50", notification?.Operations?.OperationList?.Last().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.Last().Currency);
            Assert.AreEqual("7953666", notification?.Operations?.OperationList?.Last().TransactionId);
            Assert.AreEqual("756974", notification?.Operations?.OperationList?.First().MerchantTransactionId);
            Assert.AreEqual("Pablo", notification?.Operations?.OperationList?.Last().PaymentDetails.CardHolderName);
            Assert.AreEqual("180", notification?.Operations?.OperationList?.Last().PaymentCode);
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestXML_Inside_JSON()
        {
            string jsonContent = ReadXmlContent("xml_inside_json.json");
            Notification notification = NotificationAdapter.ParseNotification(jsonContent);

            Assert.AreEqual("REDIRECTED", notification?.Operations?.OperationList?.Last().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("70", notification?.Operations?.OperationList?.Last().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.Last().Currency);
            Assert.AreEqual("0000", notification?.Operations?.OperationList?.Last().RespCode.Code);
            Assert.AreEqual("Successful", notification?.Operations?.OperationList?.Last().RespCode.Message);
            // Assert.AreEqual("7934766", notification?.Operations?.OperationList?.Last().TransactionId);
            Assert.AreEqual("311181", notification?.Operations?.OperationList?.Last().MerchantTransactionId);
            Assert.AreEqual("REDIRECTED", notification?.Operations?.OperationList?.Last()?.PaymentDetails?.ExtraDetails?.GetStatus());

            Assert.AreEqual("898a0370-249b-43db-b604-e4ce5e7f120f", notification?.Operations?.OperationList?.Last()?.PaymentDetails?.ExtraDetails?.GetNemuruCartHash(), "NemuruCartHash should be '898a0370-249b-43db-b604-e4ce5e7f120f'");
            Assert.AreEqual("LHb76UKXmwW78LUI9VCWnwP9NKv5Qljt", notification?.Operations?.OperationList?.Last()?.PaymentDetails?.ExtraDetails?.GetNemuruAuthToken(), "NemuruAuthToken should be 'LHb76UKXmwW78LUI9VCWnwP9NKv5Qljt'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestNotificationWithOptionalParametersJSON()
        {
            string xmlContent = ReadXmlContent("notification_with_optionalParameters.json");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(2, notification?.Operations?.OperationList?.Count, "Operation Size should be '2'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("30", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8203", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Frictionless requires", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("23506844", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("REDIRECTED", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'SUCCESS3DS'");
            Assert.AreEqual("30", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("threeDSMethodData", notification?.Operations?.OperationList?[1].PaymentDetails?.ExtraDetails?.Entry?[0].Key);
            Assert.AreEqual("eyJ0aHJlZURTU2VydmVyVHJhbnNJRCI6IjRhNzUwYmNlLWEwM2UtNGI1Ni1iMTRmLWE1YTBlNjc5YTRiOSIsICJ0aHJlZURTTWV0aG9kTm90aWZpY2F0aW9uVVJMIjogImh0dHBzOi8vY2hlY2tvdXQuc3RnLWV1LXdlc3QxLmVwZ2ludC5jb20vRVBHQ2hlY2tvdXQvY2FsbGJhY2svZ2F0aGVyRGV2aWNlTm90aWZpY2F0aW9uL3BheXNvbC8zZHN2Mi8xMTA4MTA0In0=", notification?.Operations?.OperationList?[1].PaymentDetails?.ExtraDetails?.Entry?[0].Value);
            Assert.AreEqual("threeDSv2Token", notification?.Operations?.OperationList?[1].PaymentDetails?.ExtraDetails?.Entry?[1].Key);
            Assert.AreEqual("4a750bce-a03e-4b56-b14f-a5a0e679a4b9", notification?.Operations?.OperationList?[1].PaymentDetails?.ExtraDetails?.Entry?[1].Value);

            Assert.AreEqual("ClaveN", notification?.OptionalTransactionParams?.Entry?[0].Key);
            Assert.AreEqual("ValorN", notification?.OptionalTransactionParams?.Entry?[0].Value);
            Assert.AreEqual("Clave1", notification?.OptionalTransactionParams?.Entry?[1].Key);
            Assert.AreEqual("Valor1", notification?.OptionalTransactionParams?.Entry?[1].Value);
            Assert.AreEqual("Clave2", notification?.OptionalTransactionParams?.Entry?[2].Key);
            Assert.AreEqual("Valor2", notification?.OptionalTransactionParams?.Entry?[2].Value);
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestNotificationWithOptionalParametersXML()
        {
            string xmlContent = ReadXmlContent("notification_with_optionalParameters.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification?.Operations?.OperationList?.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification?.Operations?.OperationList?.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.First().Status, "TRA Status should be 'SUCCESS'");
            Assert.AreEqual("13", notification?.Operations?.OperationList?.First().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.First().Currency);
            Assert.AreEqual("8203", notification?.Operations?.OperationList?.First().RespCode.Code);
            Assert.AreEqual("Frictionless requires", notification?.Operations?.OperationList?.First().RespCode.Message);
            Assert.AreEqual("8232609", notification?.Operations?.OperationList?.First().TransactionId);
            Assert.AreEqual("1496918", notification?.Operations?.OperationList?.First().MerchantTransactionId);

            Assert.AreEqual("3DSv2", notification?.Operations?.OperationList?[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification?.Operations?.OperationList?[1].Status, "ThreeDsService should be 'SUCCESS3DS'");
            Assert.AreEqual("13", notification?.Operations?.OperationList?[1].Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?[1].Currency);
            Assert.AreEqual("8232609", notification?.Operations?.OperationList?[1].TransactionId);
            Assert.AreEqual("1496918", notification?.Operations?.OperationList?[1].MerchantTransactionId);
            Assert.AreEqual("nsY1", notification?.Operations?.OperationList?[1].PaymentCode);
            Assert.AreEqual("163c965a-9772-4bb1-a2f4-e96e184a2661", notification?.Operations?.OperationList?[1]?.MPI?.AcsTransID);
            Assert.AreEqual("AJkBB4OBmVFmgYFYFIGZAAAAAAA=", notification?.Operations?.OperationList?[1]?.MPI?.Cavv);

            Assert.AreEqual("SUCCESS", notification?.Operations?.OperationList?.Last().Status, "PaymentSolution should be 'ERROR'");
            Assert.AreEqual("Success 'Settle' operation", notification?.Operations?.OperationList?.Last().Message, "ThreeDs Message should be 'Success \"Settle\" operation'");
            Assert.AreEqual("SUCCESS", notification?.Status, "Transaction should be 'SUCCESS'");
            Assert.AreEqual("13.00", notification?.Operations?.OperationList?.Last().Amount);
            Assert.AreEqual(Currency.EUR, notification?.Operations?.OperationList?.Last().Currency);
            Assert.AreEqual("8232609", notification?.Operations?.OperationList?.Last().TransactionId);
            Assert.AreEqual("test", notification?.Operations?.OperationList?.Last()?.PaymentDetails?.CardHolderName);
            Assert.AreEqual("000", notification?.Operations?.OperationList?.Last().PaymentCode);

            Assert.AreEqual("sdk", notification?.Operations?.OperationList?.Last().OptionalTransactionParams?.Entry?[0].Key);
            Assert.AreEqual("php", notification?.Operations?.OperationList?.Last().OptionalTransactionParams?.Entry?[0].Value);
            Assert.AreEqual("type", notification?.Operations?.OperationList?.Last().OptionalTransactionParams?.Entry?[1].Key);
            Assert.AreEqual("JsCharge", notification?.Operations?.OperationList?.Last().OptionalTransactionParams?.Entry?[1].Value);
            Assert.AreEqual("version", notification?.Operations?.OperationList?.Last().OptionalTransactionParams?.Entry?[2].Key);
            Assert.AreEqual("1.00", notification?.Operations?.OperationList?.Last().OptionalTransactionParams?.Entry?[2].Value);

            Assert.AreEqual("sdk", notification?.OptionalTransactionParams?.Entry?[0].Key);
            Assert.AreEqual("php", notification?.OptionalTransactionParams?.Entry?[0].Value);
            Assert.AreEqual("type", notification?.OptionalTransactionParams?.Entry?[1].Key);
            Assert.AreEqual("JsCharge", notification?.OptionalTransactionParams?.Entry?[1].Value);
            Assert.AreEqual("version", notification?.OptionalTransactionParams?.Entry?[2].Key);
            Assert.AreEqual("1.00", notification?.OptionalTransactionParams?.Entry?[2].Value);
        }

    }
}
