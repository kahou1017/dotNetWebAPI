namespace Dimensions.Contracts.Auth;

public sealed record LoginRequest
{
    public string LoginAccount { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
