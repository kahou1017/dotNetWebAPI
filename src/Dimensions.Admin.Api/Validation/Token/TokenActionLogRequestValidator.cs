using Dimensions.Admin.Api.Validation.Shared;
using Dimensions.Contracts.Token;
using FluentValidation;

namespace Dimensions.Admin.Api.Validation.Token;

public sealed class TokenActionLogRequestValidator : AbstractValidator<TokenActionLogRequest>
{
    private static readonly HashSet<string> SupportedActionTypes =
    [
        "CreateToken",
        "RevokeToken",
        "ReissueToken",
        "RenewToken"
    ];

    public TokenActionLogRequestValidator()
    {
        this.ApplyPagingRules(x => x.PageNo, x => x.PageSize);
        this.ApplyDateRangeRule(x => x.CreatedAtStart, x => x.CreatedAtEnd, nameof(TokenActionLogRequest.CreatedAtStart), nameof(TokenActionLogRequest.CreatedAtEnd));

        RuleFor(x => x.TokenId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.TokenId));

        RuleFor(x => x.ActionType)
            .Must(actionType => actionType is null || SupportedActionTypes.Contains(actionType))
            .WithMessage("ActionType must be CreateToken, RevokeToken, ReissueToken, or RenewToken.");

        RuleFor(x => x.OperatorUserId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.OperatorUserId));
    }
}

