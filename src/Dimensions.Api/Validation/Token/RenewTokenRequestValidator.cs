using Dimensions.Contracts.Token;
using FluentValidation;

namespace Dimensions.Api.Validation.Token;

public sealed class RenewTokenRequestValidator : AbstractValidator<RenewTokenRequest>
{
    public RenewTokenRequestValidator()
    {
        RuleFor(x => x.TokenId)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.NewExpireAt)
            .NotNull()
            .WithMessage("NewExpireAt is required.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(200);
    }
}
