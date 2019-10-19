using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class AbeAuthStepTwo
    {
        [ProtoMember(1)]
        public string[] AccessPolicy { get; set; }
        
        [ProtoMember(2)]
        public string[] AbonentAttributes { get; set; }
        
        [ProtoMember(3)]
        public byte[] CT { get; set; }
        
        [ProtoMember(4)]
        public byte[] Z { get; set; }
    }
}