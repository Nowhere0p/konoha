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

            return Ok(new AuthResponse{Token=result});
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "USER")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            

            return Ok( new LogoutResponse{Message="logged out"});
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
                _logger.LogError($"Failed to Register: {er.Message}");
                throw new KonohaException(
                    KonohaException.InternalServerError,
                    "Internal Server Error"
                );
            }
        }

        // [Authorize(AuthenticationSchemes = "Bearer", Roles = "USER")]
        [AllowAnonymous]
        [HttpGet("welcome")]
        public async Task<IActionResult> Hello()
        {
            return Ok($"WELOME TO KONOHA");
        }
    }
}
