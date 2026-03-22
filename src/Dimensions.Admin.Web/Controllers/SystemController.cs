using System.Diagnostics;
using Dimensions.Admin.Web.Models;
using Dimensions.Admin.Web.Models.System;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Web.Controllers;

public sealed class SystemController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("system/error")]
    public IActionResult Error()
        => View("~/Views/Shared/Error.cshtml", new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });

    [HttpGet("system/forbidden")]
    public IActionResult Forbidden(string? message = null, string? errorCode = null, string? caseId = null)
    {
        ViewData["Title"] = "權限不足";
        return View(new ForbiddenPageModel
        {
            Message = string.IsNullOrWhiteSpace(message) ? "你目前沒有權限執行這個操作。" : message,
            ErrorCode = errorCode,
            CaseId = caseId
        });
    }
}
