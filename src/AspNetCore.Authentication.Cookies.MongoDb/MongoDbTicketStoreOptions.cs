using System;
using MongoDB.Driver;

namespace AspNetCore.Authentication.Cookies.MongoDb
{
    public class MongoDbTicketStoreOptions
    {
        public IMongoDatabase Database { get; set; }
        public string CollectionName { get; set; } = "authTicketStore";
        public string NameClaimType { get; set; }

        internal void ValidateOptions()
        {
            if (Database == null)
            {
                throw new ArgumentException("Database cannot be null", nameof(Database));
            }

            if (string.IsNullOrWhiteSpace(CollectionName))
            {
                throw new ArgumentException("CollectionName cannot be null", nameof(CollectionName));
            }
        }
    }
}