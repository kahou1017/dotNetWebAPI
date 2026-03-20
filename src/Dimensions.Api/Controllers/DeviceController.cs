using Dimensions.Api.Policies;
using Dimensions.Api.Responses;
using Dimensions.Application.Interfaces;
using Dimensions.Contracts.Device;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Api.Controllers;

[ApiController]
[Authorize(Policy = PolicyNames.TokenManage)]
[Route("api/device")]
public sealed class DeviceController(IDeviceService deviceService, IConfiguration configuration) : ControllerBase
{
    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] DeviceListRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await deviceService.GetDeviceListAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDeviceRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await deviceService.CreateDeviceAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("disable")]
    public async Task<IActionResult> Disable([FromBody] DisableDeviceRequest request, CancellationToken cancellationToken)
    {
        await deviceService.DisableDeviceAsync(request, cancellationToken);
        return Ok(ApiResponseFactory.Success(HttpContext, new { request.DeviceId, Disabled = true }, GetSystemCode()));
    }

    private string GetSystemCode() => configuration["System:SystemCode"] ?? "Dimensions";
}
