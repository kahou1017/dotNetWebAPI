using Dimensions.Contracts.Token;
using FluentValidation;

namespace Dimensions.Api.Validation.Token;

public sealed class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
{
    public RevokeTokenRequestValidator()
    {
        RuleFor(x => x.TokenId)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(200);
    }
}
