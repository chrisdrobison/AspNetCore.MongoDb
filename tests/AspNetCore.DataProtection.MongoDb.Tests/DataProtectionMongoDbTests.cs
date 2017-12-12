using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace AspNetCore.DataProtection.MongoDb.Tests
{
    public class DataProtectionMongoDbTests : IClassFixture<DatabaseFixture>
    {
        public DataProtectionMongoDbTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        private readonly DatabaseFixture _databaseFixture;

        [Fact]
        public void GetAllElements_ReturnsAllXmlValuesForGivenKey()
        {
            const string collectionName = "test1";
            var collection = _databaseFixture.Database.GetCollection<KeyElement>(collectionName);
            collection.InsertMany(new[]
            {
                new KeyElement {Xml = "<Element1/>"},
                new KeyElement {Xml = "<Element2/>"}
            });
            var loggerMock = new Mock<ILogger<MongoDbXmlRepository>>();
            var repo = new MongoDbXmlRepository(() => _databaseFixture.Database, collectionName, loggerMock.Object);
            var elements = repo.GetAllElements().ToArray();
            Assert.Equal(new XElement("Element1").ToString(), elements[0].ToString());
            Assert.Equal(new XElement("Element2").ToString(), elements[1].ToString());
        }

        [Fact]
        public void GetAllElements_ThrowsParsingException()
        {
            const string collectionName = "test2";
            var collection = _databaseFixture.Database.GetCollection<KeyElement>(collectionName);
            collection.InsertMany(new[]
            {
                new KeyElement {Xml = "<Element1/>"},
                new KeyElement {Xml = "<Element2"}
            });
            var loggerMock = new Mock<ILogger<MongoDbXmlRepository>>();
            var repo = new MongoDbXmlRepository(() => _databaseFixture.Database, collectionName, loggerMock.Object);

            Assert.Throws<XmlException>(() => repo.GetAllElements());
        }

        [Fact]
        public void StoreElement_PushesValueToList()
        {
            const string collectionName = "test3";
            var loggerMock = new Mock<ILogger<MongoDbXmlRepository>>();
            var repo = new MongoDbXmlRepository(() => _databaseFixture.Database, collectionName, loggerMock.Object);
            repo.StoreElement(new XElement("Element2"), null);

            var collection = _databaseFixture.Database.GetCollection<KeyElement>(collectionName);
            var keyElement = collection.Find(FilterDefinition<KeyElement>.Empty).FirstOrDefault();
            Assert.NotNull(keyElement);
            Assert.Equal("<Element2 />", keyElement.Xml);
        }
    }
}