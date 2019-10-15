using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class AbeAuthStepOne
    {
        [ProtoMember(1)]
        public string[] AccessPolicy { get; set; }

        [ProtoMember(2)]
        public byte[] Z { get; set;}
    }
}