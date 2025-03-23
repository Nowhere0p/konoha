using Konoha.common;
using Konoha.Common;
using Konoha.Extensions;
using Konoha.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Konoha.Controllers;

[ApiController]
[Authorize]
[Route("api/v1.0")]
public class UserDetailsController(IUserClient userClient) : ControllerBase
{
    private readonly IUserClient _userClient = userClient;

    [Authorize(AuthenticationSchemes = "Bearer", Roles = "USER")]
    [HttpGet("user")]
    public async Task<IActionResult> GetUserDetails()
    {
        try
        {
            var userId = User.FindFirst(CustomClaimTypes.UserId)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new KonohaException(KonohaException.InternalServerError, "Internal Error");
            }
            var user = (await _userClient.GetUserByIdAsync(userId)).RedactPassword();
            return Ok(user);
        }
        catch (KonohaException ex)
        {
            throw new KonohaException(KonohaException.InternalServerError, ex.Message);
        }
    }
}
