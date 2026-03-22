using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;

namespace Dimensions.Application.Services;

public sealed partial class ApiPayloadLogService(
    IApiPayloadLogRepository apiPayloadLogRepository,
    ICurrentUserAccessor currentUserAccessor) : IApiPayloadLogService
{
    private const int MaxPayloadLength = 4000;
    private const string MaskedValue = "***MASKED***";

    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "password",
        "accessToken",
        "refreshToken",
        "authorization"
    };

    public Task LogAsync(ApiPayloadLogEntry entry, CancellationToken cancellationToken = default)
    {
        var tokenId = currentUserAccessor.GetTokenId();
        var userId = currentUserAccessor.GetUserId();
        var tokenType = currentUserAccessor.GetTokenType();
        var (maskedPayload, isTruncated) = MaskPayload(entry.ContentType, entry.PayloadText);

        return apiPayloadLogRepository.InsertAsync(
            new ApiPayloadLogRecord
            {
                PayloadLogId = Guid.NewGuid().ToString("N"),
                CaseId = entry.CaseId,
                CreatedAt = entry.CreatedAt,
                Direction = entry.Direction,
                Method = entry.Method,
                Path = entry.Path,
                ContentType = entry.ContentType,
                PayloadText = maskedPayload,
                PayloadLength = entry.PayloadLength,
                IsTruncated = isTruncated,
                IsAuthenticated = !string.IsNullOrWhiteSpace(tokenId) && !string.IsNullOrWhiteSpace(userId),
                TokenId = tokenId,
                UserId = userId,
                TokenType = tokenType
            },
            cancellationToken);
    }

    private static (string PayloadText, bool IsTruncated) MaskPayload(string? contentType, string payloadText)
    {
        var normalized = payloadText ?? string.Empty;
        var masked = IsJsonContentType(contentType)
            ? MaskJsonPayload(normalized)
            : MaskTextPayload(normalized);

        if (masked.Length <= MaxPayloadLength)
        {
            return (masked, false);
        }

        return (masked[..MaxPayloadLength], true);
    }

    private static string MaskJsonPayload(string payloadText)
    {
        if (string.IsNullOrWhiteSpace(payloadText))
        {
            return payloadText;
        }

        try
        {
            var node = JsonNode.Parse(payloadText);
            if (node is null)
            {
                return payloadText;
            }

            MaskJsonNode(node);
            return node.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
        }
        catch (JsonException)
        {
            return MaskTextPayload(payloadText);
        }
    }

    private static void MaskJsonNode(JsonNode node)
    {
        if (node is JsonObject jsonObject)
        {
            foreach (var property in jsonObject.ToList())
            {
                if (property.Key is not null && SensitiveKeys.Contains(property.Key))
                {
                    jsonObject[property.Key] = MaskedValue;
                    continue;
                }

                if (property.Value is not null)
                {
                    MaskJsonNode(property.Value);
                }
            }

            return;
        }

        if (node is JsonArray jsonArray)
        {
            foreach (var item in jsonArray)
            {
                if (item is not null)
                {
                    MaskJsonNode(item);
                }
            }
        }
    }

    private static string MaskTextPayload(string payloadText)
    {
        if (string.IsNullOrWhiteSpace(payloadText))
        {
            return payloadText;
        }

        var masked = payloadText;
        masked = SensitiveFieldRegex().Replace(masked, match => $"{match.Groups["key"].Value}{match.Groups["separator"].Value}{MaskedValue}{match.Groups["suffix"].Value}");
        return masked;
    }

    private static bool IsJsonContentType(string? contentType)
        => !string.IsNullOrWhiteSpace(contentType)
           && contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);

    [GeneratedRegex("(?<key>password|accessToken|refreshToken|authorization)(?<separator>\\s*[=:]\\s*)(?<value>[^\\s,&;]+)(?<suffix>[,&;]?)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SensitiveFieldRegex();
}
