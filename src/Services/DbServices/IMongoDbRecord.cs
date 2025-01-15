using MongoDB.Bson;

public interface IMongoDbRecord
{
    object GetPartitionKey();
}
