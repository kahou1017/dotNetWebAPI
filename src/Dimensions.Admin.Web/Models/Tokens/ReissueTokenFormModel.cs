using System.ComponentModel.DataAnnotations;

namespace Dimensions.Admin.Web.Models.Tokens;

public sealed class ReissueTokenFormModel
{
    [Required]
    public string TokenId { get; set; } = string.Empty;

    [Display(Name = "新的生效時間")]
    public DateTimeOffset EffectiveAt { get; set; } = DateTimeOffset.UtcNow;

    [Display(Name = "新的到期時間")]
    public DateTimeOffset? ExpireAt { get; set; } = DateTimeOffset.UtcNow.AddDays(30);

    [Display(Name = "DeviceId")]
    public string? DeviceId { get; set; }

    [Display(Name = "DeviceName")]
    public string? DeviceName { get; set; }

    [Required]
    [Display(Name = "補發原因")]
    public string Reason { get; set; } = "Admin Web reissue";
}
