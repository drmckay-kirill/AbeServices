using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AbeServices.IoTA.Models
{
    public class Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }
        
        [BsonRequired]
        public string[] ReadAttributes { get; set; }
        
        [BsonRequired]
        public string[] WriteAttributes { get; set; }
    }
}