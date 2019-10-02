using System;  
using System.IO;  
using System.Security.Cryptography;  
using System.Text;

namespace AbeServices.Common.Helpers
{
    public static class SymmetricEncryption
    {
        public static byte[] EncryptString(string key, string plainText)
        {
            return Encrypt(key, Encoding.UTF8.GetBytes(plainText));
        }

        public static string DecryptToString(string key, byte[] cipherText)
        {
            var res = Decrypt(key, cipherText);
            return Encoding.UTF8.GetString(res);
        }

        public static byte[] Encrypt(string key, byte[] plainText)  
        {
            byte[] iv = new byte[16];
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = Encoding.UTF8.GetBytes(key);
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

        public static byte[] Decrypt(string key, byte[] cipherText)  
        {
            byte[] iv = new byte[16];
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = Encoding.UTF8.GetBytes(key);
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