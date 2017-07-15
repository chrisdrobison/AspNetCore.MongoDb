using System;
using MongoDB.Driver;

namespace AspNetCore.DataProtection.MongoDb.Tests
{
    public class DatabaseFixture : IDisposable
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _databaseName = Guid.NewGuid().ToString();

        public IMongoDatabase Database { get; }

        public DatabaseFixture()
        {
            _mongoClient = new MongoClient("mongodb://localhost");
            Database = _mongoClient.GetDatabase(_databaseName);
        }

        public void Dispose()
        {
            _mongoClient.DropDatabase(_databaseName);
        }
    }
}