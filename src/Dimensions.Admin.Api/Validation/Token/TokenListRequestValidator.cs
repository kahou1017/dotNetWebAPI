using Dimensions.Admin.Api.Validation.Shared;
using Dimensions.Contracts.Token;
using Dimensions.Domain.Enums;
using FluentValidation;

namespace Dimensions.Admin.Api.Validation.Token;

public sealed class TokenListRequestValidator : AbstractValidator<TokenListRequest>
{
    private static readonly HashSet<string> SupportedTokenTypes =
    [
        TokenType.AdminSession,
        TokenType.UserAccess,
        TokenType.Integration,
        TokenType.Service
    ];

    private static readonly HashSet<string> SupportedStatuses =
    [
        TokenStatus.Active,
        TokenStatus.Revoked,
        TokenStatus.Expired,
        TokenStatus.Reissued,
        TokenStatus.Disabled
    ];

    public TokenListRequestValidator()
    {
        this.ApplyPagingRules(x => x.PageNo, x => x.PageSize);
        this.ApplyDateRangeRule(x => x.IssuedAtStart, x => x.IssuedAtEnd, nameof(TokenListRequest.IssuedAtStart), nameof(TokenListRequest.IssuedAtEnd));
        this.ApplyDateRangeRule(x => x.ExpireAtStart, x => x.ExpireAtEnd, nameof(TokenListRequest.ExpireAtStart), nameof(TokenListRequest.ExpireAtEnd));

        RuleFor(x => x.TokenType)
            .Must(tokenType => tokenType is null || SupportedTokenTypes.Contains(tokenType))
            .WithMessage("TokenType must be AdminSession, UserAccess, Integration, or Service.");

        RuleFor(x => x.Status)
            .Must(status => status is null || SupportedStatuses.Contains(status))
            .WithMessage("Status must be Active, Revoked, Expired, Reissued, or Disabled.");

        RuleFor(x => x.UserId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.UserId));

        RuleFor(x => x.TokenId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.TokenId));

        RuleFor(x => x.DeviceId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceId));
    }
}

