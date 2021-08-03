using System;
using AspNetCore.DataProtection.MongoDb;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.DataProtection
{
    public static class MongoDbDataProtectionBuilderExtensions
    {
        private const string DataProtectionKeysCollectionName = "dataProtectionKeys";

        public static IDataProtectionBuilder PersistKeysToMongoDb(this IDataProtectionBuilder builder,
            Func<IMongoDatabase> databaseFactory,
            string collectionName = DataProtectionKeysCollectionName)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (databaseFactory == null)
                throw new ArgumentNullException(nameof(databaseFactory));
            builder.Services.Configure<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new MongoDbXmlRepository(databaseFactory, collectionName);
            });
            return builder;
        }

        public static IDataProtectionBuilder PersistKeysToMongoDb(this IDataProtectionBuilder builder,
            Func<IServiceProvider, IMongoDatabase> databaseFactory,
            string collectionName = DataProtectionKeysCollectionName)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (databaseFactory == null)
                throw new ArgumentNullException(nameof(databaseFactory));
            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
            {
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    options.XmlRepository = new MongoDbXmlRepository(() => databaseFactory(services), collectionName);
                });
            });
            return builder;
        }
    }
}