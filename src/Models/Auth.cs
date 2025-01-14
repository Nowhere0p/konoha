using System.Text.Json.Serialization;

namespace Konoha.Models
{
    public class Login
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}

public class AuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}
