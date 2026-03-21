# .NET WebAPI 架構規格

## 目的

這份文件定義 `Dimensions` solution 的 project 切分、責任分配，以及 API host 的基本執行方式。

## Solution 結構

```text
Dimensions.sln
src/
  Dimensions.Api
  Dimensions.Admin.Api
  Dimensions.Admin.Web
  Dimensions.Application
  Dimensions.Domain
  Dimensions.Infrastructure
  Dimensions.Contracts
tests/
  Dimensions.Api.Tests
  Dimensions.Admin.Api.Tests
  Dimensions.Application.Tests
```

## 各 project 職責

### 1. Dimensions.Api

負責：

- 業務 API controller
- 業務 token 的 JWT 驗證
- 業務 API authorization policy
- middleware、filter、Swagger、logging

常見 controller：

- `CustomerController`
- 未來的 `OrderController`
- 未來的 `AccountController`

不負責：

- 管理員登入
- `TokenController`
- `DeviceController`

### 2. Dimensions.Admin.Api

負責：

- 管理員登入
- 取得目前登入管理員資訊
- token 管理 API
- device 管理 API
- 管理查詢與稽核 API

常見 controller：

- `AuthController`
- `TokenController`
- `DeviceController`

### 3. Dimensions.Admin.Web

負責：

- ASP.NET Core MVC 管理前端
- 頁面流程
- 表單提交
- 管理操作畫面
- 呼叫 `Dimensions.Admin.Api`

### 4. Dimensions.Application

負責：

- use case / service orchestration
- 應用層規則
- repository 介面
- current user / caseId 抽象

常見 service：

- `AuthService`
- `TokenService`
- `DeviceService`
- `TokenUsageLogService`

### 5. Dimensions.Domain

負責：

- enum
- constants
- domain rule
- 共用語意模型

重要 enum：

- `TokenType`
- `TokenStatus`

### 6. Dimensions.Infrastructure

負責：

- Dapper / SQL 存取
- SQLite / SQL Server provider
- repository implementation
- logging persistence
- security / time helper

### 7. Dimensions.Contracts

負責：

- request DTO
- response DTO
- shared response model
- paging model

重要共用模型：

- `ApiResponse<T>`
- `ApiErrorData`
- `PagedResult<T>`

## API Host 執行方式

### 共用 pipeline 方向

`Dimensions.Api` 與 `Dimensions.Admin.Api` 應採相同的基本平台風格：

1. `CaseIdMiddleware`
2. `ExceptionHandlingMiddleware`
3. Authentication
4. Authorization
5. Request logging
6. Controllers

### 回應格式責任

成功回應：

- 由 controller 回傳 `ApiResponse<T>`

失敗回應：

- 由 `ExceptionHandlingMiddleware` 處理
- 由 validation filter 處理 request validation failure

不使用：

- `UnifiedResponseFilter`

## Policy 模型

### Admin API

- `AdminOnly`
- `TokenManage`

### 業務 API

- `AuthenticatedUser`
- 未來可擴充 `Scope:{scope}` 或 `ModuleAccess:{module}`

## Token 類型使用規則

### Dimensions.Admin.Api

接受：

- `AdminSession`

### Dimensions.Api

接受：

- `UserAccess`
- `Integration`
- `Service`

拒絕：

- `AdminSession`

## Logging

目前標準：

- 使用 ASP.NET Core `ILogger<T>`
- 使用 `log4net` 作為本機檔案 logging provider
- `ApiRequestLogMiddleware` 目前已會將基本 request 資訊寫入 DB
- `ExceptionHandlingMiddleware` 目前已會將例外基本資訊寫入 DB
- `TokenUsageLogService` 目前只負責 token usage log，不等同完整 API logging service

之後 `Dimensions.Api` 與 `Dimensions.Admin.Api` 都應遵循相同方向。

## 設定檔方向

### Dimensions.Api

包含：

- 業務 API JWT 驗證設定
- DB connection
- logging

### Dimensions.Admin.Api

包含：

- 管理員登入相關設定
- token management 設定
- DB connection
- logging

### Dimensions.Admin.Web

包含：

- Admin API base URL
- MVC app 設定
- 必要的 session / UI 設定

## 對新手的補充說明

### 為什麼要拆成 `Api` 與 `Admin.Api`

因為兩者雖然共用底層邏輯，但入口責任不同：

- `Dimensions.Api`
  - 是給外部或前端呼叫的業務 API
- `Dimensions.Admin.Api`
  - 是給管理端用的後台 API

這樣拆之後：

- 安全邊界更清楚
- Swagger 更清楚
- controller 責任更明確
- 後續部署也更有彈性

### 為什麼共用層先不再細拆

因為目前最有價值的是「入口分清楚」，不是「底層拆很多 project」。

對新手來說，這樣也比較容易理解：

- 先看入口
- 再看共用 service
- 最後看 repository
