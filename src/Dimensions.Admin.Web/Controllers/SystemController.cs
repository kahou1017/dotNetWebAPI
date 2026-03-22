using System.Diagnostics;
using Dimensions.Admin.Web.Models;
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
}
