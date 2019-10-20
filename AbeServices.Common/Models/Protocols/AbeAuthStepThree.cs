using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class AbeAuthStepThree
    {
        [ProtoMember(1)]
        public int Nonce { get; set; }

        [ProtoMember(2)]
        public byte[] CtAbonent { get; set; }

        [ProtoMember(3)]
        public byte[] CtAccess { get; set; }

        [ProtoMember(4)]
        public byte[] NonceHash { get; set; }
    }
}