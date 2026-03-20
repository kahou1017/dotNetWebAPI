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
    ICurrentUserAccessor currentUserAccessor) : ControllerBase
{
    [HttpPost("query")]
    public IActionResult Query([FromBody] CustomerQueryRequest request)
    {
        var response = new CustomerQueryResponse
        {
            CustomerId = string.IsNullOrWhiteSpace(request.CustomerId) ? "CUST-001" : request.CustomerId,
            CustomerName = request.Keyword is { Length: > 0 }
                ? $"Customer matched: {request.Keyword}"
                : "Demo Customer",
            Status = "Active",
            QueriedByUserId = currentUserAccessor.GetUserId() ?? string.Empty,
            QueriedByName = currentUserAccessor.GetDisplayName() ?? string.Empty,
            DeviceId = HttpContext.Request.Headers[HeaderNames.DeviceId].ToString(),
            QueriedAt = DateTimeOffset.UtcNow
        };

        return Ok(ApiResponseFactory.Success(HttpContext, response, configuration["System:SystemCode"] ?? "Dimensions"));
    }
}
