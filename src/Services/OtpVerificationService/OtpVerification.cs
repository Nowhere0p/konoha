using System;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using Konoha.Common;
using Konoha.Models;
using Konoha.Services.EmailHelper;

namespace Konoha.Services.OtpVerificationService;

public class OtpVerificationService(
    IMongoDbService<UserDetails> usersDb,
    IMongoDbService<OtpVerification> otpDb,
    ILogger<OtpVerification> logger,
    IEmailService emailServices
) : IOtpVerificationService
{
    private readonly ILogger<OtpVerification> _logger = logger;
    private readonly IEmailService _emailService = emailServices;
    private readonly IMongoDbService<OtpVerification> _otpDb = otpDb;
    private readonly IMongoDbService<UserDetails> _usersDb = usersDb;

    public async Task SendOtpAsync(string email)
    {
        var code = GenerateOtpCode();
        var otp = new OtpVerification { Email = email, verificationCode = code.ToString() };
        await _otpDb.SaveAsync(otp);
        var mail = new EmailModel
        {
            ToEmails = new List<string> { email },
            Subject = "OTP Verification",
            Body = $"Your OTP for Registration  is {code}. The Code is Valid for next 5 mins.",
        };
        await _emailService.SendEmailAsync(mail);
    }

public async Task VerifyOtpAsync(string code, string email)
{
    try
    {
        // Validate OTP
        var otpRecord = await ValidateOtpAsync(code, email);

        // Fetch user
        var user = (await _usersDb.GetItemsAsync(x => x.Email == email && !x.IsVerified)).FirstOrDefault();
        if (user == null)
            throw new KonohaException(KonohaException.BadRequest, "User Not Found");

        await UpdateRecordsAsync(otpRecord, user);
    }
    catch (KonohaException ex)
    {
        _logger.LogError(ex, $"Error during OTP verification for email: {email}");
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "error during OTP verification for email: {Email}", email);
        throw new KonohaException(KonohaException.InternalServerError, $"Error: {ex.Message}");
    }
}

private async Task<OtpVerification> ValidateOtpAsync(string code, string email)
{
    var otp = (await _otpDb.GetItemsAsync(x => x.Email == email && x.verificationCode == code)).FirstOrDefault();
    if (otp == null)
        throw new KonohaException(KonohaException.BadRequest, "Invalid OTP");

    if (otp.ExpiresAt < DateTime.UtcNow)
    {
        otp.IsValid = false;
        await _otpDb.UpdateAsync(otp.Id, otp);
        throw new KonohaException(KonohaException.BadRequest, "OTP Expired");
    }
    return otp;
}

private async Task UpdateRecordsAsync(OtpVerification otpRecord, UserDetails user)
{
    otpRecord.IsValid = false;
    user.IsVerified = true;

    await _otpDb.UpdateAsync(otpRecord.Id, otpRecord);
    await _usersDb.UpdateAsync(user.Id, user);
}

private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

private async Task<int> GenerateOtpCode()
{
    byte[] buffer = new byte[4];
    _rng.GetBytes(buffer);
    int code = BitConverter.ToInt32(buffer, 0) % 900000 + 100000;
    return Math.Abs(code);
}

}
