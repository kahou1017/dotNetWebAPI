namespace Dimensions.Application.Interfaces;

public interface ICurrentUserAccessor
{
    string? GetUserId();

    string? GetDisplayName();

    string? GetRole();

    string? GetScope();

    string? GetTokenType();
}
