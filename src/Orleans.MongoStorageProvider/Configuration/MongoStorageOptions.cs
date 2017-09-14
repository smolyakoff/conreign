using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Driver;

namespace Orleans.MongoStorageProvider.Configuration
{
    public class MongoStorageOptions
    {
        public const string DefaultConnectionString = "mongodb://localhost:27017/orleans_grains";

        public MongoStorageOptions() : this(DefaultConnectionString)
        {
        }

        public MongoStorageOptions(string connectionString) 
            : this(connectionString, new List<Assembly>(0))
        {
        }

        public MongoStorageOptions(string connectionString, IEnumerable<Assembly> grainAssemblies) 
            : this(connectionString, grainAssemblies, new List<Assembly>(0))
        {
        }

        public MongoStorageOptions(
            string connectionString, 
            IEnumerable<Assembly> grainAssemblies, 
            IEnumerable<Assembly> bootstrapAssemblies)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            ConnectionString = connectionString;
            GrainAssemblies = grainAssemblies?.ToList() ?? throw new ArgumentNullException(nameof(grainAssemblies));
            BootstrapAssemblies = bootstrapAssemblies?.ToList() ?? throw new ArgumentNullException(nameof(bootstrapAssemblies));
        }

        public string ConnectionString { get; set; }

        public List<Assembly> GrainAssemblies { get; set; }

        public List<Assembly> BootstrapAssemblies { get; set; }

        public string CollectionNamePrefix { get; set; }

        public override string ToString()
        {
            var properties = this.ToDictionary();
            var connectionString = properties[nameof(ConnectionString)];
            properties[nameof(ConnectionString)] = SanitizeConnectionString(connectionString);
            return string.Join(", ", properties.Select((k, v) => $"{k}={v}"));
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
    }
}