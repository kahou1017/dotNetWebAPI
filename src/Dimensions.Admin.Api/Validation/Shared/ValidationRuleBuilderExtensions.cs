using System.Linq.Expressions;
using FluentValidation;

namespace Dimensions.Admin.Api.Validation.Shared;

public static class ValidationRuleBuilderExtensions
{
    public static void ApplyPagingRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, int>> pageNoExpression,
        Expression<Func<T, int>> pageSizeExpression)
    {
        validator.RuleFor(pageNoExpression)
            .GreaterThan(0)
            .WithMessage("PageNo must be greater than 0.");

        validator.RuleFor(pageSizeExpression)
            .InclusiveBetween(1, 200)
            .WithMessage("PageSize must be between 1 and 200.");
    }

    public static void ApplyDateRangeRule<T>(
        this AbstractValidator<T> validator,
        Func<T, DateTimeOffset?> startSelector,
        Func<T, DateTimeOffset?> endSelector,
        string startName,
        string endName)
    {
        validator.RuleFor(x => x)
            .Must(instance =>
            {
                var start = startSelector(instance);
                var end = endSelector(instance);
                return !start.HasValue || !end.HasValue || start.Value <= end.Value;
            })
            .WithMessage($"{endName} must be later than or equal to {startName}.");
    }
}

