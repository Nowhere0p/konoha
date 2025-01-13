using System.Text.Json.Serialization;

namespace Konoha.Models;
public class User: IMongoDbRecord
{

    [JsonPropertyName("userName")]
    public string? UserName { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")] 
    public string? LastName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }


    [JsonPropertyName("partitionKey")]
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

