using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DnsClient.Protocol;
using Konoha.common;
using Konoha.Extensions;
using Konoha.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace Konoha.Services
{
    public class AuthService(IMongoDbService<UserDetails> usersDb, IConfiguration configuration)
        : IAuthService
    {
        private readonly IMongoDbService<UserDetails> _usersDb = usersDb;
        private readonly IConfiguration _configuration = configuration;

        public async Task<string?> Login(Login request)
        {
            var user = (
                await _usersDb.GetItemsAsync(x => x.Email == request.Email)
            ).FirstOrDefault();
            if (user == null || !VerifyPassword(request.Password, user.Password ?? ""))
                return null;

            return GenerateJwtToken(user);
        }

        public async Task<bool> Register(RegisterInteraction registerData)
        {
            if ((await _usersDb.GetItemsAsync(x => x.Email == registerData.Email)).Any())
                return false;

            var userDetails = registerData.ToUserDetails();

            await _usersDb.SaveAsync(userDetails);
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
