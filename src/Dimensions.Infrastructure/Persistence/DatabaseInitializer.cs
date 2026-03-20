using Dapper;
using Dimensions.Application.Interfaces;
using Dimensions.Domain.Enums;
using Dimensions.Infrastructure.Options;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dimensions.Infrastructure.Persistence;

public sealed class DatabaseInitializer(
    IDbConnectionFactory connectionFactory,
    IOptions<DatabaseOptions> options,
    ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    private readonly DatabaseOptions _options = options.Value;
    private readonly ILogger<DatabaseInitializer> _logger = logger;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!string.Equals(_options.Provider, "Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Database initializer skipped for provider {Provider}.", _options.Provider);
            return;
        }

        await using var connection = (SqliteConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string schemaSql = """
            PRAGMA foreign_keys = ON;

            CREATE TABLE IF NOT EXISTS AdminUsers (
                UserId TEXT NOT NULL PRIMARY KEY,
                LoginAccount TEXT NOT NULL,
                Password TEXT NOT NULL,
                DisplayName TEXT NOT NULL,
                Role TEXT NOT NULL,
                Scope TEXT NULL,
                LastLoginAt TEXT NULL,
                CreatedAt TEXT NOT NULL
            );

            CREATE UNIQUE INDEX IF NOT EXISTS IX_AdminUsers_LoginAccount ON AdminUsers(LoginAccount);

            CREATE TABLE IF NOT EXISTS Devices (
                DeviceId TEXT NOT NULL PRIMARY KEY,
                UserId TEXT NOT NULL,
                DeviceName TEXT NOT NULL,
                DeviceType TEXT NOT NULL,
                IsEnabled INTEGER NOT NULL,
                Remark TEXT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Customers (
                CustomerId TEXT NOT NULL PRIMARY KEY,
                CustomerName TEXT NOT NULL,
                Status TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS ApiRequestLogs (
                CaseId TEXT NOT NULL PRIMARY KEY,
                RequestTime TEXT NOT NULL,
                HttpMethod TEXT NOT NULL,
                RequestPath TEXT NOT NULL,
                StatusCode INTEGER NOT NULL,
                ClientIp TEXT NULL,
                DeviceId TEXT NULL,
                IsAuthenticated INTEGER NOT NULL,
                IsSuccess INTEGER NOT NULL,
                TokenId TEXT NULL,
                UserId TEXT NULL,
                TokenType TEXT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_ApiRequestLogs_RequestTime ON ApiRequestLogs(RequestTime);
            CREATE INDEX IF NOT EXISTS IX_ApiRequestLogs_TokenId ON ApiRequestLogs(TokenId);

            CREATE TABLE IF NOT EXISTS Tokens (
                TokenId TEXT NOT NULL PRIMARY KEY,
                JwtId TEXT NOT NULL,
                TokenType TEXT NOT NULL,
                UserId TEXT NOT NULL,
                UserName TEXT NOT NULL,
                TokenName TEXT NOT NULL,
                Status TEXT NOT NULL,
                IsRevoked INTEGER NOT NULL,
                IsSingleDevice INTEGER NOT NULL,
                IsEnabled INTEGER NOT NULL,
                CanReissue INTEGER NOT NULL,
                CanRenew INTEGER NOT NULL,
                DeviceId TEXT NULL,
                DeviceName TEXT NULL,
                AccessToken TEXT NOT NULL,
                IssuedAt TEXT NOT NULL,
                EffectiveAt TEXT NOT NULL,
                ExpireAt TEXT NULL,
                LastUsedAt TEXT NULL,
                Purpose TEXT NULL,
                Remark TEXT NULL,
                CreatedBy TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                RevokedAt TEXT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_Tokens_UserId ON Tokens(UserId);
            CREATE INDEX IF NOT EXISTS IX_Tokens_DeviceId ON Tokens(DeviceId);
            CREATE INDEX IF NOT EXISTS IX_Tokens_Status ON Tokens(Status);

            CREATE TABLE IF NOT EXISTS TokenUsageLogs (
                CaseId TEXT NOT NULL PRIMARY KEY,
                TokenId TEXT NOT NULL,
                UserId TEXT NOT NULL,
                RequestTime TEXT NOT NULL,
                HttpMethod TEXT NOT NULL,
                RequestPath TEXT NOT NULL,
                ClientIp TEXT NULL,
                DeviceId TEXT NULL,
                IsSuccess INTEGER NOT NULL,
                FailureReason TEXT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_TokenUsageLogs_TokenId ON TokenUsageLogs(TokenId);
            CREATE INDEX IF NOT EXISTS IX_TokenUsageLogs_RequestTime ON TokenUsageLogs(RequestTime);

            CREATE TABLE IF NOT EXISTS TokenActionLogs (
                CaseId TEXT NOT NULL PRIMARY KEY,
                TokenId TEXT NOT NULL,
                ActionType TEXT NOT NULL,
                ActionReason TEXT NULL,
                ActionResult TEXT NOT NULL,
                OperatorUserId TEXT NOT NULL,
                OperatorUserName TEXT NOT NULL,
                BeforeStatus TEXT NULL,
                AfterStatus TEXT NULL,
                CreatedAt TEXT NOT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_TokenActionLogs_TokenId ON TokenActionLogs(TokenId);
            CREATE INDEX IF NOT EXISTS IX_TokenActionLogs_CreatedAt ON TokenActionLogs(CreatedAt);
            """;

        await connection.ExecuteAsync(new CommandDefinition(schemaSql, cancellationToken: cancellationToken));

        const string seedSql = """
            INSERT OR IGNORE INTO AdminUsers (UserId, LoginAccount, Password, DisplayName, Role, Scope, LastLoginAt, CreatedAt)
            VALUES
                ('ADMIN001', 'admin', 'admin', 'System Admin', 'Admin', 'token.manage', NULL, @SeedNow),
                ('ADMIN002', 'admin2', 'admin2', 'Backup Admin', 'Admin', 'token.manage', NULL, @SeedNow);

            INSERT OR IGNORE INTO Devices (DeviceId, UserId, DeviceName, DeviceType, IsEnabled, Remark, CreatedAt, UpdatedAt)
            VALUES
                ('DEVICE-001', 'USER001', 'Kevin Laptop', 'Windows', 1, 'seed device', @SeedNow, @SeedNow);

            INSERT OR IGNORE INTO Customers (CustomerId, CustomerName, Status, UpdatedAt)
            VALUES
                ('CUST-001', 'Demo Customer', 'Active', @SeedNow),
                ('CUST-900', 'VIP Customer', 'Active', @SeedNow),
                ('CUST-999', 'Suspended Customer', 'Suspended', @SeedNow);

            INSERT OR IGNORE INTO Tokens (
                TokenId, JwtId, TokenType, UserId, UserName, TokenName, Status, IsRevoked, IsSingleDevice, IsEnabled,
                CanReissue, CanRenew, DeviceId, DeviceName, AccessToken, IssuedAt, EffectiveAt, ExpireAt, LastUsedAt,
                Purpose, Remark, CreatedBy, CreatedAt, RevokedAt)
            VALUES
                ('ADM2026000001', 'JTI-ADM-0001', @AdminSessionType, 'ADMIN001', 'System Admin', 'Admin Session', @ActiveStatus, 0, 0, 1, 0, 0, NULL, NULL, 'seed-admin-session', @SeedNow, @SeedNow, NULL, @SeedNow, 'AdminLogin', 'seed admin token', 'SYSTEM', @SeedNow, NULL),
                ('TK2026000001', 'JTI-TK-0001', @UserAccessType, 'USER001', 'Kevin', 'Main Token', @ActiveStatus, 0, 1, 1, 1, 1, 'DEVICE-001', 'Kevin Laptop', 'seed-user-token', @SeedNow, @SeedNow, @UserExpireAt, @SeedNow, 'UserAccess', 'seed user token', 'ADMIN001', @SeedNow, NULL),
                ('TK2026000004', 'JTI-TK-0004', @IntegrationType, 'SYSTEM001', 'Integration Client', 'Integration Token', @ActiveStatus, 0, 0, 1, 1, 1, NULL, NULL, 'seed-integration-token', @SeedNow, @SeedNow, @IntegrationExpireAt, @SeedNow, 'Integration', 'seed integration token', 'ADMIN001', @SeedNow, NULL);

            INSERT OR IGNORE INTO TokenUsageLogs (CaseId, TokenId, UserId, RequestTime, HttpMethod, RequestPath, ClientIp, DeviceId, IsSuccess, FailureReason)
            VALUES
                ('CASE-SEED-USAGE-001', 'TK2026000001', 'USER001', @SeedNow, 'POST', '/api/customer/query', '127.0.0.1', 'DEVICE-001', 1, NULL);

            INSERT OR IGNORE INTO TokenActionLogs (CaseId, TokenId, ActionType, ActionReason, ActionResult, OperatorUserId, OperatorUserName, BeforeStatus, AfterStatus, CreatedAt)
            VALUES
                ('CASE-SEED-ACTION-001', 'TK2026000001', 'CreateToken', 'seed action', 'Success', 'ADMIN001', 'System Admin', NULL, @ActiveStatus, @SeedNow);
            """;

        var seedNow = DateTimeOffset.UtcNow;
        await connection.ExecuteAsync(new CommandDefinition(
            seedSql,
            new
            {
                SeedNow = seedNow,
                UserExpireAt = seedNow.AddDays(30),
                IntegrationExpireAt = seedNow.AddDays(90),
                AdminSessionType = TokenType.AdminSession,
                UserAccessType = TokenType.UserAccess,
                IntegrationType = TokenType.Integration,
                ActiveStatus = TokenStatus.Active
            },
            cancellationToken: cancellationToken));

        _logger.LogInformation("SQLite database initialization completed.");
    }
}
