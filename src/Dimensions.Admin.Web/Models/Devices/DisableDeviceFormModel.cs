using System.ComponentModel.DataAnnotations;

namespace Dimensions.Admin.Web.Models.Devices;

public sealed class DisableDeviceFormModel
{
    [Required]
    public string DeviceId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "停用原因")]
    public string Reason { get; set; } = "Disabled from Admin Web";
}
