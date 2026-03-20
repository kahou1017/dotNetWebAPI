# Dimensions 各 Project NuGet 套件清單

## 說明

這份文件整理 `Dimensions` solution 目前各 project 直接使用到的 NuGet 套件。

整理原則：
- 只記錄 `.csproj` 中明確宣告的 `PackageReference`
- `ProjectReference` 不列入 NuGet 套件
- 若套件會帶入重要的相依套件，會補充在說明區
- 目前沒有直接套件的專案，也會明確標示

---

## Solution

目前包含：
- `Dimensions.Api`
- `Dimensions.Application`
- `Dimensions.Domain`
- `Dimensions.Infrastructure`
- `Dimensions.Contracts`
- `Dimensions.Api.Tests`
- `Dimensions.Application.Tests`

---

## 1. Dimensions.Api

專案位置：
- `src/Dimensions.Api/Dimensions.Api.csproj`

### 直接使用到的 NuGet 套件

| 套件名稱 | 版本 | 用途 |
|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `10.0.0` | 提供 JWT bearer 驗章與 ASP.NET Core authentication pipeline 整合 |
| `Microsoft.Extensions.Logging.Log4Net.AspNetCore` | `8.0.0` | 將 ASP.NET Core `ILogger` 接到 `log4net` provider，寫入本機檔案 log |

### 補充

- 目前 API 已改用真實 JWT bearer 驗證
- `AuthController` 會簽發 JWT，受保護 API 透過 bearer token 驗章
- `log4net.config` 負責 rolling file appender 設定

### 重要相依套件

由目前套件帶入或直接依賴的重要組件包含：

- `System.IdentityModel.Tokens.Jwt`
- `Microsoft.IdentityModel.Tokens`
- `log4net`
- `System.Configuration.ConfigurationManager`

---

## 2. Dimensions.Application

專案位置：
- `src/Dimensions.Application/Dimensions.Application.csproj`

### 直接使用到的 NuGet 套件

- 無

### 補充

- 目前主要參考專案：
  - `Dimensions.Domain`
  - `Dimensions.Contracts`

---

## 3. Dimensions.Domain

專案位置：
- `src/Dimensions.Domain/Dimensions.Domain.csproj`

### 直接使用到的 NuGet 套件

- 無

---

## 4. Dimensions.Infrastructure

專案位置：
- `src/Dimensions.Infrastructure/Dimensions.Infrastructure.csproj`

### 直接使用到的 NuGet 套件

| 套件名稱 | 版本 | 用途 |
|---|---|---|
| `Dapper` | `2.1.66` | 提供輕量 ORM / SQL mapping，作為資料存取骨架 |
| `Microsoft.Data.SqlClient` | `6.1.2` | SQL Server 連線驅動，供切回正式 SQL Server 時使用 |
| `Microsoft.Data.Sqlite` | `9.0.0` | SQLite 連線驅動，供本機開發模式建立與開啟 `.db` 檔 |

### 補充

- 目前主要參考專案：
  - `Dimensions.Application`
  - `Dimensions.Domain`
  - `Dimensions.Contracts`
- 已建立 `DatabaseOptions`、`IDbConnectionFactory`、`DbConnectionFactory`、`DapperSqlExecutor`
- Development 環境目前預設走 SQLite；正式環境仍可切回 SQL Server

---

## 5. Dimensions.Contracts

專案位置：
- `src/Dimensions.Contracts/Dimensions.Contracts.csproj`

### 直接使用到的 NuGet 套件

- 無

---

## 6. Dimensions.Api.Tests

專案位置：
- `tests/Dimensions.Api.Tests/Dimensions.Api.Tests.csproj`

### 直接使用到的 NuGet 套件

- 無

### 補充

- 目前主要參考專案：
  - `Dimensions.Api`

---

## 7. Dimensions.Application.Tests

專案位置：
- `tests/Dimensions.Application.Tests/Dimensions.Application.Tests.csproj`

### 直接使用到的 NuGet 套件

- 無

### 補充

- 目前主要參考專案：
  - `Dimensions.Application`

---

## 總覽

目前 solution 直接使用到的 NuGet 套件共有：

1. `Microsoft.AspNetCore.Authentication.JwtBearer` `10.0.0`
2. `Microsoft.Extensions.Logging.Log4Net.AspNetCore` `8.0.0`
3. `Dapper` `2.1.66`
4. `Microsoft.Data.SqlClient` `6.1.2`
5. `Microsoft.Data.Sqlite` `9.0.0`

目前主要用途集中在：

- 真實 JWT bearer 驗章
- 本機檔案 logging
- `log4net` provider 骨架
- 可切換的資料庫 provider 骨架
- SQL Server 連線骨架
- SQLite 開發模式
- Dapper 資料存取骨架

---

## 後續可能新增

後續如果進入正式開發，可能還會補入：

- `FluentValidation.AspNetCore`
- `Swashbuckle.AspNetCore`
- 視資料存取策略再評估是否補 `Microsoft.Extensions.Diagnostics.HealthChecks`

到時再同步更新這份文件即可。
