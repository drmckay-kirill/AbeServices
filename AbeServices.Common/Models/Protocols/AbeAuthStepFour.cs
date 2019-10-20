using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class AbeAuthStepFour
    {
        [ProtoMember(1)]
        public byte[] NonceHash { get; set; }
    }
}