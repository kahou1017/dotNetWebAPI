using Dimensions.Api.Policies;
using Dimensions.Api.Responses;
using Dimensions.Application.Interfaces;
using Dimensions.Contracts.Business;
using Dimensions.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Api.Controllers;

[ApiController]
[Authorize(Policy = PolicyNames.AuthenticatedUser)]
[Route("api/customer")]
public sealed class CustomerController(
    IConfiguration configuration,
    ICustomerService customerService) : ControllerBase
{
    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] CustomerQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await customerService.QueryAsync(
            request,
            HttpContext.Request.Headers[HeaderNames.DeviceId].ToString(),
            cancellationToken);

        return Ok(ApiResponseFactory.Success(HttpContext, response, configuration["System:SystemCode"] ?? "Dimensions"));
    }
}
