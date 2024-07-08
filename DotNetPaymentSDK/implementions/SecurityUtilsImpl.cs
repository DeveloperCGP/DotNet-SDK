using DotNetPaymentSDK.Interfaces;
using DotNetPaymentSDK.Utilities;

public class SecurityUtilsImpl : ISecurityUtils
{
    public byte[] GenerateIV()
    {
        return SecurityUtils.GenerateIV();
    }

    public byte[] CbcEncryption(string data, string key, byte[] iv)
    {
        return SecurityUtils.CbcEncryption(data, key, iv);
    }

    public string Sha256Hash(string data)
    {
        return SecurityUtils.Sha256Hash(data);
    }
}