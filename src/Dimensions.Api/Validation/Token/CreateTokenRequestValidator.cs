using Dimensions.Contracts.Token;
using Dimensions.Domain.Enums;
using FluentValidation;

namespace Dimensions.Api.Validation.Token;

public sealed class CreateTokenRequestValidator : AbstractValidator<CreateTokenRequest>
{
    private static readonly HashSet<string> SupportedTokenTypes =
    [
        TokenType.UserAccess,
        TokenType.Integration,
        TokenType.Service
    ];

    public CreateTokenRequestValidator()
    {
        RuleFor(x => x.TokenType)
            .NotEmpty()
            .Must(tokenType => SupportedTokenTypes.Contains(tokenType))
            .WithMessage("TokenType must be UserAccess, Integration, or Service.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.TokenName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.EffectiveAt)
            .NotEmpty();

        RuleFor(x => x.ExpireAt)
            .NotNull()
            .When(x => !x.IsPermanent)
            .WithMessage("ExpireAt is required when IsPermanent is false.");

        RuleFor(x => x)
            .Must(x => x.IsPermanent || x.ExpireAt > x.EffectiveAt)
            .WithMessage("ExpireAt must be later than EffectiveAt.");

        RuleFor(x => x.DeviceId)
            .NotEmpty()
            .When(x => x.IsSingleDevice)
            .WithMessage("DeviceId is required when IsSingleDevice is true.");

        RuleFor(x => x.DeviceName)
            .NotEmpty()
            .When(x => x.IsSingleDevice)
            .WithMessage("DeviceName is required when IsSingleDevice is true.");

        RuleFor(x => x.Purpose)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Purpose));

        RuleFor(x => x.Remark)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Remark));
    }
}
