using Dimensions.Contracts.Device;
using FluentValidation;

namespace Dimensions.Admin.Api.Validation.Device;

public sealed class DisableDeviceRequestValidator : AbstractValidator<DisableDeviceRequest>
{
    public DisableDeviceRequestValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(200);
    }
}

