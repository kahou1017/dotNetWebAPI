using System.ComponentModel.DataAnnotations;

namespace Dimensions.Admin.Web.Models.Tokens;

public sealed class RevokeTokenFormModel
{
    [Required]
    public string TokenId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "撤銷原因")]
    public string Reason { get; set; } = "Admin Web revoke";
}
