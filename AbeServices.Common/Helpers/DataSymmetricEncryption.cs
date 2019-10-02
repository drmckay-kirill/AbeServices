using System;  
using System.IO;  
using System.Security.Cryptography;  
using System.Text;

namespace AbeServices.Common.Helpers
{
    public class DataSymmetricEncryption: IDataSymmetricEncryptor
    {
        private string _key;

        public DataSymmetricEncryption() { }

        public DataSymmetricEncryption(string symmetricKey)
        {
            _key = symmetricKey;
        }

        public void SetKey(string key)
        {
            _key = key;
        }

        public byte[] EncryptString(string plainText)
        {
            return Encrypt(Encoding.UTF8.GetBytes(plainText));
        }

        public string DecryptToString(byte[] cipherText)
        {
            var res = Decrypt(cipherText);
            return Encoding.UTF8.GetString(res);
        }

        public byte[] Encrypt(byte[] plainText)  
        {
            byte[] iv = new byte[16];
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = Encoding.UTF8.GetBytes(_key);
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainText, 0, plainText.Length);
                            cryptoStream.FlushFinalBlock();

                            return ms.ToArray();
                        }
                    }
                }
            }
        }

        public byte[] Decrypt(byte[] cipherText)  
        {
            byte[] iv = new byte[16];
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = Encoding.UTF8.GetBytes(_key);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(cipherText, 0, cipherText.Length);
                            cryptoStream.FlushFinalBlock();

                            return ms.ToArray();
                        }
                    }
                }
            }  
        }  
    }
}