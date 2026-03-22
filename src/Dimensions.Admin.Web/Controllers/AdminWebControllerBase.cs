using Dimensions.Admin.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Web.Controllers;

public abstract class AdminWebControllerBase : Controller
{
    protected IActionResult? HandleAdminApiAuthFailure<T>(IAdminSessionAccessor sessionAccessor, Models.AdminApiResult<T> result)
    {
        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            sessionAccessor.Clear();
            return RedirectToAction("Login", "Account", new { reason = "expired" });
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return RedirectToAction("Forbidden", "System", new
            {
                message = result.ErrorMessage,
                errorCode = result.ErrorCode,
                caseId = result.CaseId
            });
        }

        return null;
    }
}
