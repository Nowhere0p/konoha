using Konoha.Models;

namespace Konoha.Services;

public interface IOtpVerificationService
{
    public Task SendOtpAsync(string email);
    public Task VerifyOtpAsync(string code, string email);
}
