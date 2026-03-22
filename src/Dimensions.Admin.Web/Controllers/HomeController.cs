using Dimensions.Admin.Web.Filters;
using Dimensions.Admin.Web.Models.Dashboard;
using Dimensions.Admin.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Web.Controllers;

[RequireAdminSession]
public sealed class HomeController(IAdminSessionAccessor adminSessionAccessor) : Controller
{
    public IActionResult Index()
    {
        var session = adminSessionAccessor.GetCurrent();
        var model = new DashboardViewModel
        {
            DisplayName = session?.DisplayName ?? string.Empty,
            UserId = session?.UserId ?? string.Empty,
            LoginAccount = session?.LoginAccount
        };

        ViewData["Title"] = "管理後台首頁";
        ViewData["ActiveNav"] = "Home";
        return View(model);
    }
}
