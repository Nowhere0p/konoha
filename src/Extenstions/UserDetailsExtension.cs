using System.Text;
using Konoha.Models;

namespace Konoha.Extensions
{
    public static class UserDetailsExtension
    {
        public static UserDetails ToUserDetails(this RegisterInteraction registerInteraction)
        {
            return new UserDetails
            {
                FirstName = registerInteraction.FirstName,
                LastName = registerInteraction.LastName,
                Email = registerInteraction.Email,
                Password = HashPassword(registerInteraction.Password ?? ""),
                Gender = registerInteraction.Gender,
                UserId = Guid.NewGuid().ToString(),
                Role = Role.USER,
                CreatedAt = DateTime.UtcNow,
                PublicUsername = registerInteraction.GenerateUsername(),
            };
        }

        private static string GenerateUsername(this RegisterInteraction registerInteraction)
        {
            var random = new Random();
            var randomNumber = random.Next(100, 999);
            var username =
                $"{registerInteraction.FirstName}.{registerInteraction.LastName}{randomNumber}";
            return username.Length > 10 ? username.Substring(0, 10) : username;
        }

        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }

        public static UserDetails RedactPassword(this UserDetails userDetails)
        {
            userDetails.Password = "REDACTED";
            return userDetails;
        }
    }
}
