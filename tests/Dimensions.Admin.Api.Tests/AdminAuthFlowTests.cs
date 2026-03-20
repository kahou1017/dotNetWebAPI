using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dimensions.Admin.Api.Tests.TestHost;
using Dimensions.Contracts.Auth;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Device;
using Microsoft.Data.Sqlite;

namespace Dimensions.Admin.Api.Tests;

public sealed class AdminAuthFlowTests : IDisposable
{
    private readonly SqliteTestDatabase _database = new();

    [Fact]
    public async Task Login_And_Me_ReturnCurrentAdminProfile()
    {
        await using var factory = new DimensionsAdminApiFactory(_database.ConnectionString);
        using var client = factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/admin-api/auth/login", new LoginRequest
        {
            LoginAccount = "admin",
            Password = "admin"
        });

        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();

        Assert.NotNull(loginPayload);
        Assert.True(loginPayload.Success);
        Assert.NotNull(loginPayload.Data);
        Assert.Equal("ADMIN001", loginPayload.Data.UserId);
        Assert.Equal("System Admin", loginPayload.Data.DisplayName);
        Assert.False(string.IsNullOrWhiteSpace(loginPayload.Data.AccessToken));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload.Data.AccessToken);

        var meResponse = await client.GetAsync("/admin-api/auth/me");
        meResponse.EnsureSuccessStatusCode();
        var mePayload = await meResponse.Content.ReadFromJsonAsync<ApiResponse<CurrentAdminResponse>>();

        Assert.NotNull(mePayload);
        Assert.True(mePayload.Success);
        Assert.NotNull(mePayload.Data);
        Assert.Equal("ADMIN001", mePayload.Data.UserId);
        Assert.Equal("admin", mePayload.Data.LoginAccount);
        Assert.Equal("System Admin", mePayload.Data.DisplayName);

        await using var connection = new SqliteConnection(_database.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(1)
            FROM ApiRequestLogs
            WHERE RequestPath IN ('/admin-api/auth/login', '/admin-api/auth/me');
            """;

        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        Assert.True(count >= 2);
    }

    [Fact]
    public async Task DisableDevice_WithUnknownDevice_WritesApiExceptionLog()
    {
        await using var factory = new DimensionsAdminApiFactory(_database.ConnectionString);
        using var client = factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/admin-api/auth/login", new LoginRequest
        {
            LoginAccount = "admin",
            Password = "admin"
        });

        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload!.Data!.AccessToken);

        var disableResponse = await client.PostAsJsonAsync("/admin-api/device/disable", new DisableDeviceRequest
        {
            DeviceId = "DEVICE-NOT-FOUND",
            Reason = "integration test"
        });

        Assert.Equal(System.Net.HttpStatusCode.NotFound, disableResponse.StatusCode);

        await using var connection = new SqliteConnection(_database.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(1)
            FROM ApiExceptionLogs
            WHERE RequestPath = '/admin-api/device/disable'
              AND ErrorCode = 'Device.NotFound'
              AND UserId = 'ADMIN001';
            """;

        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        Assert.True(count >= 1);
    }

    public void Dispose() => _database.Dispose();
}
