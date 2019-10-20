using System;
using System.Security.Cryptography;
using System.Text;

namespace AbeServices.Common.Helpers
{
    public static class CryptoHelper
    {
        public static int GetNonce()
        {
            var rnd = new Random();
            var nonce = rnd.Next();
            return nonce;
        }

        public static byte[] ComputeHash(byte[] data)
        {
            using (var hashAlgorithm = SHA256.Create())
            {
                return hashAlgorithm.ComputeHash(data);
            }
        }


        public static byte[] ComputeHash(byte[] first, byte[] second)
        {
            byte[] combined = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combined, 0, first.Length);
            Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
            return ComputeHash(combined);
        }
        public static byte[] ComputeHash(string data)
        {
            return ComputeHash(Encoding.UTF8.GetBytes(data));
        }
    }
}