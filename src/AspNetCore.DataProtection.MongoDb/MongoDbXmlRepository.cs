using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AspNetCore.DataProtection.MongoDb
{
    public class MongoDbXmlRepository : IXmlRepository
    {
        private readonly string _collectionName;
        private readonly ILogger<MongoDbXmlRepository> _logger;
        private readonly Func<IMongoDatabase> _databaseFactory;

        public MongoDbXmlRepository(Func<IMongoDatabase> databaseFactory, string collectionName, ILogger<MongoDbXmlRepository> logger)
        {
            _databaseFactory = databaseFactory;
            _collectionName = collectionName;
            _logger = logger;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var collection = GetCollection();
            var keyElements = collection.Find(FilterDefinition<KeyElement>.Empty).ToList();
            _logger.LogInformation($"Found {keyElements.Count} key(s)");
            return keyElements.Select(element => XElement.Parse(element.Xml)).ToList().AsReadOnly();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var collection = GetCollection();
            var keyElement = new KeyElement()
            {
                Xml = element.ToString(SaveOptions.DisableFormatting)
            };
            collection.InsertOne(keyElement);

            const int prefixLength = 50;
            _logger.LogInformation($"Stored key '{keyElement.Xml.Substring(0, Math.Min(prefixLength, keyElement.Xml.Length))}...' with id {keyElement.Id}");
        }

        private IMongoCollection<KeyElement> GetCollection()
        {
            var database = _databaseFactory();
            var collection = database.GetCollection<KeyElement>(_collectionName);
            return collection;
        }
    }
}