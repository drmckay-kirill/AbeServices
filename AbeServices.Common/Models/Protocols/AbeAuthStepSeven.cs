using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class AbeAuthStepSeven
    {
        [ProtoMember(1)]
        public byte[] HMAC { get; set; }
    }
}