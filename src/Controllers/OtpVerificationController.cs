using System.Threading.Tasks;
using Konoha.Common;
using Konoha.Models;
using Konoha.Services;
using Microsoft.AspNetCore.Mvc;

namespace Konoha.Controllers;

[ApiController]
[Route("api/v1.0")]
public class OtpVerificationController(
    IOtpVerificationService otpVerificationService,
    ILogger<OtpVerificationController> logger
) : ControllerBase
{
    private readonly IOtpVerificationService _otpVerificationService = otpVerificationService;
    private readonly ILogger<OtpVerificationController> _logger = logger;

    [HttpPost]
    [Route("user/verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequest request)
    {
        try
        {
            _logger.LogInformation($"Verifying OTP for {request.Email}");
            await _otpVerificationService.VerifyOtpAsync(request.Code, request.Email);
            _logger.LogInformation($"User Verified Successfully : {request.Email}");
            return Ok("User Verified Successfully");
        }
        catch (KonohaException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to Verify User : {request.Email}");
            throw new KonohaException(
                KonohaException.InternalServerError,
                $"Failed to verify user: {e.Message} "
            );
        }
    }
}
