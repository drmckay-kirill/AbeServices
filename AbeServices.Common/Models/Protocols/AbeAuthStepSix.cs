using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class AbeAuthStepSix
    {
        [ProtoMember(1)]
        public byte[] CtPep { get; set; }

        [ProtoMember(2)]
        public byte[] HMAC { get; set; }
    }
}