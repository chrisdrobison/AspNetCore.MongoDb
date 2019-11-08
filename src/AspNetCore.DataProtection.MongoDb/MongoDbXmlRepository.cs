using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using MongoDB.Driver;

namespace AspNetCore.DataProtection.MongoDb
{
    public class MongoDbXmlRepository : IXmlRepository
    {
        private readonly IMongoCollection<KeyElement> _collection;

        public MongoDbXmlRepository(Func<IMongoDatabase> databaseFactory, string collectionName)
        {
            _collection = databaseFactory().GetCollection<KeyElement>(collectionName);
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var keyElements = _collection.Find(FilterDefinition<KeyElement>.Empty).ToList();
            return keyElements.Select(element => XElement.Parse(element.Xml)).ToList().AsReadOnly();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var keyElement = new KeyElement
            {
                Xml = element.ToString(SaveOptions.DisableFormatting)
            };
            _collection.InsertOne(keyElement);
        }
    }
}