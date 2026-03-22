using System.ComponentModel.DataAnnotations;

namespace Dimensions.Admin.Web.Models.Account;

public sealed class LoginPageModel
{
    [Required(ErrorMessage = "請輸入登入帳號。")]
    [Display(Name = "登入帳號")]
    public string LoginAccount { get; set; } = string.Empty;

    [Required(ErrorMessage = "請輸入密碼。")]
    [DataType(DataType.Password)]
    [Display(Name = "密碼")]
    public string Password { get; set; } = string.Empty;

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? CaseId { get; set; }
}
