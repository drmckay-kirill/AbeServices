using System;

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
    }
}