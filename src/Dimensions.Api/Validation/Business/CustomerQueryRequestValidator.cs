using Dimensions.Contracts.Business;
using FluentValidation;

namespace Dimensions.Api.Validation.Business;

public sealed class CustomerQueryRequestValidator : AbstractValidator<CustomerQueryRequest>
{
    public CustomerQueryRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Keyword)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Keyword));
    }
}
