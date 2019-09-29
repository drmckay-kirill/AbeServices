using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AbeServices.AttributeAuthority.Models
{
    // Коллекция субъектов доступа
    public class Login
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Уникальное имя
        [BsonRequired]
        [BsonElement("Login")]
        public string Name { get; set; }

        // Общий ключ для симметричного шифрования
        [BsonRequired]
        public string SharedKey { get; set; }

        // Атрибуты субъекта
        public string[] Attributes { get; set; }

        // Список событий генерации секретного ключа
        public KeyEvent[] KeyEvents { get; set; }
    }
}