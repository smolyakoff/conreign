using System;
using System.Threading.Tasks;
using Microsoft.Orleans.Storage.Conventions;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Serialization;
using Orleans.Storage;

namespace Microsoft.Orleans.Storage
{
    public class MongoStorage : IStorageProvider
    {
        private static bool _isInitialized;

        private MongoClient _client;

        private IMongoDatabase _database;

        private MongoStorageOptions _options;
        private Guid _serviceId;

        public static void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;
            var conventions = new ConventionPack
            {
                new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfArrays)
            };
            ConventionRegistry.Register("Orleans", conventions, x => true);
        }

        public string Name { get; private set; }

        public Logger Log { get; private set; }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            EnsureInitialized();
            Name = name;
            _serviceId = providerRuntime.ServiceId;
            Log = providerRuntime.GetLogger($"Storage.MongoStorage.{_serviceId}");
            if (Log.IsVerbose3)
            {
                Log.Verbose3($"Going to initialize mongo storage client.");
            }
            _options = MongoStorageOptions.Create(config);
            _client = new MongoClient(_options.ConnectionString);
            _database = _client.GetDatabase(_options.DatabaseName);
            LogInfo($"Initialized mongo client with options: {_options}", MongoStorageCode.InfoInit);
            return TaskDone.Done;
        }

        public Task Close()
        {
            _client = null;
            _database = null;
            LogInfo("Closed mongo storage provider.");
            return TaskDone.Done;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            EnsureNotClosed();
            var meta = grainState.ToGrainMeta(grainReference, grainType, _serviceId);
            try
            {
                var collectionName = Keys.CollectionNameForGrain(grainType, _options.CollectionNamePrefix);
                var collection = _database.GetCollection<MongoGrain>(collectionName);
                var id = Keys.PrimaryKeyForGrain(_serviceId, grainReference);
                LogVerbose3($"Reading data: {meta}", MongoStorageCode.TraceReading);
                var doc = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (doc == null)
                {
                    LogVerbose3($"Grain state not found. {meta}", MongoStorageCode.TraceNotFound);
                    return;
                }
                grainState.ETag = doc.Meta.ETag;
                grainState.State = SerializationManager.DeserializeFromByteArray<object>(doc.Data);
                LogVerbose3($"Read data: {meta}", MongoStorageCode.TraceRead);
            }
            catch (Exception ex)
            {
                LogError($"Error while reading. {meta} Error={ex.Message}", MongoStorageCode.ErrorRead, ex);
                throw;
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            EnsureNotClosed();
            var meta = grainState.ToGrainMeta(grainReference, grainType, _serviceId);
            try
            {
                var collectionName = Keys.CollectionNameForGrain(grainType, _options.CollectionNamePrefix);
                var collection = _database.GetCollection<MongoGrain>(collectionName);
                LogVerbose3($"Writing data: {meta}", MongoStorageCode.TraceWriting);
                if (string.IsNullOrEmpty(grainState.ETag))
                {
                    var grain = grainState.ToGrain(grainReference, grainType, _serviceId);
                    grain.Meta.Timestamp = DateTime.UtcNow.Ticks;
                    await collection.InsertOneAsync(grain);
                    grainState.ETag = grain.Meta.ETag;
                }
                else
                {
                    var id = Keys.PrimaryKeyForGrain(_serviceId, grainReference);
                    // Update data and generate new timestamp
                    var update = Builders<MongoGrain>.Update
                        .Set(x => x.Data, SerializationManager.SerializeToByteArray(grainState.State))
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
                LogVerbose3($"Wrote data: {meta}", MongoStorageCode.TraceWrite);
            }
            catch (Exception ex)
            {
                LogError($"Error while writing. {meta} Error={ex.Message}", MongoStorageCode.ErrorWrite, ex);
                throw;
            }
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            EnsureNotClosed();
            var meta = grainState.ToGrainMeta(grainReference, grainType, _serviceId);
            try
            {
                var collectionName = Keys.CollectionNameForGrain(grainType, _options.CollectionNamePrefix);
                var collection = _database.GetCollection<MongoGrain>(collectionName);
                LogVerbose3($"Deleting data: {meta}", MongoStorageCode.TraceDeleting);
                var id = Keys.PrimaryKeyForGrain(_serviceId, grainReference);
                await collection.DeleteOneAsync(x => x.Id == id);
                grainState.ETag = null;
                LogVerbose3($"Deleted data: {meta}", MongoStorageCode.TraceDelete);
            }
            catch (Exception ex)
            {
                LogError($"Error while deleting. {meta} Error={ex.Message}", MongoStorageCode.ErrorDelete, ex);
                throw;
            }
        }

        private void EnsureNotClosed()
        {
            if (_database == null)
            {
                LogWarn("Invoked ReadStateAsync() when database instance is null.",
                    MongoStorageCode.ErrorInvalidOperation);
                throw new ObjectDisposedException($"MongoProvider has been already closed.");
            }
        }

        private void LogInfo(string message, MongoStorageCode? code = null)
        {
            if (!Log.IsInfo)
            {
                return;
            }
            if (code == null)
            {
                Log.Info(FormatMessage(message));
            }
            else
            {
                Log.Info((int) code.Value, FormatMessage(message));
            }
        }

        private void LogWarn(string message, MongoStorageCode code)
        {
            if (!Log.IsWarning)
            {
                return;
            }
            Log.Warn((int) code, FormatMessage(message));
        }

        private void LogError(string message, MongoStorageCode code, Exception ex)
        {
            Log.Error((int) code, FormatMessage(message), ex);
        }

        private void LogVerbose3(string message, MongoStorageCode? code = null)
        {
            if (!Log.IsVerbose3)
            {
                return;
            }
            if (code == null)
            {
                Log.Verbose3(FormatMessage(message));
            }
            else
            {
                Log.Verbose3((int) code.Value, FormatMessage(message));
            }
        }

        private string FormatMessage(string message)
        {
            return $"[MongoStorage:{Name}] {message}";
        }
    }
}