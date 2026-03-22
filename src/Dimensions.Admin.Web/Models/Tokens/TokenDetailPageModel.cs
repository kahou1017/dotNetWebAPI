using Dimensions.Contracts.Common;
using Dimensions.Contracts.Token;

namespace Dimensions.Admin.Web.Models.Tokens;

public sealed class TokenDetailPageModel
{
    public string TokenId { get; init; } = string.Empty;

    public TokenDetailResponse? Detail { get; init; }

    public PagedResult<TokenUsageItemResponse>? UsageLogs { get; init; }

    public PagedResult<TokenActionLogItemResponse>? ActionLogs { get; init; }

    public RevokeTokenFormModel RevokeForm { get; init; } = new();

    public ReissueTokenFormModel ReissueForm { get; init; } = new();

    public RenewTokenFormModel RenewForm { get; init; } = new();

    public string? ResultTitle { get; init; }

    public string? ResultMessage { get; init; }

    public string? ResultAccessToken { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public string? CaseId { get; init; }
}
