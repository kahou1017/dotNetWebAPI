using Dimensions.Admin.Api.Policies;
using Dimensions.Admin.Api.Responses;
using Dimensions.Application.Interfaces;
using Dimensions.Contracts.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Admin.Api.Controllers;

[ApiController]
[Authorize(Policy = PolicyNames.TokenManage)]
[Route("admin-api/log")]
public sealed class LogController(ILogService logService, IConfiguration configuration) : ControllerBase
{
    [HttpPost("request/list")]
    public async Task<IActionResult> RequestList([FromBody] ApiRequestLogListRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await logService.GetApiRequestLogsAsync(request, cancellationToken), GetSystemCode()));

    [HttpPost("exception/list")]
    public async Task<IActionResult> ExceptionList([FromBody] ApiExceptionLogListRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponseFactory.Success(HttpContext, await logService.GetApiExceptionLogsAsync(request, cancellationToken), GetSystemCode()));

    private string GetSystemCode() => configuration["System:SystemCode"] ?? "Dimensions";
}
