using MongoDB.Driver;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Konoha.Models;

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
                Gender = registerData.Gender
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-default-key-here"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", user.Id),
                new Claim("Name", user.FirstName+" "+user.LastName ?? ""),
                new Claim("Role", user.Role.ToString()),
                new Claim("CreateTime", user.CreatedAt.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}