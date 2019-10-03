using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class KeyDistrubutionStepOne
    {
        [ProtoMember(1)]
        public string AbonentId { get; set; }

        [ProtoMember(2)]
        public string AttributeAuthorityId { get; set; }

        [ProtoMember(3)]
        public string[] Attributes { get; set; }

        [ProtoMember(4)]
        public byte[] Payload { get; set; }
    }
}