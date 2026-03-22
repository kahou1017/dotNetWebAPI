using System.ComponentModel.DataAnnotations;

namespace Dimensions.Admin.Web.Models.Devices;

public sealed class DeviceCreatePageModel
{
    [Required]
    [Display(Name = "使用者 ID")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DeviceId")]
    public string DeviceId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Device 名稱")]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Device 類型")]
    public string DeviceType { get; set; } = string.Empty;

    [Display(Name = "備註")]
    public string? Remark { get; set; }

    public string? ResultMessage { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? CaseId { get; set; }
}
