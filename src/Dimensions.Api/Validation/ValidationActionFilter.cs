using Dimensions.Api.Responses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dimensions.Api.Validation;

public sealed class ValidationActionFilter(
    IServiceProvider serviceProvider,
    IConfiguration configuration) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var validationErrors = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        foreach (var argument in context.ActionArguments.Values.Where(static value => value is not null))
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
            if (serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            ValidationResult result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
            if (result.IsValid)
            {
                continue;
            }

            foreach (var errorGroup in result.Errors
                         .GroupBy(error => error.PropertyName)
                         .Select(group => new
                         {
                             PropertyName = group.Key,
                             Errors = group.Select(error => error.ErrorMessage).Distinct().ToList()
                         }))
            {
                if (!validationErrors.TryGetValue(errorGroup.PropertyName, out var messages))
                {
                    messages = [];
                    validationErrors[errorGroup.PropertyName] = messages;
                }

                messages.AddRange(errorGroup.Errors.Where(message => !messages.Contains(message, StringComparer.Ordinal)));
            }
        }

        if (validationErrors.Count == 0)
        {
            await next();
            return;
        }

        var systemCode = configuration["System:SystemCode"] ?? "Dimensions";
        var payload = ApiResponseFactory.Error(
            context.HttpContext,
            systemCode,
            "Validation.InvalidRequest",
            "Request validation failed.",
            validationErrors.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray(), StringComparer.Ordinal));

        context.Result = new BadRequestObjectResult(payload);
    }
}
