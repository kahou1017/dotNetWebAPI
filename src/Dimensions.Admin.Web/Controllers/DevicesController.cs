using Dimensions.Admin.Web.Filters;
using Dimensions.Admin.Web.Models.Devices;
using Dimensions.Admin.Web.Services;
using Dimensions.Contracts.Device;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Web.Controllers;

[RequireAdminSession]
public sealed class DevicesController(IAdminApiClient adminApiClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] DeviceListRequest filter, CancellationToken cancellationToken)
    {
        var result = await adminApiClient.GetDevicesAsync(filter, cancellationToken);
        ViewData["Title"] = "Device 清單";
        ViewData["ActiveNav"] = "Devices";

        return View(new DeviceIndexPageModel
        {
            Filter = filter,
            Result = result.Data,
            DisableForm = new DisableDeviceFormModel(),
            ResultMessage = TempData["ResultMessage"] as string,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage,
            CaseId = result.CaseId
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "建立 Device";
        ViewData["ActiveNav"] = "Devices";
        return View(new DeviceCreatePageModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DeviceCreatePageModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "建立 Device";
        ViewData["ActiveNav"] = "Devices";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await adminApiClient.CreateDeviceAsync(
            new CreateDeviceRequest
            {
                UserId = model.UserId,
                DeviceId = model.DeviceId,
                DeviceName = model.DeviceName,
                DeviceType = model.DeviceType,
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

        model.ResultMessage = $"Device {model.DeviceId} 已建立。";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Disable(DisableDeviceFormModel form, CancellationToken cancellationToken)
    {
        var result = await adminApiClient.DisableDeviceAsync(
            new DisableDeviceRequest
            {
                DeviceId = form.DeviceId,
                Reason = form.Reason
            },
            cancellationToken);

        TempData["ResultMessage"] = result.IsSuccess
            ? $"Device {form.DeviceId} 已停用。"
            : result.ErrorMessage ?? "停用 Device 失敗。";

        return RedirectToAction(nameof(Index));
    }
}
