# dotNetWebAPI

`Dimensions` 是目前開發中的 `.NET 10` WebAPI 專案。

目前主線以 `develop` 分支為主，`main` 暫不異動。

## 專案現況

- 已完成 `Dimensions.Api / Application / Domain / Infrastructure / Contracts / tests` 基本骨架
- 已接上 `log4net`
- 已接上 JWT Bearer 驗證
- Development 環境使用 SQLite
- 已完成 request validation
- 已完成 Swagger / OpenAPI
- 已完成第一波 token / device 規則補強
- 已整理新版正式文件到 `docs/`

## 目前架構方向

- `Dimensions.Api`
  - 只提供業務 API
  - 不提供 login API
  - 只驗 token，不簽發 token

- `Dimensions.Admin.Api`
  - 提供管理員登入
  - 管理 token
  - 管理 device
  - 提供管理查詢 API

- `Dimensions.Admin.Web`
  - 採用 ASP.NET Core MVC
  - 呼叫 `Dimensions.Admin.Api`

## 專案建立狀態

### 目前已建立的專案

- `Dimensions.Api`
- `Dimensions.Application`
- `Dimensions.Contracts`
- `Dimensions.Domain`
- `Dimensions.Infrastructure`
- `Dimensions.Api.Tests`
- `Dimensions.Application.Tests`

### 已定案、待建立的專案

- `Dimensions.Admin.Api`
- `Dimensions.Admin.Web`
- `Dimensions.Admin.Api.Tests`

## Solution 結構

### 目前已建立

```text
Dimensions.slnx
src/
  Dimensions.Api
  Dimensions.Application
  Dimensions.Contracts
  Dimensions.Domain
  Dimensions.Infrastructure
tests/
  Dimensions.Api.Tests
  Dimensions.Application.Tests
```

### 已定案、待建立

```text
src/
  Dimensions.Admin.Api
  Dimensions.Admin.Web
tests/
  Dimensions.Admin.Api.Tests
```

## 技術棧

- `.NET 10`
- ASP.NET Core Web API
- SQLite
- Dapper
- FluentValidation
- log4net

## 本機啟動

1. Restore

```powershell
dotnet restore .\Dimensions.slnx
```

2. Build

```powershell
dotnet build .\Dimensions.slnx --no-restore
```

3. Run API

```powershell
dotnet run --project .\src\Dimensions.Api
```

4. Open Swagger

- `http://localhost:<port>/swagger`

## Development Database

Development 預設使用 [appsettings.Development.json](src/Dimensions.Api/appsettings.Development.json)：

- `Provider`: `Sqlite`
- `ConnectionStrings:DefaultConnection`: `Data Source=dimensions-dev.db;Cache=Shared;Foreign Keys=True`

SQLite 檔案位置：

- [dimensions-dev.db](src/Dimensions.Api/dimensions-dev.db)

## Seed 帳號

- `admin / admin`
- `admin2 / admin2`

## 目前 API 狀態

### 目前已存在

- `POST /api/customer/query`
- `GET /api/public/{resource}`

### 目前程式碼仍保留的過渡骨架

目前程式碼裡還留有舊版 `Auth / Token / Device` 骨架，後續會依正式文件逐步切分到：

- `Dimensions.Admin.Api`
- `Dimensions.Admin.Web`

也就是說：

- 現行程式碼不完全等於最新正式文件
- 正式方向請以 `docs/` 下的文件為主

### 已定案、待切分的 API 入口

- `Dimensions.Admin.Api`
  - admin login
  - token 管理
  - device 管理
  - 管理查詢 API

## Request Validation

目前已套用 validator 的 request model：

- `LoginRequest`
- `TokenListRequest`
- `TokenDetailRequest`
- `CreateTokenRequest`
- `RevokeTokenRequest`
- `ReissueTokenRequest`
- `RenewTokenRequest`
- `TokenUsageRequest`
- `TokenActionLogRequest`
- `DeviceListRequest`
- `CreateDeviceRequest`
- `DisableDeviceRequest`
- `CustomerQueryRequest`

## Token / Device 規則現況

目前已補上的重點：

- `revoke / renew / reissue` 已加入基本前置條件檢查
- `renew` 會簽發新的 JWT 與 `jti`
- 驗章時會明確處理 `Revoked / Expired / Disabled / Reissued`

目前採用的過渡策略：

- `DeviceId` 欄位先保留，但第一版暫不強制實作
- 單裝置 token 可先建立為未綁定狀態
- 後續可由管理頁面根據 usage log 觀察候選來源
- 等流程定案後，再補正式 device 綁定

## Logging

- API 使用 `log4net`
- 設定檔在 [log4net.config](src/Dimensions.Api/log4net.config)
- 本機 log 會寫到 `Logs/`

## Swagger / OpenAPI

- 開發環境可直接使用 `/swagger`
- 已支援 JWT Bearer `Authorize`

## VS Code

VS Code 工作區推薦設定在 [extensions.json](.vscode/extensions.json)。

## Git Flow

- `develop`: 目前主線開發分支
- `main`: 暫不異動

## 文件入口

正式文件請從這裡開始：

- [00_master_index.md](docs/00_master_index.md)

如果你想先看每個 `src` project 與目錄用途，可以接著看：

- [09_project_structure_guide.md](docs/09_project_structure_guide.md)

開發待辦請看：

- [README.md](planning/README.md)
- [00_backend_todo_list.md](planning/00_backend_todo_list.md)
- [01_frontend_todo_list.md](planning/01_frontend_todo_list.md)

## 舊版文件說明

以下資料夾目前視為舊版素材：

- `docs/v1_1/`

可以拿來參考，但不作為正式閱讀入口。

## 文件維護規則

- 每次寫 code，如果有影響使用方式、設定、流程或文件內容，會一起檢查 `README.md`
- 正式規格文件以 `docs/` 為主
- 工作追蹤與待辦以 `planning/` 為主
