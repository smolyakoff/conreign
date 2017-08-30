namespace Microsoft.Orleans.MongoStorage
{
    public enum MongoStorageCode
    {
        ProvidersBase = 200000,
        Base = ProvidersBase + 500,
        InfoInit = Base + 1,
        TraceReading = Base + 2,
        TraceWriting = Base + 3,
        TraceDeleting = Base + 4,
        TraceRead = Base + 5,
        TraceWrite = Base + 6,
        TraceDelete = Base + 7,
        ErrorRead = Base + 8,
        ErrorWrite = Base + 9,
        ErrorDelete = Base + 10,
        TraceNotFound = Base + 11,
        ErrorInvalidOperation = Base + 12
    }
}