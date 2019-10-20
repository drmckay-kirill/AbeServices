using System;

namespace AbeServices.TokenGeneration.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public string[] AccessPolicy { get; set; }
        public string[] AbonentAttributes { get; set; }
        public int Nonce1 { get; set; }
        public int Nonce2 { get; set; }
        public int Nonce3 { get; set; }
        public byte[] HMAC { get; set; }
        public bool IsProcessed { get; set; }
    }
}