# .NET WebAPI 專案架構規格 V1.1

## 一、目標

建立一套可延續至 .NET 8 / 9 / 10 的 WebAPI 專案架構，並滿足：

- JWT 驗證與管理
- `TokenType` 分型
- 統一 JSON 回傳
- `caseId` 全域追蹤
- API Log / Exception Log / Payload Log / Usage Log
- Device 綁定驗證
- CORS 白名單
- 本機檔案 log
- 可作為 ASMX 遷移目標架構

---

## 二、專案分層建議

```text
Dimensions.slnx
├─ src
│  ├─ Dimensions.Api
│  ├─ Dimensions.Application
│  ├─ Dimensions.Domain
│  ├─ Dimensions.Infrastructure
│  └─ Dimensions.Contracts
└─ tests
   ├─ Dimensions.Api.Tests
   └─ Dimensions.Application.Tests
```

---

## 三、各層責任

### 1. `Dimensions.Api`

負責：
- Controller
- Middleware
- Authentication / Authorization
- Filter
- Request / Response 轉接
- Swagger
- CORS
- Logging provider 啟動

不負責：
- 業務核心規則
- Token 狀態核心判斷
- Device 綁定核心規則

### 2. `Dimensions.Application`

負責：
- Use Case / Service
- Auth 流程
- Token 流程
- Device 流程
- Token / Device / User 一致性驗證
- 驗證規則整合

### 3. `Dimensions.Domain`

負責：
- Entity / Aggregate
- Domain Rule
- Enum / 常數
- Value Object

### 4. `Dimensions.Infrastructure`

負責：
- SQL Server
- Repository
- JWT 產生器
- Hash 服務
- Time Provider
- 後續資料存取與 logging 落地實作

### 5. `Dimensions.Contracts`

負責：
- Request DTO
- Response DTO
- 共用 Model
- `ApiResponse<T>`
- `ApiErrorData`
- `PagedResult<T>`

---

## 四、API 專案內部結構建議

```text
Dimensions.Api
├─ Authentication
│  └─ DimensionsAuthenticationHandler.cs
├─ Constants
│  └─ ApiContextItemKeys.cs
├─ Controllers
│  ├─ AuthController.cs
│  ├─ TokenController.cs
│  ├─ DeviceController.cs
│  └─ PublicController.cs
├─ Middleware
│  ├─ CaseIdMiddleware.cs
│  ├─ ExceptionHandlingMiddleware.cs
│  └─ ApiRequestLogMiddleware.cs
├─ Extensions
│  ├─ ServiceCollectionExtensions.cs
│  └─ AuthenticationExtensions.cs
├─ Policies
│  └─ PolicyNames.cs
├─ Responses
│  └─ ApiResponseFactory.cs
├─ Services
│  └─ HttpContextCaseIdAccessor.cs
├─ Program.cs
├─ appsettings*.json
└─ log4net.config
```

### V1.1 決議

- 不採用 `UnifiedResponseFilter`
- `ApiResponse<T>` 不放在 `Api` 專案
- 成功回傳由 Controller 明確回傳
- 失敗回傳由 `ExceptionHandlingMiddleware` 與 validation 機制統一產生
- 本機檔案 log 採 `log4net`

---

## 五、Application 層結構建議

```text
Dimensions.Application
├─ Interfaces
│  ├─ IAuthService.cs
│  ├─ ITokenService.cs
│  ├─ IDeviceService.cs
│  ├─ IApiLogService.cs
│  └─ ICaseIdAccessor.cs
└─ Services
   ├─ AuthService.cs
   ├─ TokenService.cs
   ├─ DeviceService.cs
   └─ ApiLogService.cs
```

---

## 六、Domain 層結構建議

```text
Dimensions.Domain
├─ Constants
│  ├─ HeaderNames.cs
│  ├─ ClaimNames.cs
│  └─ SystemCodes.cs
└─ Enums
   ├─ TokenStatus.cs
   └─ TokenType.cs
```

### `TokenType` 建議值

- `AdminSession`
- `UserAccess`
- `Integration`
- `Service`

---

## 七、Infrastructure 層結構建議

```text
Dimensions.Infrastructure
└─ 後續放置：
   ├─ Persistence
   ├─ Security
   ├─ Time
   └─ Logging
```

---

## 八、Controller 清單建議

### `AuthController`

用途：
- 管理員登入
- 取得目前登入管理員資訊

API：
- `POST /api/auth/login`
- `GET /api/auth/me`

### `TokenController`

用途：
- Token 清單 / 明細 / 新建 / 撤銷 / 重發 / 延長 / 紀錄

API：
- `POST /api/token/list`
- `POST /api/token/detail`
- `POST /api/token/create`
- `POST /api/token/revoke`
- `POST /api/token/reissue`
- `POST /api/token/renew`
- `POST /api/token/usage`
- `POST /api/token/action-log`

### `DeviceController`

用途：
- Device 管理

API：
- `POST /api/device/list`
- `POST /api/device/create`
- `POST /api/device/disable`

### `PublicController`

用途：
- 公開 GET API

API：
- `GET /api/public/{resource}`

---

## 九、Service 清單建議

### `AuthService`

負責：
- 驗證管理員帳號
- 簽發 `AdminSession`
- 登入紀錄

### `TokenService`

負責：
- Token 清單 / 明細
- 新建 / 撤銷 / 重發 / 延長
- 驗證 DB Token 狀態
- 驗證 `TokenType`

### `DeviceService`

負責：
- Device 查詢
- Device 建立
- Device 停用
- 驗證 `token.UserId == device.UserId`

### `ApiLogService`

負責：
- `ApiRequestLog`
- `PayloadLog`
- `ExceptionLog`
- `UsageLog`
- `ActionLog`

---

## 十、共用回傳模型定案

### 成功回傳

由 Controller / Application Service 組合 `ApiResponse<T>`。

### 失敗回傳

由以下元件統一處理：

- `ExceptionHandlingMiddleware`
- Validation Filter 或 FluentValidation

### 不採用

- `UnifiedResponseFilter`

---

## 十一、Middleware / Pipeline 規格

### 1. `CaseIdMiddleware`

責任：
- Request 一進來即產生 `caseId`
- 放入 `HttpContext.Items`

不負責：
- 包裝 JSON
- 改寫成功 response

### 2. `ExceptionHandlingMiddleware`

責任：
- 全域例外攔截
- 轉成統一錯誤回應
- 寫入 `ApiExceptionLog`
- 寫入 logging pipeline

### 3. `ApiRequestLogMiddleware`

責任：
- 記錄 request 基本資訊
- 補寫 response 結果
- 串接 payload / usage log
- 將 request lifecycle 訊息寫入 ASP.NET Core logging pipeline

### 4. Authentication / Authorization

建議採 ASP.NET Core 官方機制。

最少提供：
- `AdminOnly`
- `AuthenticatedUser`
- `TokenManage`

---

## 十二、建議 Middleware 順序

1. `CaseIdMiddleware`
2. `ExceptionHandlingMiddleware`
3. Authentication
4. Authorization
5. `ApiRequestLogMiddleware`
6. Controller

---

## 十三、本機 Logging 定案

V1.1 實作骨架已採用：

- ASP.NET Core `ILogger<T>`
- `log4net` 作為本機檔案 logging provider

目前落地方式：

- `Dimensions.Api` 引用 `Microsoft.Extensions.Logging.Log4Net.AspNetCore`
- 於 `Program.cs` 啟用 `builder.Logging.AddLog4Net("log4net.config")`
- 啟動時自動建立 `Logs` 目錄
- 使用 `log4net.config` 設定 rolling file appender
- 預設 log 檔名格式為 `Dimensions.Api.yyyymmdd.log`

目前會寫入 logging pipeline 的典型訊息來源：

- `ApiRequestLogMiddleware`
- `ExceptionHandlingMiddleware`
- 其他使用 `ILogger<T>` 的元件

---

## 十四、設定檔規格建議

### `appsettings.json`

至少需要：

- `ConnectionStrings`
- `Jwt`
  - `Issuer`
  - `Audience`
  - `SecretKey`
  - `DefaultExpireDays`
  - `AllowPermanentToken`
- `Cors`
  - `AllowedOrigins`
- `Logging`
- `System`
  - `EnvironmentName`
  - `SystemCode`

### `log4net.config`

目前骨架補充：

- 置於 `Dimensions.Api` 專案根目錄
- 由專案檔複製到輸出目錄
- 採 `RollingFileAppender`
- 預設 level 為 `INFO`

---

## 十五、JWT Claim 建議

至少保留：

- `sub`
- `name`
- `role`
- `scope`
- `token_type`
- `token_id`
- `jti`

---

## 十六、ASMX 遷移原則

若從 ASMX 遷移，建議：

1. 先抽出舊商業邏輯到 `Application`
2. API Controller 只負責轉接
3. 舊回傳格式統一轉成 `ApiResponse`
4. 舊登入 / 驗證改為 JWT + DB Token 模型
5. Device / Token / Policy 規則只放在後端
6. 本機 log 與後續集中式 log 分開規劃

---

## 十七、實作優先順序建議

### Phase 1

- Solution 結構
- 設定檔
- DB 連線
- `Contracts`
- `CaseIdMiddleware`
- `ExceptionHandlingMiddleware`

### Phase 2

- Authentication
- Authorization Policy
- `AuthController`
- `TokenController` 基本功能

### Phase 3

- `DeviceController`
- `ApiLogService`
- Payload / Usage / Action Log
- `log4net` 本機檔案 logging

### Phase 4

- `PublicController`
- 業務模組擴充
- Scope / Module 權限
- Swagger / OpenAPI

---

## 十八、V1.1 定案重點

- 採 Controller-based Web API
- 採分層架構
- `ApiResponse<T>` 只放在 `Contracts`
- `caseId` 由 Middleware 產生
- JWT 驗證採驗章 + DB 狀態雙層
- `JwtToken` 必須有 `TokenType`
- 不採用 `UnifiedResponseFilter`
- 以 Policy 處理管理與一般驗證權限
- 本機檔案 log 採 `log4net`
