# dotNetWebAPI

`Dimensions` 的 `.NET WebAPI` 開發中專案。

目前這個 repo 以 `develop` 為主要開發分支，`main` 暫不更新，避免把開發中的骨架直接推到正式主線。

## 目前狀態

- 已建立 `Api / Application / Domain / Infrastructure / Contracts / tests` 分層骨架
- 已接上 `log4net` 本機檔案 log
- 已接上真實 `JWT Bearer` 驗章與基本 authorization policy
- 已接上第一批 request validation
- Development 環境預設使用 `SQLite`
- 啟動時會自動建立本機資料庫與 seed 資料
- 已有 `Auth / Token / Device / Customer / Public` API 骨架
- 已提供 Swagger / OpenAPI

## Solution 結構

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

## 開發環境

- `.NET 10`
- ASP.NET Core Web API
- SQLite
- Dapper
- FluentValidation
- log4net

## 本機啟動

1. 還原套件

```powershell
dotnet restore .\Dimensions.slnx
```

2. 編譯

```powershell
dotnet build .\Dimensions.slnx --no-restore
```

3. 啟動 API

```powershell
dotnet run --project .\src\Dimensions.Api
```

4. 開啟 Swagger

啟動後可直接在瀏覽器打開：

- `http://localhost:<port>/swagger`

若要測試受保護 API，可先透過 `POST /api/auth/login` 取得 JWT，再於 Swagger 右上角 `Authorize` 輸入：

```text
Bearer {token}
```

## 開發模式資料庫

Development 設定在 [appsettings.Development.json](d:/Git/dotNetWebAPI/src/Dimensions.Api/appsettings.Development.json)：

- `Provider`: `Sqlite`
- `ConnectionStrings:DefaultConnection`: `Data Source=dimensions-dev.db;Cache=Shared;Foreign Keys=True`

啟動 API 後，SQLite 檔案會建立在：

- [dimensions-dev.db](d:/Git/dotNetWebAPI/src/Dimensions.Api/dimensions-dev.db)

## 預設 seed 帳號

目前開發用 seed 管理員帳號：

- `admin / admin`
- `admin2 / admin2`

## 主要 API

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

## 目前驗證規則

- `POST /api/auth/login` 會簽發真實 JWT bearer token
- 受保護 API 會進行 JWT 驗章，並回查 DB token 狀態
- 若 token 為單裝置模式，request 需帶 `X-Device-Id`
- 已驗證成功的 token 請求會寫入 usage log，並更新 token 的 `LastUsedAt`
- `POST /api/customer/query` 可用來測試一般使用者 token 的受保護業務流程
- 第一批 request validation 已套用在：
  - `LoginRequest`
  - `CreateTokenRequest`
  - `ReissueTokenRequest`
  - `CreateDeviceRequest`
  - `CustomerQueryRequest`

## Logging

- API 使用 `log4net`
- 設定檔在 [log4net.config](d:/Git/dotNetWebAPI/src/Dimensions.Api/log4net.config)
- 本機 log 會寫入 `Logs/`

## Swagger / OpenAPI

- 開發環境可使用 `/swagger`
- 已支援 JWT Bearer `Authorize` 按鈕
- 可直接測試 `Auth / Token / Device / Customer` API

## VS Code

專案已提供 VS Code 外掛建議：

- [extensions.json](d:/Git/dotNetWebAPI/.vscode/extensions.json)

其中包含 SQLite 檢視外掛，方便直接查看本機 `.db` 檔。

## Git 分支說明

- `develop`: 目前開發主線
- `main`: 暫不更新

目前所有開發中的變更都先以 `develop` 為主，確認穩定後再考慮是否整理進 `main`。

## 文件維護約定

- 每次開發若有調整程式碼，也要同步檢查相關文件是否需要更新
- 若本次變更有影響專案使用方式、結構、設定、啟動流程或開發規則，也要一併確認 `README.md` 是否需要修正
- 文件更新以 `docs/` 與 `README.md` 一起維護，避免程式與說明脫節

## 規劃入口

- 後端主線 TODO：
  - [00_backend_todo_list.md](d:/Git/dotNetWebAPI/docs/planning/00_backend_todo_list.md)

以下文件屬於其他 thread，目前不納入後端主線提交：

- [8_admin_web_implementation_spec_v1.md](d:/Git/dotNetWebAPI/docs/v1_1/8_admin_web_implementation_spec_v1.md)
- [9_admin_web_development_todo_v1.md](d:/Git/dotNetWebAPI/docs/v1_1/9_admin_web_development_todo_v1.md)
- [01_frontend_todo_list.md](d:/Git/dotNetWebAPI/docs/planning/01_frontend_todo_list.md)

## 後續方向

- 補強 token/device 驗證規則
- 補 usage/action/exception/payload log 一致性
- 擴充正式 business API module
- 補更多 request validation
- 補更多整合測試
- 規劃部署與 CI
