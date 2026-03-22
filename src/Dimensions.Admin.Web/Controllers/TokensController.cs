using Dimensions.Admin.Web.Filters;
using Dimensions.Admin.Web.Models.Tokens;
using Dimensions.Admin.Web.Services;
using Dimensions.Contracts.Token;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Web.Controllers;

[RequireAdminSession]
public sealed class TokensController(IAdminApiClient adminApiClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] TokenListRequest filter, CancellationToken cancellationToken)
    {
        var result = await adminApiClient.GetTokenListAsync(filter, cancellationToken);
        ViewData["Title"] = "Token 清單";
        ViewData["ActiveNav"] = "Tokens";

        return View(new TokenIndexPageModel
        {
            Filter = filter,
            Result = result.Data,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage,
            CaseId = result.CaseId
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "建立 Token";
        ViewData["ActiveNav"] = "Tokens";
        return View(new TokenCreatePageModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TokenCreatePageModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "建立 Token";
        ViewData["ActiveNav"] = "Tokens";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await adminApiClient.CreateTokenAsync(
            new CreateTokenRequest
            {
                TokenType = model.TokenType,
                UserId = model.UserId,
                UserName = model.UserName,
                TokenName = model.TokenName,
                Scope = model.Scope,
                IsSingleDevice = model.IsSingleDevice,
                DeviceId = model.DeviceId,
                DeviceName = model.DeviceName,
                EffectiveAt = model.EffectiveAt,
                ExpireAt = model.IsPermanent ? null : model.ExpireAt,
                IsPermanent = model.IsPermanent,
                CanReissue = model.CanReissue,
                CanRenew = model.CanRenew,
                Purpose = model.Purpose,
                Remark = model.Remark
            },
            cancellationToken);

        if (!result.IsSuccess)
        {
            model.ErrorCode = result.ErrorCode;
            model.ErrorMessage = result.ErrorMessage;
            model.CaseId = result.CaseId;
            return View(model);
        }

        model.CreatedToken = result.Data;
        return View(model);
    }

    [HttpGet("/tokens/{tokenId}")]
    public async Task<IActionResult> Detail(string tokenId, CancellationToken cancellationToken)
    {
        var detailResult = await adminApiClient.GetTokenDetailAsync(new TokenDetailRequest { TokenId = tokenId }, cancellationToken);
        var usageResult = await adminApiClient.GetTokenUsageAsync(new TokenUsageRequest { TokenId = tokenId, PageNo = 1, PageSize = 10 }, cancellationToken);
        var actionResult = await adminApiClient.GetTokenActionLogsAsync(new TokenActionLogRequest { TokenId = tokenId, PageNo = 1, PageSize = 10 }, cancellationToken);

        ViewData["Title"] = "Token 詳細資料";
        ViewData["ActiveNav"] = "Tokens";

        return View(new TokenDetailPageModel
        {
            TokenId = tokenId,
            Detail = detailResult.Data,
            UsageLogs = usageResult.Data,
            ActionLogs = actionResult.Data,
            RevokeForm = new RevokeTokenFormModel { TokenId = tokenId },
            ReissueForm = new ReissueTokenFormModel { TokenId = tokenId },
            RenewForm = new RenewTokenFormModel { TokenId = tokenId },
            ResultTitle = TempData["ResultTitle"] as string,
            ResultMessage = TempData["ResultMessage"] as string,
            ResultAccessToken = TempData["ResultAccessToken"] as string,
            ErrorCode = detailResult.ErrorCode,
            ErrorMessage = detailResult.ErrorMessage,
            CaseId = detailResult.CaseId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revoke(RevokeTokenFormModel form, CancellationToken cancellationToken)
    {
        var result = await adminApiClient.RevokeTokenAsync(new RevokeTokenRequest { TokenId = form.TokenId, Reason = form.Reason }, cancellationToken);
        SetTempResult(result.IsSuccess, "撤銷 Token", result.ErrorMessage ?? $"Token {form.TokenId} 已撤銷。", null);
        return RedirectToAction(nameof(Detail), new { tokenId = form.TokenId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reissue(ReissueTokenFormModel form, CancellationToken cancellationToken)
    {
        var result = await adminApiClient.ReissueTokenAsync(
            new ReissueTokenRequest
            {
                TokenId = form.TokenId,
                EffectiveAt = form.EffectiveAt,
                ExpireAt = form.ExpireAt,
                DeviceId = form.DeviceId,
                DeviceName = form.DeviceName,
                Reason = form.Reason
            },
            cancellationToken);

        SetTempResult(result.IsSuccess, "補發 Token", result.ErrorMessage ?? $"Token {form.TokenId} 已補發。", result.Data?.AccessToken);
        return RedirectToAction(nameof(Detail), new { tokenId = form.TokenId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Renew(RenewTokenFormModel form, CancellationToken cancellationToken)
    {
        var result = await adminApiClient.RenewTokenAsync(
            new RenewTokenRequest
            {
                TokenId = form.TokenId,
                NewExpireAt = form.NewExpireAt,
                Reason = form.Reason
            },
            cancellationToken);

        SetTempResult(result.IsSuccess, "續期 Token", result.ErrorMessage ?? $"Token {form.TokenId} 已續期。", result.Data?.AccessToken);
        return RedirectToAction(nameof(Detail), new { tokenId = form.TokenId });
    }

    private void SetTempResult(bool isSuccess, string title, string message, string? accessToken)
    {
        TempData["ResultTitle"] = isSuccess ? title : $"{title}失敗";
        TempData["ResultMessage"] = message;
        TempData["ResultAccessToken"] = accessToken;
    }
}
