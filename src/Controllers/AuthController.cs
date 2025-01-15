using System.Security.Claims;
using Konoha.common;
using Konoha.Models;
using Konoha.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Konoha.Controllers
{
    [ApiController]
    [Route("api/v1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

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
            var result = await _authService.Register(request);
            if (!result)
                return BadRequest(new { message = "Registration failed" });

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "USER")]
        [HttpGet("welcome")]
        public async Task<IActionResult> Hello()
        {
            return Ok($"WELOME TO KONOHA");
        }
    }
}
