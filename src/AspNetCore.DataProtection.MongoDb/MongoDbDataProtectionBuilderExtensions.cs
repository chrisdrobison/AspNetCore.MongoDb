using System;
using AspNetCore.DataProtection.MongoDb;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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
            builder.Services.TryAddSingleton<IXmlRepository>(provider => new MongoDbXmlRepository(databaseFactory,
                collectionName, provider.GetService<ILogger<MongoDbXmlRepository>>()));
            return builder;
        }
    }
}