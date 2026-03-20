using Dimensions.Api.Validation.Shared;
using Dimensions.Contracts.Token;
using FluentValidation;

namespace Dimensions.Api.Validation.Token;

public sealed class TokenUsageRequestValidator : AbstractValidator<TokenUsageRequest>
{
    public TokenUsageRequestValidator()
    {
        this.ApplyPagingRules(x => x.PageNo, x => x.PageSize);
        this.ApplyDateRangeRule(x => x.RequestTimeStart, x => x.RequestTimeEnd, nameof(TokenUsageRequest.RequestTimeStart), nameof(TokenUsageRequest.RequestTimeEnd));

        RuleFor(x => x.TokenId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.TokenId));

        RuleFor(x => x.UserId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.UserId));

        RuleFor(x => x.DeviceId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceId));

        RuleFor(x => x.ClientIp)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.ClientIp));
    }
}
