using Dimensions.Contracts.Device;
using FluentValidation;

namespace Dimensions.Api.Validation.Device;

public sealed class CreateDeviceRequestValidator : AbstractValidator<CreateDeviceRequest>
{
    public CreateDeviceRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.DeviceId)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DeviceName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DeviceType)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Remark)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Remark));
    }
}
