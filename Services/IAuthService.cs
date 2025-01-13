using Konoha.Models;

namespace Konoha.Services
{
    public interface IAuthService
    {
        Task<string?> Login(Login request);
        Task<bool> Register(RegisterInteraction user);
    }
}