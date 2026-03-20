using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dimensions.Admin.Api.Options;
using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Domain.Constants;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dimensions.Admin.Api.Services;

internal sealed class JwtTokenGenerator(IOptions<JwtOptions> options) : IJwtTokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    public JwtTokenResult GenerateToken(JwtTokenRequest request)
    {
        var issuedAt = request.IssuedAt == default ? DateTimeOffset.UtcNow : request.IssuedAt;
        var effectiveAt = request.EffectiveAt == default ? issuedAt : request.EffectiveAt;
        var expiresAt = request.ExpiresAt;

        if (expiresAt is null && !_options.AllowPermanentToken)
        {
            expiresAt = effectiveAt.AddDays(_options.DefaultExpireDays);
        }

        var claims = new List<Claim>
        {
            new(ClaimNames.Subject, request.UserId),
            new(ClaimNames.Name, request.DisplayName),
            new(ClaimNames.Role, request.Role),
            new(ClaimNames.Scope, request.Scope),
            new(ClaimNames.TokenType, request.TokenType),
            new(ClaimNames.TokenId, request.TokenId),
            new(ClaimNames.JwtId, request.JwtId)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: effectiveAt.UtcDateTime,
            expires: expiresAt?.UtcDateTime,
            signingCredentials: credentials);

        return new JwtTokenResult
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            IssuedAt = issuedAt,
            EffectiveAt = effectiveAt,
            ExpiresAt = expiresAt
        };
    }
}

