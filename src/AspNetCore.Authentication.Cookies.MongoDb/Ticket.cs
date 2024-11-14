using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace AspNetCore.Authentication.Cookies.MongoDb
{
    internal class Ticket
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public byte[] Value { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastActivity { get; set; }

        [BsonIgnoreIfNull]
        public DateTime? Expires { get; set; }
    }
}