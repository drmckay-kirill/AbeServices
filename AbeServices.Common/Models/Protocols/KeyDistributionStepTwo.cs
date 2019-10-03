using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class KeyDistributionStepTwo
    {
        [ProtoMember(1)]
        public string KeyServiceId { get; set; }

        [ProtoMember(2)]
        public string AbonentId { get; set; }

        [ProtoMember(3)]
        public byte[] KeyServicePayload { get; set; }

        [ProtoMember(4)]
        public byte[] AbonentPayload { get; set; }
    }
}
