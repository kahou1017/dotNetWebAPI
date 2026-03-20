# SQL Server JWT API 資料表規格

## 目的

這份文件整理 `Dimensions` 在 SQL Server 上的核心資料表方向，讓新手先知道有哪些表、各自做什麼。

## 核心資料表

目前建議的主要資料表如下：

1. `JwtAdminUser`
2. `JwtDeviceRegistry`
3. `JwtToken`
4. `ApiRequestLog`
5. `ApiRequestPayloadLog`
6. `ApiExceptionLog`
7. `JwtTokenActionLog`
8. `JwtTokenUsageLog`

另外，正式業務模組可依需求增加自己的業務資料表。

目前第一個示範模組為：

- `Customer`
  - 建議對應資料表：`Customer`
  - 用來支援 `POST /api/customer/query`

## 各資料表用途

### 1. JwtAdminUser

用途：

- 儲存管理員帳號
- 管理員登入驗證

### 2. JwtDeviceRegistry

用途：

- 儲存可綁定裝置
- 管理單裝置 token 的 device 資料

### 3. JwtToken

用途：

- 儲存 token 主資料
- 管理 token 狀態
- 管理 token 類型、有效時間、裝置綁定

重要欄位概念：

- `TokenId`
- `JwtId`
- `TokenType`
- `UserId`
- `Status`
- `IsRevoked`
- `IsSingleDevice`
- `IsEnabled`
- `CanReissue`
- `CanRenew`
- `DeviceId`
- `IssuedAt`
- `EffectiveAt`
- `ExpireAt`
- `LastUsedAt`

### 4. ApiRequestLog

用途：

- 記錄 API request 基本資訊

目前已落地的欄位方向：

- `CaseId`
- `RequestTime`
- `HttpMethod`
- `RequestPath`
- `StatusCode`
- `ClientIp`
- `DeviceId`
- `IsAuthenticated`
- `IsSuccess`
- `TokenId`
- `UserId`
- `TokenType`

### 5. ApiRequestPayloadLog

用途：

- 記錄 request / response payload

### 6. ApiExceptionLog

用途：

- 記錄 exception

### 7. JwtTokenActionLog

用途：

- 記錄 token 被建立、撤銷、補發、續期等操作

### 8. JwtTokenUsageLog

用途：

- 記錄 token 實際被拿來呼叫 API 的使用紀錄

## Token 類型

`JwtToken.TokenType` 目前定義為：

- `AdminSession`
- `UserAccess`
- `Integration`
- `Service`

## 重點規則

### AdminSession

用途：

- 只給 `Dimensions.Admin.Api` 與 `Dimensions.Admin.Web`

### 業務 token

用途：

- `UserAccess`
- `Integration`
- `Service`

只給 `Dimensions.Api` 使用。

### 單裝置 token

當 `IsSingleDevice = 1`：

- `DeviceId` 欄位先保留
- 第一版可先不綁定 `DeviceId`
- 後續若正式啟用綁定，再要求 request 帶 `X-Device-Id`
- 到那個階段，request device 才需要與資料表綁定一致

## 新手先看哪裡

如果你是第一次碰這個 schema，建議先理解這三張表：

1. `JwtAdminUser`
2. `JwtDeviceRegistry`
3. `JwtToken`

因為登入、發 token、驗 token、綁 device，幾乎都從這三張表開始。
