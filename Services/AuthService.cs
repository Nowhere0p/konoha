using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DnsClient.Protocol;
using Konoha.common;
using Konoha.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace Konoha.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<UserDetails> _usersDb;
        private readonly IConfiguration _configuration;

        public AuthService(IMongoDatabase database, IConfiguration configuration)
        {
            _usersDb = database.GetCollection<UserDetails>("users");
            _configuration = configuration;
        }

        public async Task<string?> Login(Login request)
        {
            var user = await _usersDb.Find(x => x.Email == request.Email).FirstOrDefaultAsync();
            if (user == null || !VerifyPassword(request.Password, user.Password ?? ""))
                return null;

            return GenerateJwtToken(user);
        }

        public async Task<bool> Register(RegisterInteraction registerData)
        {
            if (await _usersDb.Find(x => x.Email == registerData.Email).AnyAsync())
                return false;

            var userDetails = new UserDetails
            {
                UserName = registerData.UserName,
                PublicUsername = registerData.PublicUsername,
                FirstName = registerData.FirstName,
                LastName = registerData.LastName,
                Email = registerData.Email,
                Password = HashPassword(registerData.Password ?? ""),
                CreatedAt = DateTime.UtcNow,
                Gender = registerData.Gender,
                UserId = Guid.NewGuid().ToString(),
                Role = Role.USER,
            };

            await _usersDb.InsertOneAsync(userDetails);
            return true;
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return HashPassword(inputPassword) == storedPassword;
        }

        private string GenerateJwtToken(UserDetails user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                        new Claim(CustomClaimTypes.UserId, user.UserId),
                        new Claim(ClaimTypes.GivenName, user.FirstName),
                        new Claim(ClaimTypes.Surname, user.LastName),
                        new Claim(ClaimTypes.Role, Role.USER.ToString()),
                    ]
                ),
                Expires = DateTime.UtcNow.AddHours(48),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
