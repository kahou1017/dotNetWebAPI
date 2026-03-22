using Dimensions.Admin.Web.Models;
using Dimensions.Admin.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dimensions.Admin.Web.Filters;

public sealed class RequireAdminSessionAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var adminSessionAccessor = context.HttpContext.RequestServices.GetRequiredService<IAdminSessionAccessor>();
        var session = adminSessionAccessor.GetCurrent();
        if (session is null || string.IsNullOrWhiteSpace(session.AccessToken))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var adminApiClient = context.HttpContext.RequestServices.GetRequiredService<IAdminApiClient>();
        var currentAdmin = await adminApiClient.GetCurrentAdminAsync(context.HttpContext.RequestAborted);
        if (!currentAdmin.IsSuccess || currentAdmin.Data is null)
        {
            adminSessionAccessor.Clear();
            context.Result = new RedirectToActionResult("Login", "Account", new { reason = "expired" });
            return;
        }

        adminSessionAccessor.SetCurrent(session with
        {
            UserId = currentAdmin.Data.UserId,
            LoginAccount = currentAdmin.Data.LoginAccount,
            DisplayName = currentAdmin.Data.DisplayName
        });

        context.HttpContext.Items["CurrentAdmin"] = currentAdmin.Data;
        await next();
    }
}
