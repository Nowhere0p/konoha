using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Konoha.Models;

public class UserDetails : User 

{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("publicUsername")]
    public string? PublicUsername { get; set; }
    [JsonPropertyName("rollNumber")]
    public string? RollNumber { get; set; }
    [JsonPropertyName("role")]
    public Role Role { get; set; } = Role.User;
    
    [JsonPropertyName("gender")]
    public Gender? Gender { get; set; }

    // [JsonPropertyName("dateOfBirth")]
    // public DateOnly? DateOfBirth { get; set; }

    // [JsonPropertyName("phoneNumber")]
    // public string PhoneNumber { get; set; } = null!;

    // [JsonPropertyName("address")]
    // public string Address { get; set; } = null!;
    

    public UserDetails()
    {
        Id = Guid.NewGuid().ToString();
        base.PartitionKey = DateTime.UtcNow.ToString("MM-yyyy");
    }
    
    public string ToStringInsecure(bool serializeCredentials = false)
    {
        string text = JsonSerializer.Serialize(this);
        if (!serializeCredentials && base.Password != null)
        {
            return text.Replace(base.Password, "_REDACTED_");
        }

        return text;
    }
}
public enum Role
    {
        User,
        Admin,
        SuperAdmin,
    }
  public enum Gender
    {
        Male,
        Female,
        NonBinary,
        PreferNotToSay,
        Other
    }
 