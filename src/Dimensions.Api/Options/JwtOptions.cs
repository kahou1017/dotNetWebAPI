namespace Dimensions.Api.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public int DefaultExpireDays { get; set; } = 30;

    public bool AllowPermanentToken { get; set; } = true;
}
