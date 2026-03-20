using Dimensions.Admin.Api.Policies;
using Dimensions.Admin.Api.Responses;
using Dimensions.Application.Interfaces;
using Dimensions.Contracts.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Api.Controllers;

[ApiController]
[Authorize(Policy = PolicyNames.TokenManage)]
[Route("admin-api/token")]
public sealed class TokenController(ITokenService tokenService, IConfiguration configuration) : ControllerBase
{
    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] TokenListRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await tokenService.GetTokenListAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("detail")]
    public async Task<IActionResult> Detail([FromBody] TokenDetailRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await tokenService.GetTokenDetailAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTokenRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await tokenService.CreateTokenAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await tokenService.RevokeTokenAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("reissue")]
    public async Task<IActionResult> Reissue([FromBody] ReissueTokenRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await tokenService.ReissueTokenAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("renew")]
    public async Task<IActionResult> Renew([FromBody] RenewTokenRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await tokenService.RenewTokenAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("usage")]
    public async Task<IActionResult> Usage([FromBody] TokenUsageRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await tokenService.GetTokenUsageAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("action-log")]
    public async Task<IActionResult> ActionLog([FromBody] TokenActionLogRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await tokenService.GetTokenActionLogAsync(request, cancellationToken), GetSystemCode()));

    private string GetSystemCode() => configuration["System:SystemCode"] ?? "Dimensions";
}

