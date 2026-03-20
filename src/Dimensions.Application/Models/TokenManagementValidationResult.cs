namespace Dimensions.Application.Models;

public sealed record TokenManagementValidationResult
{
    public required string EffectiveStatus { get; init; }

    public required bool IsExpired { get; init; }
}
