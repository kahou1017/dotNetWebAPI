using Dimensions.Contracts.Token;
using FluentValidation;

namespace Dimensions.Admin.Api.Validation.Token;

public sealed class TokenDetailRequestValidator : AbstractValidator<TokenDetailRequest>
{
    public TokenDetailRequestValidator()
    {
        RuleFor(x => x.TokenId)
            .NotEmpty()
            .MaximumLength(50);
    }
}

