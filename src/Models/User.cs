using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Konoha.Models;

public class User : IMongoDbRecord
{
    [BsonElement("userName")]
    public string? UserName { get; set; }

    [BsonElement("firstName")]
    public string? FirstName { get; set; }

    [BsonElement("lastName")]
    public string? LastName { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("password")]
    public string? Password { get; set; }

    [BsonElement("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [BsonElement("partitionKey")]
    public string PartitionKey { get; set; }

    public object GetPartitionKey()
    {
        return PartitionKey;
    }

    public User GetRedactedUser()
    {
        // Redact password for security
        Password = "[RedactedPassword]";
        return this;
    }
}
