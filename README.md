# dotNetWebAPI

`Dimensions` 是目前開發中的 `.NET 10` WebAPI 專案，主線開發以 `develop` 分支為主，`main` 暫不異動。

## Current Status

- 已完成 `Api / Application / Domain / Infrastructure / Contracts / tests` 分層骨架
- 已接上 `log4net` 本機檔案 logging
- 已接上 `JWT Bearer` 驗章、DB token 狀態檢查與 authorization policy
- Development 環境已切到 `SQLite`
- 已完成 SQLite 啟動建表與基本 seed
- 已完成 `Auth / Token / Device / Customer / Public` API skeleton
- 已完成 Swagger / OpenAPI
- 已完成第一批與第二批 request validation

## Solution Structure

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

## Tech Stack

- `.NET 10`
- ASP.NET Core Web API
- SQLite
- Dapper
- FluentValidation
- log4net

## Local Run

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

如果要測受保護 API，先呼叫 `POST /api/auth/login` 取得 JWT，再到 Swagger 右上角 `Authorize` 輸入：

```text
Bearer {token}
```

## Development Database

Development 預設使用 [appsettings.Development.json](d:/Git/dotNetWebAPI/src/Dimensions.Api/appsettings.Development.json)：

- `Provider`: `Sqlite`
- `ConnectionStrings:DefaultConnection`: `Data Source=dimensions-dev.db;Cache=Shared;Foreign Keys=True`

SQLite 檔案位置：

- [dimensions-dev.db](d:/Git/dotNetWebAPI/src/Dimensions.Api/dimensions-dev.db)

## Seed Accounts

- `admin / admin`
- `admin2 / admin2`

## Current APIs

- `POST /api/auth/login`
- `GET /api/auth/me`
- `POST /api/token/list`
- `POST /api/token/detail`
- `POST /api/token/create`
- `POST /api/token/revoke`
- `POST /api/token/reissue`
- `POST /api/token/renew`
- `POST /api/token/usage`
- `POST /api/token/action-log`
- `POST /api/device/list`
- `POST /api/device/create`
- `POST /api/device/disable`
- `POST /api/customer/query`
- `GET /api/public/{resource}`

## Auth Rules

- `POST /api/auth/login` 會回傳 JWT bearer token
- 受保護 API 會先做 JWT 驗章，再回查 DB token 狀態
- 單裝置 token 需要在 request header 帶 `X-Device-Id`
- 驗證成功的受保護請求會寫 usage log，並更新 token 的 `LastUsedAt`
- `POST /api/customer/query` 用來驗證一般使用者 token 的受保護業務流程

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

## Logging

- API 使用 `log4net`
- 設定檔在 [log4net.config](d:/Git/dotNetWebAPI/src/Dimensions.Api/log4net.config)
- 本機 log 會寫到 `Logs/`

## Swagger / OpenAPI

- 開發環境可直接使用 `/swagger`
- 已支援 JWT Bearer `Authorize`

## VS Code

VS Code 工作區推薦設定在 [extensions.json](d:/Git/dotNetWebAPI/.vscode/extensions.json)。

目前已推薦 SQLite 檢視相關外掛，方便直接查看 `.db` 檔。

## Git Flow

- `develop`: 目前主線開發分支
- `main`: 暫不異動

目前所有開發變更先進 `develop`，確認穩定後再決定是否整理到 `main`。

## Documentation Rule

- 每次寫 code，如果有影響使用方式、設定、流程或文件內容，會一併檢查 `README.md` 是否需要同步修正
- 若修改 `docs/` 內文件，也會一起確認 `README.md` 是否需要補充入口或說明

## Planning Entry

後端主線 TODO：

- [00_backend_todo_list.md](d:/Git/dotNetWebAPI/docs/planning/00_backend_todo_list.md)

以下文件屬於其他 thread，暫不納入目前後端主線提交：

- [8_admin_web_implementation_spec_v1.md](d:/Git/dotNetWebAPI/docs/v1_1/8_admin_web_implementation_spec_v1.md)
- [9_admin_web_development_todo_v1.md](d:/Git/dotNetWebAPI/docs/v1_1/9_admin_web_development_todo_v1.md)
- [01_frontend_todo_list.md](d:/Git/dotNetWebAPI/docs/planning/01_frontend_todo_list.md)
