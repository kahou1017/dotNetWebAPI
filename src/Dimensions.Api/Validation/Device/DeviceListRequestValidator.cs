using Dimensions.Api.Validation.Shared;
using Dimensions.Contracts.Device;
using FluentValidation;

namespace Dimensions.Api.Validation.Device;

public sealed class DeviceListRequestValidator : AbstractValidator<DeviceListRequest>
{
    public DeviceListRequestValidator()
    {
        this.ApplyPagingRules(x => x.PageNo, x => x.PageSize);

        RuleFor(x => x.UserId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.UserId));

        RuleFor(x => x.DeviceId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceId));
    }
}
