using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Auth;
using Dimensions.Domain.Enums;

namespace Dimensions.Application.Services;

public sealed class AuthService(
    IAdminRepository adminRepository,
    ITokenRepository tokenRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    ICurrentUserAccessor currentUserAccessor) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.LoginAccount) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var admin = await adminRepository.GetByLoginAccountAsync(request.LoginAccount, cancellationToken);
        if (admin is null || !string.Equals(admin.Password, request.Password, StringComparison.Ordinal))
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow;
        var tokenId = $"ADM{now:yyyyMMddHHmmss}";
        var jwtId = Guid.NewGuid().ToString("N");
        var expiresAt = now.AddDays(30);
        var token = jwtTokenGenerator.GenerateToken(new JwtTokenRequest
        {
            UserId = admin.UserId,
            DisplayName = admin.DisplayName,
            Role = "Admin",
            Scope = "token.manage",
            TokenType = TokenType.AdminSession,
            TokenId = tokenId,
            JwtId = jwtId,
            IssuedAt = now,
            EffectiveAt = now,
            ExpiresAt = expiresAt
        });

        await adminRepository.UpdateLastLoginAtAsync(admin.UserId, now, cancellationToken);
        await tokenRepository.InsertTokenAsync(
            new TokenWriteModel
            {
                TokenId = tokenId,
                JwtId = jwtId,
                TokenType = TokenType.AdminSession,
                UserId = admin.UserId,
                UserName = admin.DisplayName,
                TokenName = "Admin Session",
                Status = TokenStatus.Active,
                IsRevoked = false,
                IsSingleDevice = false,
                IsEnabled = true,
                CanReissue = false,
                CanRenew = false,
                DeviceId = null,
                DeviceName = null,
                AccessToken = token.AccessToken,
                IssuedAt = token.IssuedAt,
                EffectiveAt = token.EffectiveAt,
                ExpireAt = token.ExpiresAt,
                LastUsedAt = null,
                Purpose = "AdminLogin",
                Remark = "Admin session login",
                CreatedBy = admin.UserId,
                CreatedAt = now,
                RevokedAt = null
            },
            cancellationToken);

        return new LoginResponse
        {
            TokenType = TokenType.AdminSession,
            TokenId = tokenId,
            JwtId = jwtId,
            AccessToken = token.AccessToken,
            IssuedAt = token.IssuedAt,
            EffectiveAt = token.EffectiveAt,
            ExpireAt = token.ExpiresAt,
            UserId = admin.UserId,
            DisplayName = admin.DisplayName
        };
    }

    public async Task<CurrentAdminResponse> GetCurrentAdminAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetUserId() ?? "ADMIN001";
        var admin = await adminRepository.GetByUserIdAsync(currentUserId, cancellationToken);

        return new CurrentAdminResponse
        {
            UserId = admin?.UserId ?? currentUserId,
            LoginAccount = admin?.LoginAccount ?? "admin",
            DisplayName = admin?.DisplayName ?? currentUserAccessor.GetDisplayName() ?? "System Admin",
            LastLoginAt = admin?.LastLoginAt ?? DateTimeOffset.UtcNow
        };
    }
}
