using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class KeyDistributionStepThree
    {
        [ProtoMember(1)]
        public byte[] ServicePayload { get; set; }

        [ProtoMember(2)]
        public byte[] AbonentPayload { get; set; }
    }
}
