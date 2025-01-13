using Microsoft.AspNetCore.Mvc;
using Konoha.Models;
using Konoha.Services;

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
        public async Task<ActionResult<AuthResponse>> Login([FromBody] Login request)
        {
            var result = await _authService.Login(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterInteraction request)
        {
            var result = await _authService.Register(request);
            if (!result)
                return BadRequest(new { message = "Registration failed" });

            return Ok(result);
        }
    }
}