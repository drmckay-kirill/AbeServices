using ProtoBuf;

namespace AbeServices.Common.Models.Protocols
{
    [ProtoContract]
    public class KeyDistributionRequestPayload
    {
        // Идентификатор абонента, запрашивающего секретный ключ
        [ProtoMember(1)]
        public string AbonentId { get; set; }

        // Идентификатор промежуточного сервиса раздачи ключей
        [ProtoMember(2)]
        public string KeyServiceId { get; set; }

        // Идентификатор атрибутивного центра
        [ProtoMember(3)]
        public string AttributeAuthorityId { get; set; }

        // Атрибуты абонента
        [ProtoMember(4)]
        public string[] Attributes { get; set; }

        [ProtoMember(5)]
        public int Nonce { get; set; }
    }
}