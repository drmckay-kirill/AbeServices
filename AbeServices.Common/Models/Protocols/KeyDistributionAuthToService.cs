using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class KeyDistributionAuthToService
    {
        [ProtoMember(1)]
        public int Nonce { get; set; }
    }
}
