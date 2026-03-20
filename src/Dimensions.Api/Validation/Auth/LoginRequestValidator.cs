using Dimensions.Contracts.Auth;
using FluentValidation;

namespace Dimensions.Api.Validation.Auth;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.LoginAccount)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(100);
    }
}
