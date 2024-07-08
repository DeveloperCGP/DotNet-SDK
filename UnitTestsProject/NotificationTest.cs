using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using DotNetPaymentSDK.src.Adapters;
using DotNetPaymentSDK.src.Parameters.Nottification;

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

            Assert.AreEqual(3, notification.OperationsArray.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification.OperationsArray.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification.OperationsArray.First().Status, "TRA Status should be 'SUCCESS'");

            Assert.AreEqual("3DSv2", notification.OperationsArray[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification.OperationsArray[1].Status, "ThreeDsService should be 'SUCCESS3DS'");

            Assert.AreEqual("SUCCESS", notification.OperationsArray.Last().Status, "PaymentSolution should be 'SUCCESS'");
            Assert.AreEqual("SUCCESS", notification.Status, "Transaction should be 'SUCCESS'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4907271141151707()
        {
            string xmlContent = ReadXmlContent("4907271141151707.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification.OperationsArray.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification.OperationsArray.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification.OperationsArray.First().Status, "TRA Status should be 'SUCCESS'");

            Assert.AreEqual("3DSv2", notification.OperationsArray[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification.OperationsArray[1].Status, "ThreeDsService should be 'SUCCESS3DS'");

            Assert.AreEqual("ERROR", notification.OperationsArray.Last().Status, "PaymentSolution should be 'ERROR'");
            Assert.AreEqual("SUCCESS", notification.Status, "Transaction should be 'SUCCESS'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4907271141151715()
        {
            string xmlContent = ReadXmlContent("4907271141151715.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification.OperationsArray.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification.OperationsArray.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification.OperationsArray.First().Status, "TRA Status should be 'SUCCESS'");

            Assert.AreEqual("3DSv2", notification.OperationsArray[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification.OperationsArray[1].Status, "ThreeDsService should be 'SUCCESS3DS'");

            Assert.AreEqual("ERROR", notification.OperationsArray.Last().Status, "PaymentSolution should be 'ERROR'");
            Assert.AreEqual("SUCCESS", notification.Status, "Transaction should be 'SUCCESS'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4012000000010080()
        {
            string xmlContent = ReadXmlContent("4012000000010080.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(2, notification.OperationsArray.Count, "Operation Size should be '2'");
            Assert.AreEqual("TRA", notification.OperationsArray.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification.OperationsArray.First().Status, "TRA Status should be 'SUCCESS'");

            Assert.AreEqual("3DSv2", notification.OperationsArray[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("ERROR3DS", notification.OperationsArray[1].Status, "ThreeDsService should be 'ERROR3DS'");
            Assert.AreEqual("Not authenticated", notification.OperationsArray[1].PaymentMessage, "Payment message should be 'Not authenticated'");

            Assert.AreEqual(null, notification.OperationsArray.LastOrDefault()?.PaymentSolution, "PaymentSolution should be null");
            Assert.AreEqual("ERROR", notification.Status, "Transaction should be 'ERROR'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4012000000160083()
        {
            string xmlContent = ReadXmlContent("4012000000160083.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(2, notification.OperationsArray.Count, "Operation Size should be '2'");
            Assert.AreEqual("TRA", notification.OperationsArray.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification.OperationsArray.First().Status, "TRA Status should be 'SUCCESS'");

            Assert.AreEqual("3DSv2", notification.OperationsArray[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("ERROR3DS", notification.OperationsArray[1].Status, "ThreeDsService should be 'ERROR3DS'");
            Assert.AreEqual("Not authenticated because the issuer is rejecting authentication", notification.OperationsArray[1].PaymentMessage, "Payment message should be 'Not authenticated because the issuer is rejecting authentication'");

            Assert.AreEqual(null, notification.OperationsArray.LastOrDefault()?.PaymentSolution, "PaymentSolution should be null");
            Assert.AreEqual("ERROR", notification.Status, "Transaction should be 'ERROR'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4012000000000081()
        {
            string xmlContent = ReadXmlContent("4012000000000081.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(2, notification.OperationsArray.Count, "Operation Size should be '2'");
            Assert.AreEqual("TRA", notification.OperationsArray.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification.OperationsArray.First().Status, "TRA Status should be 'SUCCESS'");

            Assert.AreEqual("3DSv2", notification.OperationsArray[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("ERROR3DS", notification.OperationsArray[1].Status, "ThreeDsService should be 'ERROR3DS'");
            Assert.AreEqual("Not authenticated due to technical or other issue", notification.OperationsArray[1].PaymentMessage, "Payment message should be 'Not authenticated due to technical or other issue'");

            Assert.AreEqual(null, notification.OperationsArray.LastOrDefault()?.PaymentSolution, "PaymentSolution should be null");
            Assert.AreEqual("ERROR", notification.Status, "Transaction should be 'ERROR'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4012000000150084()
        {
            string xmlContent = ReadXmlContent("4012000000150084.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification.OperationsArray.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification.OperationsArray.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification.OperationsArray.First().Status, "TRA Status should be 'SUCCESS'");

            Assert.AreEqual("3DSv2", notification.OperationsArray[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification.OperationsArray[1].Status, "ThreeDsService should be 'SUCCESS3DS'");
            Assert.AreEqual("Challenge: Authenticated successfully", notification.OperationsArray[1].Message, "ThreeDs Message should be 'Challenge: Authenticated successfully'");

            Assert.AreEqual("SUCCESS", notification.OperationsArray.Last().Status, "PaymentSolution should be 'SUCCESS'");
            Assert.AreEqual("SUCCESS", notification.Status, "Transaction should be 'SUCCESS'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestCard4907271141151723()
        {
            string xmlContent = ReadXmlContent("4907271141151723.xml");
            Notification notification = NotificationAdapter.ParseNotification(xmlContent);

            Assert.AreEqual(3, notification.OperationsArray.Count, "Operation Size should be '3'");
            Assert.AreEqual("TRA", notification.OperationsArray.First().Service, "Service Name should be 'TRA'");
            Assert.AreEqual("SUCCESS", notification.OperationsArray.First().Status, "TRA Status should be 'SUCCESS'");

            Assert.AreEqual("3DSv2", notification.OperationsArray[1].Service, "Service Name should be '3DSv2'");
            Assert.AreEqual("SUCCESS3DS", notification.OperationsArray[1].Status, "ThreeDsService should be 'SUCCESS3DS'");

            Assert.AreEqual("ERROR", notification.OperationsArray.Last().Status, "PaymentSolution should be 'ERROR'");
            Assert.AreEqual("Denied 'Settle' operation with code: 180 message: Tarjeta ajena al servicio o no compatible.", notification.OperationsArray.Last().Message, "ThreeDs Message should be 'Denied \'Settle\' operation with code: 180 message: Tarjeta ajena al servicio o no compatible.'");
            Assert.AreEqual("SUCCESS", notification.Status, "Transaction should be 'SUCCESS'");
        }

        [TestCategory("Notification")]
        [TestMethod]
        public void TestXML_Inside_JSON()
        {
            string jsonContent = ReadXmlContent("xml_inside_json.json");
            Notification notification = NotificationAdapter.ParseNotification(jsonContent);
            System.Console.WriteLine($"Notification Object: {notification}");
            Assert.AreEqual("898a0370-249b-43db-b604-e4ce5e7f120f", notification.GetNemuruCartHash(), "NemuruCartHash should be '898a0370-249b-43db-b604-e4ce5e7f120f'");
            Assert.AreEqual("LHb76UKXmwW78LUI9VCWnwP9NKv5Qljt", notification.GetNemuruAuthToken(), "NemuruAuthToken should be 'LHb76UKXmwW78LUI9VCWnwP9NKv5Qljt'");
        }
    }
}
