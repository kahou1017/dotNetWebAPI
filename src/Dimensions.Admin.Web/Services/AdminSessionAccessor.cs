using System.Text.Json;
using Dimensions.Admin.Web.Models;

namespace Dimensions.Admin.Web.Services;

public sealed class AdminSessionAccessor(IHttpContextAccessor httpContextAccessor) : IAdminSessionAccessor
{
    private const string SessionKey = "Dimensions.Admin.Web.AdminSession";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public AdminSessionState? GetCurrent()
    {
        var httpContext = httpContextAccessor.HttpContext;
        var json = httpContext?.Session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<AdminSessionState>(json, SerializerOptions);
    }

    public void SetCurrent(AdminSessionState session)
    {
        var httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available.");

        var json = JsonSerializer.Serialize(session, SerializerOptions);
        httpContext.Session.SetString(SessionKey, json);
    }

    public void Clear()
    {
        httpContextAccessor.HttpContext?.Session.Remove(SessionKey);
    }
}
