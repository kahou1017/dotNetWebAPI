using Dimensions.Admin.Api.Policies;
using Dimensions.Admin.Api.Responses;
using Dimensions.Application.Interfaces;
using Dimensions.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Api.Controllers;

[ApiController]
[Route("admin-api/auth")]
public sealed class AuthController(IAuthService authService, IConfiguration configuration) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var login = await authService.LoginAsync(request, cancellationToken);
        if (login is null)
        {
            var failed = ApiResponseFactory.Error(HttpContext, GetSystemCode(), "Auth.LoginFailed", "Login account or password is invalid.");
            return Unauthorized(failed);
        }

        return Ok(ApiResponseFactory.Success(HttpContext, login, GetSystemCode()));
    }

    [Authorize(Policy = PolicyNames.AdminOnly)]
    [HttpGet("me")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var current = await authService.GetCurrentAdminAsync(cancellationToken);
        return Ok(ApiResponseFactory.Success(HttpContext, current, GetSystemCode()));
    }

    private string GetSystemCode() => configuration["System:SystemCode"] ?? "Dimensions";
}

