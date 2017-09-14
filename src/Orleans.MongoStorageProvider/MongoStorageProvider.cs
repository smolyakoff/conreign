using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Orleans.MongoStorageProvider.Configuration;
using Orleans.MongoStorageProvider.Driver;
using Orleans.MongoStorageProvider.Model;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;

namespace Orleans.MongoStorageProvider
{
    public class MongoStorageProvider : IStorageProvider
    {
        private static readonly object DriverInitializationLock = new object();
        private static bool _isDriverInitialized;
        private MongoClient _client;
        private IMongoDatabase _database;
        
        private MongoStorageOptions _options;
        private Guid _serviceId;
        private IGrainReferenceConverter _grainReferenceConverter;

        public string Name { get; private set; }

        public Logger Log { get; private set; }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Name = name;
            _serviceId = providerRuntime.ServiceId;
            _grainReferenceConverter = providerRuntime.ServiceProvider.GetRequiredService<IGrainReferenceConverter>();
            Log = providerRuntime.GetLogger($"Storage.MongoStorage.{_serviceId}");
            if (Log.IsVerbose3)
            {
                Log.Verbose3("Going to initialize MongoDB storage.");
            }
            _options = config.DeserializeToMongoStorageOptions();
            MongoUrl mongoUrl;
            try
            {
                mongoUrl = MongoUrl.Create(_options.ConnectionString);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Invalid MongoDB connection string, failed to initialize.", ex);
            }
            EnsureDriverInitialized(_options.BootstrapAssemblies.SelectMany(a => a.GetMongoDriverBootstraps()));
            _client = new MongoClient(_options.ConnectionString);
            _database = _client.GetDatabase(mongoUrl.DatabaseName);
            LogInfo($"Initialized MongoDB storage with options: {_options}.", MongoStorageLogCode.InfoInit);
            return Task.CompletedTask;
        }

        public Task Close()
        {
            _client = null;
            _database = null;
            LogInfo("Closed MongoDB storage.");
            return Task.CompletedTask;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            EnsureNotClosed();
            var meta = grainState.ToGrainMeta(grainReference, grainType, _serviceId);
            try
            {
                var collectionName = Naming.CollectionNameForGrain(grainType, _options.CollectionNamePrefix);
                var collection = _database.GetCollection<MongoGrain>(collectionName);
                var id = Naming.PrimaryKeyForGrain(_serviceId, grainReference);
                LogVerbose3($"Reading data: {meta}", MongoStorageLogCode.TraceReading);
                var doc = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (doc == null)
                {
                    LogVerbose3($"Grain state not found. {meta}", MongoStorageLogCode.TraceNotFound);
                    return;
                }
                grainState.ETag = doc.Meta.ETag;
                grainState.State = doc.Data;
                LogVerbose3($"Read data: {meta}", MongoStorageLogCode.TraceRead);
            }
            catch (Exception ex)
            {
                LogError($"Error while reading. {meta} Error={ex.Message}", MongoStorageLogCode.ErrorRead, ex);
                throw;
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            EnsureNotClosed();
            var meta = grainState.ToGrainMeta(grainReference, grainType, _serviceId);
            try
            {
                var collectionName = Naming.CollectionNameForGrain(grainType, _options.CollectionNamePrefix);
                var collection = _database.GetCollection<MongoGrain>(collectionName);
                LogVerbose3($"Writing data: {meta}", MongoStorageLogCode.TraceWriting);
                if (string.IsNullOrEmpty(grainState.ETag))
                {
                    var grain = grainState.ToGrain(grainReference, grainType, _serviceId);
                    grain.Meta.Timestamp = DateTime.UtcNow.Ticks;
                    await collection.InsertOneAsync(grain);
                    grainState.ETag = grain.Meta.ETag;
                }
                else
                {
                    var id = Naming.PrimaryKeyForGrain(_serviceId, grainReference);
                    // Update data and generate new timestamp
                    var update = Builders<MongoGrain>.Update
                        .Set(x => x.Data, grainState.State)
                        .Set(x => x.Meta.Timestamp, DateTime.UtcNow.Ticks);
                    var options = new FindOneAndUpdateOptions<MongoGrain>
                    {
                        IsUpsert = false,
                        ReturnDocument = ReturnDocument.After,
                        Projection = Builders<MongoGrain>.Projection.Include(x => x.Meta)
                    };
                    var updatedGrain = await collection.FindOneAndUpdateAsync<MongoGrain>(
                        x => x.Id == id && x.Meta.Timestamp == meta.Timestamp,
                        update,
                        options);
                    if (updatedGrain == null)
                    {
                        var message =
                            $"Inconsistent state for grain: {meta}. ETag has changed or document has already been deleted.";
                        throw new InconsistentStateException(message);
                    }
                    grainState.ETag = updatedGrain.Meta.ETag;
                }
                LogVerbose3($"Wrote data: {meta}", MongoStorageLogCode.TraceWrite);
            }
            catch (Exception ex)
            {
                LogError($"Error while writing. {meta} Error={ex.Message}", MongoStorageLogCode.ErrorWrite, ex);
                throw;
            }
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            EnsureNotClosed();
            var meta = grainState.ToGrainMeta(grainReference, grainType, _serviceId);
            try
            {
                var collectionName = Naming.CollectionNameForGrain(grainType, _options.CollectionNamePrefix);
                var collection = _database.GetCollection<MongoGrain>(collectionName);
                LogVerbose3($"Deleting data: {meta}", MongoStorageLogCode.TraceDeleting);
                var id = Naming.PrimaryKeyForGrain(_serviceId, grainReference);
                await collection.DeleteOneAsync(x => x.Id == id);
                grainState.ETag = null;
                LogVerbose3($"Deleted data: {meta}", MongoStorageLogCode.TraceDelete);
            }
            catch (Exception ex)
            {
                LogError($"Error while deleting. {meta} Error={ex.Message}", MongoStorageLogCode.ErrorDelete, ex);
                throw;
            }
        }

        // Driver is initialized late to give chance for custom conventions and class maps to be registered
        // StorageProvider Init() is called before any custom bootstrap provider
        private void EnsureDriverInitialized(IEnumerable<IMongoDriverBootstrap> bootstraps)
        {
            if (_isDriverInitialized)
            {
                return;
            }
            lock (DriverInitializationLock)
            {
                foreach (var bootstrap in bootstraps)
                {
                    bootstrap.Init();
                }
                BsonSerializer.RegisterSerializationProvider(new OrleansSerializerProvider(_grainReferenceConverter));
                // Register class maps if they are not registered yet
                foreach (var grainStateType in _options.GrainAssemblies.SelectMany(a => a.GetGrainStateTypes()))
                    BsonClassMap.LookupClassMap(grainStateType);
                _isDriverInitialized = true;
            }
        }

        private void EnsureNotClosed()
        {
            if (_database == null)
            {
                LogWarn("Invoked ReadStateAsync() when database instance is null.",
                    MongoStorageLogCode.ErrorInvalidOperation);
                throw new ObjectDisposedException("MongoProvider has been already closed.");
            }
        }

        private void LogInfo(string message, MongoStorageLogCode? logCode = null)
        {
            if (!Log.IsInfo)
            {
                return;
            }
            if (logCode == null)
            {
                Log.Info(FormatMessage(message));
            }
            else
            {
                Log.Info((int) logCode.Value, FormatMessage(message));
            }
        }

        private void LogWarn(string message, MongoStorageLogCode logCode)
        {
            if (!Log.IsWarning)
            {
                return;
            }
            Log.Warn((int) logCode, FormatMessage(message));
        }

        private void LogError(string message, MongoStorageLogCode logCode, Exception ex)
        {
            Log.Error((int) logCode, FormatMessage(message), ex);
        }

        private void LogVerbose3(string message, MongoStorageLogCode? logCode = null)
        {
            if (!Log.IsVerbose3)
            {
                return;
            }
            if (logCode == null)
            {
                Log.Verbose3(FormatMessage(message));
            }
            else
            {
                Log.Verbose3((int) logCode.Value, FormatMessage(message));
            }
        }

        private string FormatMessage(string message)
        {
            return $"[MongoStorage:{Name}] {message}";
        }
    }
}