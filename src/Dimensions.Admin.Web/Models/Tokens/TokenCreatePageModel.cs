using System.ComponentModel.DataAnnotations;
using Dimensions.Contracts.Token;

namespace Dimensions.Admin.Web.Models.Tokens;

public sealed class TokenCreatePageModel
{
    [Required]
    [Display(Name = "Token 類型")]
    public string TokenType { get; set; } = "UserAccess";

    [Required]
    [Display(Name = "使用者 ID")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "使用者名稱")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Token 名稱")]
    public string TokenName { get; set; } = string.Empty;

    [Display(Name = "Scope")]
    public string? Scope { get; set; } = "customer.query";

    [Display(Name = "單裝置綁定")]
    public bool IsSingleDevice { get; set; }

    [Display(Name = "DeviceId")]
    public string? DeviceId { get; set; }

    [Display(Name = "DeviceName")]
    public string? DeviceName { get; set; }

    [Display(Name = "生效時間")]
    public DateTimeOffset EffectiveAt { get; set; } = DateTimeOffset.UtcNow;

    [Display(Name = "到期時間")]
    public DateTimeOffset? ExpireAt { get; set; } = DateTimeOffset.UtcNow.AddDays(30);

    [Display(Name = "永久有效")]
    public bool IsPermanent { get; set; }

    [Display(Name = "允許補發")]
    public bool CanReissue { get; set; } = true;

    [Display(Name = "允許續期")]
    public bool CanRenew { get; set; } = true;

    [Display(Name = "用途")]
    public string? Purpose { get; set; }

    [Display(Name = "備註")]
    public string? Remark { get; set; }

    public CreateTokenResponse? CreatedToken { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? CaseId { get; set; }
}
