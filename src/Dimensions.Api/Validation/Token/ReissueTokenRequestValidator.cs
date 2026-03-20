using Dimensions.Contracts.Token;
using FluentValidation;

namespace Dimensions.Api.Validation.Token;

public sealed class ReissueTokenRequestValidator : AbstractValidator<ReissueTokenRequest>
{
    public ReissueTokenRequestValidator()
    {
        RuleFor(x => x.TokenId)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.EffectiveAt)
            .NotEmpty();

        RuleFor(x => x.ExpireAt)
            .GreaterThan(x => x.EffectiveAt)
            .When(x => x.ExpireAt.HasValue)
            .WithMessage("ExpireAt must be later than EffectiveAt.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(200);
    }
}
