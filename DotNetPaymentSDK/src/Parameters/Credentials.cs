using System.Text.RegularExpressions;
using DotNetPayment.Core.Domain.Enums;
using DotNetPaymentSDK.src.Exceptions;
using DotNetPaymentSDK.Utilities;

namespace DotNetPaymentSDK.src.Parameters
{
    public class Credentials
    {
        private string MerchantId;
        private string MerchantKey;
        private string MerchantPass;
        private EnvironmentEnum? Environment;
        private string ProductId;
        private int ApiVersion = -1;

        public string GetMerchantId()
        {
            return MerchantId;
        }

        public void SetMerchantId(string merchantId)
        {
            if (merchantId == null || !GeneralUtils.IsNumbersOnly(merchantId) || merchantId.Length < 4 || merchantId.Length > 7)
            {
                throw new InvalidFieldException("merchantId: Should be numbers in size 4 <= merchantId <= 7");
            }
            MerchantId = merchantId;
        }

        public string GetMerchantKey()
        {
            return MerchantKey;
        }

        public void SetMerchantKey(String merchantKey)
        {
            if (!new Regex("^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$").IsMatch(merchantKey))
            {
                throw new InvalidFieldException("merchantKey: Must be in UUID format");
            }
            MerchantKey = merchantKey;
        }

        public string GetMerchantPass()
        {
            return MerchantPass;
        }

        public void SetMerchantPass(string merchantPass)
        {
            MerchantPass = merchantPass;
        }

        public EnvironmentEnum? GetEnvironment()
        {
            return Environment;
        }

        public void SetEnvironment(EnvironmentEnum environment)
        {
            Environment = environment;
        }

        public string GetProductId()
        {
            return ProductId;
        }

        public void SetProductId(string productId)
        {
            if (!GeneralUtils.IsNumbersOnly(productId) || productId.Length < 6 || productId.Length > 11)
            {
                throw new InvalidFieldException("productId: Should be numbers in size 6 <= productId <= 11");
            }
            ProductId = productId;
        }

        public int GetApiVersion()
        {
            return ApiVersion;
        }

        public void SetApiVersion(int apiVersion)
        {
            if (apiVersion < 0)
            {
                throw new InvalidFieldException("apiVersion: Should be (apiVersion > 0)");
            }
            ApiVersion = apiVersion;
        }
    }
}