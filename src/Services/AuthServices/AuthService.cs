using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Konoha.common;
using Konoha.Common;
using Konoha.Extensions;
using Konoha.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace Konoha.Services;

public class AuthService(
    IMongoDbService<UserDetails> usersDb,
    IConfiguration configuration,
    ILogger<AuthService> logger,
    IOtpVerificationService verificationService
) : IAuthService
{
    private readonly IMongoDbService<UserDetails> _usersDb = usersDb;
    private readonly ILogger<AuthService> _logger = logger;

    private readonly IConfiguration _configuration = configuration;
    private readonly IOtpVerificationService _verificationService = verificationService;

    public async Task<string?> Login(Login request)
    {
        try
        {
            var user = (
                await _usersDb.GetItemsAsync(x => x.Email == request.Email)
            ).FirstOrDefault();
            if (user == null || !VerifyPassword(request.Password, user.Password ?? ""))
            {
                _logger.LogError("Failed to Login");
                throw new KonohaException(KonohaException.BadRequest, "Failed To Login");
            }

            return GenerateJwtToken(user);
        }
        catch (KonohaException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to provide token");
            throw new KonohaException(KonohaException.InternalServerError, "Internal Server error");
        }
    }

    public async Task Register(RegisterInteraction registerData)
    {
        try
        {
            if (
                (
                    await _usersDb.GetItemsAsync(x =>
                        x.Email == registerData.Email && x.IsVerified == true
                    )
                ).Any()
            )
                throw new KonohaException(KonohaException.Forbidden, "Email already Exists");
            var userDetails = registerData.ToUserDetails();

            await _usersDb.SaveAsync(userDetails);
            await _verificationService.SendOtpAsync(userDetails.Email);
        }
        catch (KonohaException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("INTERNAL SERVER ERROR");
            throw new KonohaException(
                KonohaException.InternalServerError,
                "Error :INTERNAL SERVER ERROR"
            );
        }
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

    private string GenerateOtpCode()
    {
        var random = new Random();
        var otpCode = random.Next(100000, 999999);
        return otpCode.ToString();
    }
}
