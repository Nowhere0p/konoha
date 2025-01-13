using MongoDB.Bson;

public interface IMongoDbService<T>
{
    // Core database operations
    Task<bool> SaveAsync();
    Task<bool> UpdateAsync();
    Task<bool> DeleteAsync();
    Task<T> GetAsync();
}
