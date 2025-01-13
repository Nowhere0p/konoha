using MongoDB.Driver;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public class MongoDbRecord<T> : IMongoDbService<T> where T : IMongoDbRecord
{
    private readonly IMongoDatabase _database;

    public MongoDbRecord(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<bool> SaveAsync()
    {
        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name.ToLower());
            var document = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(this));
            await collection.InsertOneAsync(document);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync()
    {
        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name.ToLower());
            var filter = Builders<T>.Filter.Eq("_id", this.GetType().GetProperty("Id")?.GetValue(this));
            var document = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(this));
            var result = await collection.ReplaceOneAsync(filter, document);
            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync()
    {
        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name.ToLower());
            var filter = Builders<T>.Filter.Eq("_id", this.GetType().GetProperty("Id")?.GetValue(this));
            var result = await collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<T> GetAsync()
    {
        try
        {
            var collection = _database.GetCollection<T>(typeof(T).Name.ToLower());
            var filter = Builders<T>.Filter.Eq("_id", this.GetType().GetProperty("Id")?.GetValue(this));
            return await collection.Find(filter).FirstOrDefaultAsync();
        }
        catch
        {
            return default(T);
        }
    }
}
