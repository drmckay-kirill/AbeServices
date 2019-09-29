using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AbeServices.AttributeAuthority.Models
{
    // События генерации приватного ключа
    public class KeyEvent
    {
        // Дата события
        [BsonRequired]
        public DateTime EventDate { get; set; } 
    }
}