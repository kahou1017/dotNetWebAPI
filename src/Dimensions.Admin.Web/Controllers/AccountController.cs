using Dimensions.Admin.Web.Models;
using Dimensions.Admin.Web.Models.Account;
using Dimensions.Admin.Web.Services;
using Dimensions.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Web.Controllers;

public sealed class AccountController(IAdminApiClient adminApiClient, IAdminSessionAccessor adminSessionAccessor) : Controller
{
    [HttpGet]
    public IActionResult Login(string? reason = null)
    {
        if (adminSessionAccessor.GetCurrent() is not null)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new LoginPageModel();
        if (string.Equals(reason, "expired", StringComparison.OrdinalIgnoreCase))
        {
            model.ErrorMessage = "管理員工作階段已過期，請重新登入。";
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginPageModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await adminApiClient.LoginAsync(
            new LoginRequest
            {
                LoginAccount = model.LoginAccount,
                Password = model.Password
            },
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            model.ErrorCode = result.ErrorCode;
            model.ErrorMessage = result.ErrorMessage ?? "登入失敗，請確認帳號與密碼是否正確。";
            model.CaseId = result.CaseId;
            return View(model);
        }

        adminSessionAccessor.SetCurrent(new AdminSessionState
        {
            TokenType = result.Data.TokenType,
            TokenId = result.Data.TokenId,
            JwtId = result.Data.JwtId,
            AccessToken = result.Data.AccessToken,
            IssuedAt = result.Data.IssuedAt,
            EffectiveAt = result.Data.EffectiveAt,
            ExpireAt = result.Data.ExpireAt,
            UserId = result.Data.UserId,
            LoginAccount = model.LoginAccount,
            DisplayName = result.Data.DisplayName
        });

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        adminSessionAccessor.Clear();
        return RedirectToAction("Login");
    }
}
