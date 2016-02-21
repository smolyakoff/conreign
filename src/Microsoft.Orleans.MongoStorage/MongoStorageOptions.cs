using System;
using MongoDB.Driver;
using Orleans.Providers;

namespace Microsoft.Orleans.Storage
{
    public class MongoStorageOptions
    {
        private const string DefaultDatabaseName = "orleans_storage";

        public const string DefaultConnectionString = "mongodb://localhost:27017";

        private MongoStorageOptions()
        {
        }

        public string ConnectionString { get; private set; }

        public string DatabaseName { get; private set; }

        public string CollectionNamePrefix { get; private set; }

        public static MongoStorageOptions Create(IProviderConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            var options = new MongoStorageOptions();
            var connectionString = config.GetProperty(Keys.ConnectionString, DefaultConnectionString);
            var databaseName = config.GetProperty(Keys.DatabaseName, DefaultDatabaseName);
            var collectionNamePrefix = config.GetProperty(Keys.CollectionNamePrefix, null);
            options.ConnectionString = Validator.EnsureIsValid(connectionString, Keys.ConnectionString,
                Validations.Required, Validations.ValidMongoUrl);
            options.DatabaseName = Validator.EnsureIsValid(databaseName, Keys.DatabaseName, Validations.Required);
            options.CollectionNamePrefix = collectionNamePrefix;
            return options;
        }

        public override string ToString()
        {
            return
                $"ConnectionString={SanitizeConnectionString(ConnectionString)}, DatabaseName={DatabaseName}, CollectionNamePrefix={CollectionNamePrefix}";
        }

        private static string SanitizeConnectionString(string connectionString)
        {
            var url = MongoUrl.Create(connectionString);
            if (string.IsNullOrEmpty(url.Password))
            {
                return connectionString;
            }
            var secret = $"{url.Username}:{url.Password}";
            return connectionString.Replace(secret, "**AUTH**");
        }

        private static class Validations
        {
            public static string ValidMongoUrl(string x)
            {
                try
                {
                    MongoUrl.Create(x);
                    return null;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            public static string Required(string x) => string.IsNullOrEmpty(x) ? "value is required" : null;
        }

        private static class Keys
        {
            public const string ConnectionString = "ConnectionString";

            public const string DatabaseName = "DatabaseName";

            public const string CollectionNamePrefix = "CollectionNamePrefix";
        }
    }
}