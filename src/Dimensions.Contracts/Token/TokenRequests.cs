namespace Dimensions.Contracts.Token;

public sealed record TokenListRequest
{
    public string? TokenType { get; init; }
    public string? UserId { get; init; }
    public string? TokenId { get; init; }
    public string? Status { get; init; }
    public bool? IsSingleDevice { get; init; }
    public string? DeviceId { get; init; }
    public DateTimeOffset? IssuedAtStart { get; init; }
    public DateTimeOffset? IssuedAtEnd { get; init; }
    public DateTimeOffset? ExpireAtStart { get; init; }
    public DateTimeOffset? ExpireAtEnd { get; init; }
    public int PageNo { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record TokenDetailRequest
{
    public string TokenId { get; init; } = string.Empty;
}

public sealed record CreateTokenRequest
{
    public string TokenType { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string TokenName { get; init; } = string.Empty;
    public bool IsSingleDevice { get; init; }
    public string? DeviceId { get; init; }
    public string? DeviceName { get; init; }
    public DateTimeOffset EffectiveAt { get; init; }
    public DateTimeOffset? ExpireAt { get; init; }
    public bool IsPermanent { get; init; }
    public bool CanReissue { get; init; } = true;
    public bool CanRenew { get; init; } = true;
    public string? Purpose { get; init; }
    public string? Remark { get; init; }
}

public sealed record RevokeTokenRequest
{
    public string TokenId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

public sealed record ReissueTokenRequest
{
    public string TokenId { get; init; } = string.Empty;
    public DateTimeOffset EffectiveAt { get; init; }
    public DateTimeOffset? ExpireAt { get; init; }
    public string? DeviceId { get; init; }
    public string? DeviceName { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public sealed record RenewTokenRequest
{
    public string TokenId { get; init; } = string.Empty;
    public DateTimeOffset? NewExpireAt { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public sealed record TokenUsageRequest
{
    public string? TokenId { get; init; }
    public string? UserId { get; init; }
    public string? DeviceId { get; init; }
    public string? ClientIp { get; init; }
    public bool? IsSuccess { get; init; }
    public DateTimeOffset? RequestTimeStart { get; init; }
    public DateTimeOffset? RequestTimeEnd { get; init; }
    public int PageNo { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record TokenActionLogRequest
{
    public string? TokenId { get; init; }
    public string? ActionType { get; init; }
    public string? OperatorUserId { get; init; }
    public DateTimeOffset? CreatedAtStart { get; init; }
    public DateTimeOffset? CreatedAtEnd { get; init; }
    public int PageNo { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
