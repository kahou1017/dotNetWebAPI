using Dimensions.Admin.Api.Validation.Shared;
using Dimensions.Contracts.Log;
using FluentValidation;

namespace Dimensions.Admin.Api.Validation.Log;

public sealed class ApiRequestLogListRequestValidator : AbstractValidator<ApiRequestLogListRequest>
{
    public ApiRequestLogListRequestValidator()
    {
        this.ApplyPagingRules(x => x.PageNo, x => x.PageSize);
        this.ApplyDateRangeRule(x => x.RequestTimeStart, x => x.RequestTimeEnd, nameof(ApiRequestLogListRequest.RequestTimeStart), nameof(ApiRequestLogListRequest.RequestTimeEnd));

        RuleFor(x => x.CaseId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.CaseId));

        RuleFor(x => x.Path)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Path));

        RuleFor(x => x.UserId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.UserId));

        RuleFor(x => x.TokenId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.TokenId));

        RuleFor(x => x.DeviceId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceId));

        RuleFor(x => x.ClientIp)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.ClientIp));
    }
}
