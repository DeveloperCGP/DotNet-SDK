using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPaymentSDK.Utilities
{
    public static class SecurityUtils
    {
        public static readonly Random random = new();
        public static string Sha256Hash(string data)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] byteArrayData = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(byteArrayData).Replace("-", string.Empty).ToLowerInvariant();
        }

        public static byte[] CbcEncryption(string dataToBeEncrypted, string key, byte[]  iv) {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);
            byte[] encrypted;
            using (MemoryStream msEncrypt = new())
            {
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(dataToBeEncrypted);
                }
                encrypted = msEncrypt.ToArray();
            }
            return encrypted;
        }

        public static byte[] GenerateIV() {
            byte[] iv = new byte[16];
            random.NextBytes(iv);
            return iv;
        }
    }
}