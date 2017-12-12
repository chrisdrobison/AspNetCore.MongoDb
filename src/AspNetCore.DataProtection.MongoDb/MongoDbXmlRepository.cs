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
        private readonly Func<IMongoDatabase> _databaseFactory;

        public MongoDbXmlRepository(Func<IMongoDatabase> databaseFactory, string collectionName)
        {
            _databaseFactory = databaseFactory;
            _collectionName = collectionName;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var collection = GetCollection();
            var keyElements = collection.Find(FilterDefinition<KeyElement>.Empty).ToList();
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
        }

        private IMongoCollection<KeyElement> GetCollection()
        {
            var database = _databaseFactory();
            var collection = database.GetCollection<KeyElement>(_collectionName);
            return collection;
        }
    }
}