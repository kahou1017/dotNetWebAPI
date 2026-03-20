using System.Security.Claims;
using Dimensions.Application.Interfaces;
using Dimensions.Domain.Constants;

namespace Dimensions.Admin.Api.Services;

internal sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public string? GetUserId() => GetClaimValue(ClaimNames.Subject);

    public string? GetDisplayName() => GetClaimValue(ClaimNames.Name);

    public string? GetRole() => GetClaimValue(ClaimNames.Role);

    public string? GetScope() => GetClaimValue(ClaimNames.Scope);

    public string? GetTokenType() => GetClaimValue(ClaimNames.TokenType);

    public string? GetTokenId() => GetClaimValue(ClaimNames.TokenId);

    public string? GetJwtId() => GetClaimValue(ClaimNames.JwtId);

    private string? GetClaimValue(string claimType)
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.FindFirstValue(claimType);
    }
}

