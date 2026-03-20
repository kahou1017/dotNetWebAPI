# SQL Server 建表腳本（V1.1，可直接執行）

> 目標：提供 JWT 管理系統 + API 查核/稽核所需之 SQL Server DDL  
> 本版已納入 V1.1 調整：`TokenType`、`JwtDeviceRegistry.UserId` 必填、索引補強

## 一、設計前提

- 資料庫：MS SQL Server
- `caseId` 採 `UNIQUEIDENTIFIER`
- 時間採 `DATETIME2(3)`
- 字串以 `NVARCHAR` 為主
- Token 本體不明文保存，只保留 `TokenHash` 與 `TokenPrefix`
- Schema：`dbo`

---

## 二、建表順序

1. `JwtAdminUser`
2. `JwtDeviceRegistry`
3. `JwtToken`
4. `ApiRequestLog`
5. `ApiRequestPayloadLog`
6. `ApiExceptionLog`
7. `JwtTokenActionLog`
8. `JwtTokenUsageLog`

---

## 三、完整建表腳本

```sql
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.JwtAdminUser', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.JwtAdminUser
    (
        Id                      BIGINT IDENTITY(1,1) NOT NULL,
        UserId                  NVARCHAR(100) NOT NULL,
        LoginAccount            NVARCHAR(100) NOT NULL,
        DisplayName             NVARCHAR(200) NULL,
        PasswordHash            NVARCHAR(256) NOT NULL,
        PasswordSalt            NVARCHAR(256) NOT NULL,
        Email                   NVARCHAR(200) NULL,
        IsEnabled               BIT NOT NULL CONSTRAINT DF_JwtAdminUser_IsEnabled DEFAULT (1),
        IsLocked                BIT NOT NULL CONSTRAINT DF_JwtAdminUser_IsLocked DEFAULT (0),
        LastLoginAt             DATETIME2(3) NULL,
        LastPasswordChangeAt    DATETIME2(3) NULL,
        CreatedAt               DATETIME2(3) NOT NULL CONSTRAINT DF_JwtAdminUser_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedBy               NVARCHAR(100) NOT NULL,
        UpdatedAt               DATETIME2(3) NULL,
        UpdatedBy               NVARCHAR(100) NULL,
        CONSTRAINT PK_JwtAdminUser PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_JwtAdminUser_UserId UNIQUE (UserId),
        CONSTRAINT UQ_JwtAdminUser_LoginAccount UNIQUE (LoginAccount)
    );
END
GO

IF OBJECT_ID(N'dbo.JwtDeviceRegistry', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.JwtDeviceRegistry
    (
        Id                      BIGINT IDENTITY(1,1) NOT NULL,
        DeviceId                NVARCHAR(200) NOT NULL,
        UserId                  NVARCHAR(100) NOT NULL,
        DeviceName              NVARCHAR(200) NULL,
        DeviceType              NVARCHAR(100) NULL,
        DeviceFingerprint       NVARCHAR(500) NULL,
        IsEnabled               BIT NOT NULL CONSTRAINT DF_JwtDeviceRegistry_IsEnabled DEFAULT (1),
        FirstRegisterAt         DATETIME2(3) NULL,
        LastUsedAt              DATETIME2(3) NULL,
        Remark                  NVARCHAR(1000) NULL,
        CreatedAt               DATETIME2(3) NOT NULL CONSTRAINT DF_JwtDeviceRegistry_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt               DATETIME2(3) NULL,
        CONSTRAINT PK_JwtDeviceRegistry PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_JwtDeviceRegistry_DeviceId UNIQUE (DeviceId)
    );
END
GO

IF OBJECT_ID(N'dbo.JwtToken', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.JwtToken
    (
        Id                      BIGINT IDENTITY(1,1) NOT NULL,
        TokenId                 NVARCHAR(100) NOT NULL,
        JwtId                   NVARCHAR(100) NOT NULL,
        TokenType               NVARCHAR(50) NOT NULL,
        TokenName               NVARCHAR(200) NULL,
        UserId                  NVARCHAR(100) NOT NULL,
        UserName                NVARCHAR(200) NULL,
        TokenHash               NVARCHAR(256) NOT NULL,
        TokenPrefix             NVARCHAR(50) NULL,
        Issuer                  NVARCHAR(200) NULL,
        Audience                NVARCHAR(200) NULL,
        Subject                 NVARCHAR(200) NULL,
        Scope                   NVARCHAR(500) NULL,
        Status                  NVARCHAR(50) NOT NULL,
        IsRevoked               BIT NOT NULL CONSTRAINT DF_JwtToken_IsRevoked DEFAULT (0),
        IsSingleDevice          BIT NOT NULL CONSTRAINT DF_JwtToken_IsSingleDevice DEFAULT (0),
        IsEnabled               BIT NOT NULL CONSTRAINT DF_JwtToken_IsEnabled DEFAULT (1),
        CanReissue              BIT NOT NULL CONSTRAINT DF_JwtToken_CanReissue DEFAULT (1),
        CanRenew                BIT NOT NULL CONSTRAINT DF_JwtToken_CanRenew DEFAULT (1),
        DeviceId                NVARCHAR(200) NULL,
        DeviceName              NVARCHAR(200) NULL,
        DeviceFingerprint       NVARCHAR(500) NULL,
        IssuedAt                DATETIME2(3) NOT NULL,
        EffectiveAt             DATETIME2(3) NOT NULL,
        ExpireAt                DATETIME2(3) NULL,
        LastUsedAt              DATETIME2(3) NULL,
        RevokedAt               DATETIME2(3) NULL,
        ReissuedAt              DATETIME2(3) NULL,
        ParentTokenId           NVARCHAR(100) NULL,
        ReissueFromTokenId      NVARCHAR(100) NULL,
        Purpose                 NVARCHAR(100) NULL,
        Remark                  NVARCHAR(1000) NULL,
        CreatedSource           NVARCHAR(100) NULL,
        CreatedBy               NVARCHAR(100) NOT NULL,
        CreatedAt               DATETIME2(3) NOT NULL CONSTRAINT DF_JwtToken_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedBy               NVARCHAR(100) NULL,
        UpdatedAt               DATETIME2(3) NULL,
        CONSTRAINT PK_JwtToken PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_JwtToken_TokenId UNIQUE (TokenId),
        CONSTRAINT UQ_JwtToken_JwtId UNIQUE (JwtId),
        CONSTRAINT FK_JwtToken_JwtDeviceRegistry_DeviceId FOREIGN KEY (DeviceId) REFERENCES dbo.JwtDeviceRegistry(DeviceId),
        CONSTRAINT CK_JwtToken_Status CHECK (Status IN (N'Active', N'Revoked', N'Expired', N'Reissued', N'Disabled')),
        CONSTRAINT CK_JwtToken_TokenType CHECK (TokenType IN (N'AdminSession', N'UserAccess', N'Integration', N'Service')),
        CONSTRAINT CK_JwtToken_Effective_Expire CHECK (ExpireAt IS NULL OR EffectiveAt <= ExpireAt),
        CONSTRAINT CK_JwtToken_SingleDevice CHECK (IsSingleDevice = 0 OR DeviceId IS NOT NULL)
    );
END
GO

IF OBJECT_ID(N'dbo.ApiRequestLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApiRequestLog
    (
        Id                      BIGINT IDENTITY(1,1) NOT NULL,
        CaseId                  UNIQUEIDENTIFIER NOT NULL,
        CorrelationId           NVARCHAR(100) NULL,
        RequestTime             DATETIME2(3) NOT NULL,
        ResponseTime            DATETIME2(3) NULL,
        ElapsedMs               INT NULL,
        HttpMethod              NVARCHAR(10) NOT NULL,
        RequestPath             NVARCHAR(500) NOT NULL,
        RouteTemplate           NVARCHAR(500) NULL,
        QueryString             NVARCHAR(MAX) NULL,
        ContentType             NVARCHAR(100) NULL,
        ClientIp                NVARCHAR(50) NULL,
        UserAgent               NVARCHAR(1000) NULL,
        Referer                 NVARCHAR(1000) NULL,
        HostName                NVARCHAR(255) NULL,
        IsAuthenticated         BIT NOT NULL CONSTRAINT DF_ApiRequestLog_IsAuthenticated DEFAULT (0),
        UserId                  NVARCHAR(100) NULL,
        UserName                NVARCHAR(200) NULL,
        TokenId                 NVARCHAR(100) NULL,
        JwtId                   NVARCHAR(100) NULL,
        TokenType               NVARCHAR(50) NULL,
        DeviceId                NVARCHAR(200) NULL,
        Success                 BIT NOT NULL CONSTRAINT DF_ApiRequestLog_Success DEFAULT (0),
        HttpStatusCode          INT NOT NULL,
        SystemCode              NVARCHAR(100) NULL,
        ErrorCode               NVARCHAR(200) NULL,
        ErrorMessage            NVARCHAR(1000) NULL,
        ModuleName              NVARCHAR(100) NULL,
        ActionType              NVARCHAR(100) NULL,
        ControllerName          NVARCHAR(100) NULL,
        ActionName              NVARCHAR(100) NULL,
        EnvironmentName         NVARCHAR(50) NULL,
        ServerName              NVARCHAR(100) NULL,
        CreatedAt               DATETIME2(3) NOT NULL CONSTRAINT DF_ApiRequestLog_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_ApiRequestLog PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_ApiRequestLog_CaseId UNIQUE (CaseId),
        CONSTRAINT FK_ApiRequestLog_JwtToken_TokenId FOREIGN KEY (TokenId) REFERENCES dbo.JwtToken(TokenId)
    );
END
GO

IF OBJECT_ID(N'dbo.ApiRequestPayloadLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApiRequestPayloadLog
    (
        Id                      BIGINT IDENTITY(1,1) NOT NULL,
        ApiRequestLogId         BIGINT NOT NULL,
        CaseId                  UNIQUEIDENTIFIER NOT NULL,
        RequestBody             NVARCHAR(MAX) NULL,
        RequestBodyMasked       NVARCHAR(MAX) NULL,
        ResponseBody            NVARCHAR(MAX) NULL,
        ResponseBodyMasked      NVARCHAR(MAX) NULL,
        RequestHash             NVARCHAR(128) NULL,
        ResponseHash            NVARCHAR(128) NULL,
        RequestSize             BIGINT NULL,
        ResponseSize            BIGINT NULL,
        CreatedAt               DATETIME2(3) NOT NULL CONSTRAINT DF_ApiRequestPayloadLog_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_ApiRequestPayloadLog PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_ApiRequestPayloadLog_ApiRequestLog FOREIGN KEY (ApiRequestLogId) REFERENCES dbo.ApiRequestLog(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.ApiExceptionLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApiExceptionLog
    (
        Id                      BIGINT IDENTITY(1,1) NOT NULL,
        ApiRequestLogId         BIGINT NOT NULL,
        CaseId                  UNIQUEIDENTIFIER NOT NULL,
        ExceptionType           NVARCHAR(500) NULL,
        ExceptionMessage        NVARCHAR(MAX) NULL,
        InnerExceptionMessage   NVARCHAR(MAX) NULL,
        StackTrace              NVARCHAR(MAX) NULL,
        Source                  NVARCHAR(500) NULL,
        HelpLink                NVARCHAR(1000) NULL,
        ErrorLevel              NVARCHAR(50) NULL,
        CreatedAt               DATETIME2(3) NOT NULL CONSTRAINT DF_ApiExceptionLog_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_ApiExceptionLog PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_ApiExceptionLog_ApiRequestLog FOREIGN KEY (ApiRequestLogId) REFERENCES dbo.ApiRequestLog(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.JwtTokenActionLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.JwtTokenActionLog
    (
        Id                      BIGINT IDENTITY(1,1) NOT NULL,
        CaseId                  UNIQUEIDENTIFIER NOT NULL,
        TokenId                 NVARCHAR(100) NOT NULL,
        JwtTokenId              BIGINT NULL,
        ActionType              NVARCHAR(100) NOT NULL,
        ActionReason            NVARCHAR(1000) NULL,
        ActionResult            NVARCHAR(50) NOT NULL,
        OperatorUserId          NVARCHAR(100) NOT NULL,
        OperatorUserName        NVARCHAR(200) NULL,
        SourceIp                NVARCHAR(50) NULL,
        SourceDevice            NVARCHAR(200) NULL,
        BeforeStatus            NVARCHAR(50) NULL,
        AfterStatus             NVARCHAR(50) NULL,
        BeforeExpireAt          DATETIME2(3) NULL,
        AfterExpireAt           DATETIME2(3) NULL,
        BeforeDeviceId          NVARCHAR(200) NULL,
        AfterDeviceId           NVARCHAR(200) NULL,
        Remark                  NVARCHAR(1000) NULL,
        CreatedAt               DATETIME2(3) NOT NULL CONSTRAINT DF_JwtTokenActionLog_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_JwtTokenActionLog PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_JwtTokenActionLog_JwtTokenId FOREIGN KEY (JwtTokenId) REFERENCES dbo.JwtToken(Id),
        CONSTRAINT FK_JwtTokenActionLog_TokenId FOREIGN KEY (TokenId) REFERENCES dbo.JwtToken(TokenId)
    );
END
GO

IF OBJECT_ID(N'dbo.JwtTokenUsageLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.JwtTokenUsageLog
    (
        Id                      BIGINT IDENTITY(1,1) NOT NULL,
        CaseId                  UNIQUEIDENTIFIER NOT NULL,
        TokenId                 NVARCHAR(100) NOT NULL,
        JwtTokenId              BIGINT NULL,
        JwtId                   NVARCHAR(100) NULL,
        TokenType               NVARCHAR(50) NULL,
        UserId                  NVARCHAR(100) NULL,
        RequestTime             DATETIME2(3) NOT NULL,
        HttpMethod              NVARCHAR(10) NOT NULL,
        RequestPath             NVARCHAR(500) NOT NULL,
        ClientIp                NVARCHAR(50) NULL,
        UserAgent               NVARCHAR(1000) NULL,
        DeviceId                NVARCHAR(200) NULL,
        DeviceFingerprint       NVARCHAR(500) NULL,
        IsSuccess               BIT NOT NULL,
        FailureReason           NVARCHAR(200) NULL,
        ServerName              NVARCHAR(100) NULL,
        EnvironmentName         NVARCHAR(50) NULL,
        CreatedAt               DATETIME2(3) NOT NULL CONSTRAINT DF_JwtTokenUsageLog_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_JwtTokenUsageLog PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_JwtTokenUsageLog_JwtTokenId FOREIGN KEY (JwtTokenId) REFERENCES dbo.JwtToken(Id),
        CONSTRAINT FK_JwtTokenUsageLog_TokenId FOREIGN KEY (TokenId) REFERENCES dbo.JwtToken(TokenId)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtAdminUser_IsEnabled' AND object_id = OBJECT_ID(N'dbo.JwtAdminUser'))
    CREATE NONCLUSTERED INDEX IX_JwtAdminUser_IsEnabled ON dbo.JwtAdminUser(IsEnabled);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtAdminUser_IsLocked' AND object_id = OBJECT_ID(N'dbo.JwtAdminUser'))
    CREATE NONCLUSTERED INDEX IX_JwtAdminUser_IsLocked ON dbo.JwtAdminUser(IsLocked);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtDeviceRegistry_UserId' AND object_id = OBJECT_ID(N'dbo.JwtDeviceRegistry'))
    CREATE NONCLUSTERED INDEX IX_JwtDeviceRegistry_UserId ON dbo.JwtDeviceRegistry(UserId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtDeviceRegistry_IsEnabled' AND object_id = OBJECT_ID(N'dbo.JwtDeviceRegistry'))
    CREATE NONCLUSTERED INDEX IX_JwtDeviceRegistry_IsEnabled ON dbo.JwtDeviceRegistry(IsEnabled);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtToken_UserId' AND object_id = OBJECT_ID(N'dbo.JwtToken'))
    CREATE NONCLUSTERED INDEX IX_JwtToken_UserId ON dbo.JwtToken(UserId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtToken_Status' AND object_id = OBJECT_ID(N'dbo.JwtToken'))
    CREATE NONCLUSTERED INDEX IX_JwtToken_Status ON dbo.JwtToken(Status);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtToken_TokenType' AND object_id = OBJECT_ID(N'dbo.JwtToken'))
    CREATE NONCLUSTERED INDEX IX_JwtToken_TokenType ON dbo.JwtToken(TokenType);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtToken_ExpireAt' AND object_id = OBJECT_ID(N'dbo.JwtToken'))
    CREATE NONCLUSTERED INDEX IX_JwtToken_ExpireAt ON dbo.JwtToken(ExpireAt);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtToken_DeviceId' AND object_id = OBJECT_ID(N'dbo.JwtToken'))
    CREATE NONCLUSTERED INDEX IX_JwtToken_DeviceId ON dbo.JwtToken(DeviceId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtToken_UserId_TokenType_Status' AND object_id = OBJECT_ID(N'dbo.JwtToken'))
    CREATE NONCLUSTERED INDEX IX_JwtToken_UserId_TokenType_Status ON dbo.JwtToken(UserId, TokenType, Status);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApiRequestLog_RequestTime' AND object_id = OBJECT_ID(N'dbo.ApiRequestLog'))
    CREATE NONCLUSTERED INDEX IX_ApiRequestLog_RequestTime ON dbo.ApiRequestLog(RequestTime);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApiRequestLog_TokenId' AND object_id = OBJECT_ID(N'dbo.ApiRequestLog'))
    CREATE NONCLUSTERED INDEX IX_ApiRequestLog_TokenId ON dbo.ApiRequestLog(TokenId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApiRequestPayloadLog_ApiRequestLogId' AND object_id = OBJECT_ID(N'dbo.ApiRequestPayloadLog'))
    CREATE NONCLUSTERED INDEX IX_ApiRequestPayloadLog_ApiRequestLogId ON dbo.ApiRequestPayloadLog(ApiRequestLogId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApiExceptionLog_ApiRequestLogId' AND object_id = OBJECT_ID(N'dbo.ApiExceptionLog'))
    CREATE NONCLUSTERED INDEX IX_ApiExceptionLog_ApiRequestLogId ON dbo.ApiExceptionLog(ApiRequestLogId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtTokenActionLog_TokenId' AND object_id = OBJECT_ID(N'dbo.JwtTokenActionLog'))
    CREATE NONCLUSTERED INDEX IX_JwtTokenActionLog_TokenId ON dbo.JwtTokenActionLog(TokenId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtTokenUsageLog_TokenId' AND object_id = OBJECT_ID(N'dbo.JwtTokenUsageLog'))
    CREATE NONCLUSTERED INDEX IX_JwtTokenUsageLog_TokenId ON dbo.JwtTokenUsageLog(TokenId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JwtTokenUsageLog_RequestTime_TokenId' AND object_id = OBJECT_ID(N'dbo.JwtTokenUsageLog'))
    CREATE NONCLUSTERED INDEX IX_JwtTokenUsageLog_RequestTime_TokenId ON dbo.JwtTokenUsageLog(RequestTime, TokenId);
GO
```

---

## 四、V1.1 欄位補充說明

### 1. `TokenType`

必要欄位，用於區分：

- 管理員登入 Session
- 一般使用者 Access Token
- Integration Token
- Service Token

### 2. `Scope`

可選欄位，用於儲存模組或授權範圍。

### 3. `ExpireAt`

可為 `NULL`，代表永久 Token。

### 4. `JwtDeviceRegistry.UserId`

V1.1 視為必填，避免匿名裝置與一般裝置規則混用。

### 5. `CK_JwtToken_SingleDevice`

若 `IsSingleDevice = 1`，則 `DeviceId` 不可為 `NULL`。

---

## 五、後續實作注意事項

1. `token.UserId` 與 `device.UserId` 一致性仍需由應用層驗證  
2. `RequestBody` / `ResponseBody` 需遮罩敏感資料  
3. 所有時間欄位建議統一使用 UTC  
4. `JwtTokenUsageLog` 為高成長表，需注意索引與歸檔

---

## 六、建表後驗證項目

- 所有表是否成功建立
- `TokenType` constraint 是否存在
- `JwtDeviceRegistry.UserId` 是否為必填
- `IsSingleDevice = 1` 是否限制 `DeviceId`
- `ExpireAt = NULL` 是否可接受
- `JwtToken` 是否可依 `TokenType` 查詢
