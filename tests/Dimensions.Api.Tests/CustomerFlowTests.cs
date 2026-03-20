using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dimensions.Api.Tests.TestHost;
using Dimensions.Contracts.Auth;
using Dimensions.Contracts.Business;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Token;
using Microsoft.Data.Sqlite;

namespace Dimensions.Api.Tests;

public sealed class CustomerFlowTests : IDisposable
{
    private readonly SqliteTestDatabase _database = new();

    [Fact]
    public async Task CustomerQuery_WithAdminIssuedToken_ReturnsCurrentUserContext()
    {
        await using var adminFactory = new AdminApiFactory(_database.ConnectionString);
        await using var businessFactory = new DimensionsApiFactory(_database.ConnectionString);

        using var adminClient = adminFactory.CreateClient();
        using var businessClient = businessFactory.CreateClient();

        var loginResponse = await adminClient.PostAsJsonAsync("/admin-api/auth/login", new LoginRequest
        {
            LoginAccount = "admin",
            Password = "admin"
        });

        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        var adminToken = loginPayload!.Data!.AccessToken;

        adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createTokenResponse = await adminClient.PostAsJsonAsync("/admin-api/token/create", new CreateTokenRequest
        {
            TokenType = "UserAccess",
            UserId = "USER900",
            UserName = "API Test User",
            TokenName = "Customer Query Test Token",
            IsSingleDevice = false,
            EffectiveAt = DateTimeOffset.UtcNow.AddMinutes(-1),
            ExpireAt = DateTimeOffset.UtcNow.AddDays(7),
            IsPermanent = false,
            CanReissue = true,
            CanRenew = true,
            Purpose = "IntegrationTest",
            Remark = "business api integration test"
        });

        createTokenResponse.EnsureSuccessStatusCode();
        var createTokenPayload = await createTokenResponse.Content.ReadFromJsonAsync<ApiResponse<CreateTokenResponse>>();
        var userToken = createTokenPayload!.Data!.AccessToken;

        businessClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var queryResponse = await businessClient.PostAsJsonAsync("/api/customer/query", new CustomerQueryRequest
        {
            CustomerId = "CUST-900",
            Keyword = "VIP"
        });

        queryResponse.EnsureSuccessStatusCode();
        var queryPayload = await queryResponse.Content.ReadFromJsonAsync<ApiResponse<CustomerQueryResponse>>();

        Assert.NotNull(queryPayload);
        Assert.True(queryPayload.Success);
        Assert.NotNull(queryPayload.Data);
        Assert.Equal("USER900", queryPayload.Data.QueriedByUserId);
        Assert.Equal("API Test User", queryPayload.Data.QueriedByName);
        Assert.Equal("CUST-900", queryPayload.Data.CustomerId);
        Assert.Equal("VIP Customer", queryPayload.Data.CustomerName);

        await using var connection = new SqliteConnection(_database.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(1)
            FROM ApiRequestLogs
            WHERE RequestPath = '/api/customer/query'
              AND UserId = 'USER900'
              AND TokenId IS NOT NULL;
            """;

        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        Assert.True(count >= 1);
    }

    public void Dispose() => _database.Dispose();
}
