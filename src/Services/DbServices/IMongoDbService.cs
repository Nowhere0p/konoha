using System.Linq.Expressions;
using MongoDB.Bson;

public interface IMongoDbService<T>
{
    Task DeleteAsync(string id);
    Task<T> GetItemAsync(string id);
    Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
    Task<bool> SaveAsync(T record);
    Task<bool> UpdateAsync(string id, T record);
}
