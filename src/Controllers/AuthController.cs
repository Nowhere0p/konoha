using System.Net.Http.Headers;
using System.Security.Claims;
using Konoha.common;
using Konoha.Common;
using Konoha.Models;
using Konoha.Services;
using Konoha.Services.EmailHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace Konoha.Controllers
{
    [ApiController]
    [Route("api/v1.0")]
    public class AuthController(
        IAuthService authService,
        IEmailService emailService,
        ILogger<AuthController> logger
    ) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<AuthController> _logger = logger;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var result = await _authService.Login(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInteraction request)
        {
            try
            {
                await _authService.Register(request);
                return Ok();
            }
            catch (KonohaException)
            {
                throw;
            }
            catch (Exception er)
            {
                _logger.LogError("Failed to Register");
                throw new KonohaException(
                    KonohaException.InternalServerError,
                    "Internal Server Error"
                );
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "USER")]
        [HttpGet("welcome")]
        public async Task<IActionResult> Hello()
        {
            // var email=new EmailModel{
            //     ToEmails=new List<string>{
            //         "bt23cse013@nituk.ac.in",
            //         "jzgupta.sahil04@gmail.com",
            //         "bt23ece009@nituk.ac.in",
            //     },
            //     Body="TESTING",
            //     Subject="OTP Verification",
            // };
            // await _emailService.SendEmailAsync(email);
            return Ok($"WELOME TO KONOHA");
        }
    }
}
