using Dimensions.Admin.Web.Filters;
using Dimensions.Admin.Web.Models.Logs;
using Dimensions.Admin.Web.Services;
using Dimensions.Contracts.Log;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Web.Controllers;

[RequireAdminSession]
public sealed class LogsController(IAdminApiClient adminApiClient, IAdminSessionAccessor adminSessionAccessor) : AdminWebControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Requests([FromQuery] ApiRequestLogListRequest filter, CancellationToken cancellationToken)
    {
        var result = await adminApiClient.GetRequestLogsAsync(filter, cancellationToken);
        var authFailure = HandleAdminApiAuthFailure(adminSessionAccessor, result);
        if (authFailure is not null)
        {
            return authFailure;
        }

        ViewData["Title"] = "Request Log";
        ViewData["ActiveNav"] = "RequestLogs";

        return View(new RequestLogsPageModel
        {
            Filter = filter,
            Result = result.Data,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage,
            CaseId = result.CaseId
        });
    }

    [HttpGet]
    public async Task<IActionResult> Exceptions([FromQuery] ApiExceptionLogListRequest filter, CancellationToken cancellationToken)
    {
        var result = await adminApiClient.GetExceptionLogsAsync(filter, cancellationToken);
        var authFailure = HandleAdminApiAuthFailure(adminSessionAccessor, result);
        if (authFailure is not null)
        {
            return authFailure;
        }

        ViewData["Title"] = "Exception Log";
        ViewData["ActiveNav"] = "ExceptionLogs";

        return View(new ExceptionLogsPageModel
        {
            Filter = filter,
            Result = result.Data,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage,
            CaseId = result.CaseId
        });
    }
}
