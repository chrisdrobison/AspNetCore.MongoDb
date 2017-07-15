using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AspNetCore.DataProtection.MongoDb
{
    public class KeyElement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Xml { get; set; }
    }
}