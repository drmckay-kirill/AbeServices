namespace AbeServices.Common.Helpers
{
    public interface IDataSymmetricEncryptor
    {
        void SetKey(string key);
        byte[] EncryptString(string plainText);
        string DecryptToString(byte[] cipherText);
        byte[] Encrypt(byte[] plainText);
        byte[] Decrypt(byte[] cipherText);
        byte[] GenerateKey();
    }
}