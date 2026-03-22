using Dimensions.Contracts.Common;
using Dimensions.Contracts.Token;

namespace Dimensions.Admin.Web.Models.Tokens;

public sealed class TokenIndexPageModel
{
    public TokenListRequest Filter { get; init; } = new();

    public PagedResult<TokenListItemResponse>? Result { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public string? CaseId { get; init; }
}
