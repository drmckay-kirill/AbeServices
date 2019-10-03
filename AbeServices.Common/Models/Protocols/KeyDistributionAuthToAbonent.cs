using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class KeyDistributionAuthToAbonent
    {
        [ProtoMember(1)]
        public int Nonce { get; set; }

        [ProtoMember(2)]
        public byte[] PublicKey { get; set; }

        [ProtoMember(3)]
        public byte[] SecretKey { get; set; }
    }
}
