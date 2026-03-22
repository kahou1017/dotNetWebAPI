# API 開發指南

## 目標

這份文件整理目前在專案內最實用的 API 開發方式。
如果你是新加入的開發者，可以先用這份快速進入狀況。

建議先閱讀：
1. [00_master_index.md](00_master_index.md)
2. [02_dotnet_webapi_architecture_spec.md](02_dotnet_webapi_architecture_spec.md)
3. [09_project_structure_guide.md](09_project_structure_guide.md)

## 最方便的開發流程

### 1. Restore

```powershell
dotnet restore .\Dimensions.sln
```

### 2. 啟動業務 API

```powershell
.\scripts\run-business-api.ps1
```

### 3. 啟動管理 API

```powershell
.\scripts\run-admin-api.ps1
```

### 4. 啟動管理後台 Web

```powershell
dotnet run --project .\src\Dimensions.Admin.Web
```

### 5. 使用 `.http` 測試檔

- 管理 API：
  - [Dimensions.Admin.Api.http](../src/Dimensions.Admin.Api/Dimensions.Admin.Api.http)
- 業務 API：
  - [Dimensions.Api.http](../src/Dimensions.Api/Dimensions.Api.http)

目前 `.http` 已整理成可直接照順序驗證的流程：
- admin login / admin me
- create token / token detail / token list
- token usage / token action log
- request log / exception log 查詢
- customer query 成功情境
- customer not found 錯誤情境
- scope 不足 `403` 情境

### 6. 管理後台 Smoke Test

```powershell
.\scripts\smoke-test-admin-web.ps1
```

這支腳本會自動：
- 啟動 `Dimensions.Admin.Api`
- 啟動 `Dimensions.Admin.Web`
- 打開登入頁並帶入 anti-forgery token
- 驗證登入、Dashboard、Token Detail 頁面
- 確認 `LoadingOverlay` 與 `ConfirmDialog` 已載入

### 7. 開發驗證與回歸

```powershell
.\scripts\build-and-test.ps1
```

目前建議固定照這個順序驗證：
1. `dotnet build`
2. `dotnet test`

## 不同類型功能該改哪裡

### 業務 API

例如：
- `/api/customer/query`
- 未來的 `/api/order/create`

通常會修改：
1. `Dimensions.Contracts`
2. `Dimensions.Application`
3. `Dimensions.Infrastructure`
4. `Dimensions.Api`
5. `tests/Dimensions.Api.Tests`

### 管理 API

例如：
- token 查詢
- device 停用
- request / exception log 查詢

通常會修改：
1. `Dimensions.Contracts`
2. `Dimensions.Application`
3. `Dimensions.Infrastructure`
4. `Dimensions.Admin.Api`
5. `tests/Dimensions.Admin.Api.Tests`

### 資料表或 SQLite 初始化

通常會修改：
1. `Dimensions.Application/Interfaces`
2. `Dimensions.Application/Models`
3. `Dimensions.Infrastructure/Persistence`
4. `DatabaseInitializer`

## 開發一支新 API 的建議步驟

### 業務 API

1. 在 `Dimensions.Contracts` 建 request / response
2. 在 `Dimensions.Application/Interfaces` 補 service / repository 介面
3. 在 `Dimensions.Application/Services` 補業務邏輯
4. 在 `Dimensions.Infrastructure/Persistence/Repositories` 補資料存取
5. 在 `Dimensions.Api/Controllers` 開 endpoint
6. 在 `Dimensions.Api/Validation` 補 validator
7. 在 `tests/Dimensions.Api.Tests` 補 integration test
8. 視情況同步更新 `README.md`

### 管理 API

1. 在 `Dimensions.Contracts` 建 request / response
2. 在 `Dimensions.Application` 補 service 邏輯
3. 在 `Dimensions.Infrastructure` 補 repository
4. 在 `Dimensions.Admin.Api/Controllers` 開 endpoint
5. 在 `Dimensions.Admin.Api/Validation` 補 validator
6. 在 `tests/Dimensions.Admin.Api.Tests` 補 integration test
7. 視情況同步更新 `README.md`

## 新手最常改到的目錄

### 業務 API

- `src/Dimensions.Api/Controllers`
- `src/Dimensions.Api/Validation`
- `src/Dimensions.Application/Services`
- `src/Dimensions.Application/Interfaces`
- `src/Dimensions.Infrastructure/Persistence/Repositories`
- `src/Dimensions.Contracts`
- `tests/Dimensions.Api.Tests`

### 管理 API

- `src/Dimensions.Admin.Api/Controllers`
- `src/Dimensions.Admin.Api/Validation`
- `src/Dimensions.Application/Services`
- `src/Dimensions.Application/Interfaces`
- `src/Dimensions.Infrastructure/Persistence/Repositories`
- `src/Dimensions.Contracts`
- `tests/Dimensions.Admin.Api.Tests`

### 管理後台 Web

- `src/Dimensions.Admin.Web/Controllers`
- `src/Dimensions.Admin.Web/Views`
- `src/Dimensions.Admin.Web/Models`
- `src/Dimensions.Admin.Web/Services`
- `src/Dimensions.Admin.Web/wwwroot`

## 目前最常測的流程

### 管理 API

1. `POST /admin-api/auth/login`
2. `GET /admin-api/auth/me`
3. `POST /admin-api/token/create`
4. `POST /admin-api/log/request/list`
5. `POST /admin-api/log/exception/list`
6. 補發 / 續期 / 撤銷 token

### 業務 API

1. 先由 `Admin.Api` 建 token
2. 再呼叫 `/api/customer/query`
3. 驗證 request / exception / payload log 是否被記錄

### 管理後台 Web

1. 開啟登入頁
2. 以 `admin / admin` 登入
3. 查看 Dashboard
4. 查看 Token 清單與 Token Detail
5. 確認 `LoadingOverlay`、`ConfirmDialog` 已載入

## 最方便的工具組合

- 啟動 API：`.\scripts\run-business-api.ps1` / `.\scripts\run-admin-api.ps1`
- 測 API：`.http`
- 驗證：`.\scripts\build-and-test.ps1`
- 驗證管理後台：`.\scripts\smoke-test-admin-web.ps1`

## 建議的閱讀順序

1. 先看 [09_project_structure_guide.md](09_project_structure_guide.md)
2. 再看你要改的 API 或 Web
3. 再跑 `.http` 或 smoke test
4. 最後跑 `.\scripts\build-and-test.ps1`
