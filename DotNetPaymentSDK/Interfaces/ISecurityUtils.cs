namespace DotNetPaymentSDK.Interfaces
{
    public interface ISecurityUtils
    {
        byte[] GenerateIV();
        byte[] CbcEncryption(string data, string key, byte[] iv);
        string Sha256Hash(string data);
    }
}