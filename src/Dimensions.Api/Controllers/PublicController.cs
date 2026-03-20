using Dimensions.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dimensions.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/public")]
public sealed class PublicController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("{resource}")]
    public IActionResult Get(string resource)
    {
        object data = resource.ToLowerInvariant() switch
        {
            "system-info" => new { Name = "Dimensions", Version = "v1.1-skeleton" },
            "code-list" => new[] { "AdminSession", "UserAccess", "Integration", "Service" },
            _ => new { Resource = resource, Message = "Public skeleton endpoint." }
        };

        return Ok(ApiResponseFactory.Success(HttpContext, data, configuration["System:SystemCode"] ?? "Dimensions"));
    }
}
