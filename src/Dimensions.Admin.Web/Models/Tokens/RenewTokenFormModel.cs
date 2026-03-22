using System.ComponentModel.DataAnnotations;

namespace Dimensions.Admin.Web.Models.Tokens;

public sealed class RenewTokenFormModel
{
    [Required]
    public string TokenId { get; set; } = string.Empty;

    [Display(Name = "新的到期時間")]
    public DateTimeOffset? NewExpireAt { get; set; } = DateTimeOffset.UtcNow.AddDays(30);

    [Required]
    [Display(Name = "續期原因")]
    public string Reason { get; set; } = "Admin Web renew";
}
