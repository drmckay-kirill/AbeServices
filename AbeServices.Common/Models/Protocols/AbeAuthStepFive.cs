using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class AbeAuthStepFive
    {
        [ProtoMember(1)]
        public byte[] CtAbonent { get; set; }

        [ProtoMember(2)]
        public byte[] CtPep { get; set; }

        [ProtoMember(3)]
        public byte[] Z { get; set; }
    }
}