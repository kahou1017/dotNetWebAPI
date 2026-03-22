# API 開發實作指南

## 目的

這份文件不是講整體架構，而是講「實際要怎麼開始開發」。

如果你剛加入專案，建議先看：

1. [00_master_index.md](00_master_index.md)
2. [02_dotnet_webapi_architecture_spec.md](02_dotnet_webapi_architecture_spec.md)
3. [09_project_structure_guide.md](09_project_structure_guide.md)

看完之後，再回來照這份開始動手。

## 最推薦的本機開發方式

### 1. 先 restore

```powershell
dotnet restore .\Dimensions.sln
```

### 2. 啟動你要改的 API

如果你在改業務 API：

```powershell
.\scripts\run-business-api.ps1
```

如果你在改管理 API：

```powershell
.\scripts\run-admin-api.ps1
```

### 3. 用 `.http` 直接測

- 管理 API：
  - [Dimensions.Admin.Api.http](../src/Dimensions.Admin.Api/Dimensions.Admin.Api.http)
- 業務 API：
  - [Dimensions.Api.http](../src/Dimensions.Api/Dimensions.Api.http)

這是目前最快的手動驗證方式。

目前 `.http` 已整理的情境包含：

- admin login / admin me
- create token / token detail / token list
- token usage / token action log
- request log / exception log 查詢
- customer query 成功情境
- customer not found 錯誤情境
- scope 不足的 `403` 情境

### 4. 開發完成後固定執行

```powershell
.\scripts\build-and-test.ps1
```

這支腳本會固定照以下順序跑：

1. `dotnet build`
2. `dotnet test`

這也是目前專案約定的驗證順序。

## 你要改哪一個 project

### 情境 1：新增業務 API

例如：

- `/api/customer/query`
- 未來的 `/api/order/create`

通常會改到：

1. `Dimensions.Contracts`
2. `Dimensions.Application`
3. `Dimensions.Infrastructure`
4. `Dimensions.Api`
5. `tests/Dimensions.Api.Tests`

### 情境 2：新增管理 API

例如：

- token 查詢
- device 停用
- request log 查詢

通常會改到：

1. `Dimensions.Contracts`
2. `Dimensions.Application`
3. `Dimensions.Infrastructure`
4. `Dimensions.Admin.Api`
5. `tests/Dimensions.Admin.Api.Tests`

### 情境 3：只改資料查詢或 SQLite schema

通常會改到：

1. `Dimensions.Application/Interfaces`
2. `Dimensions.Application/Models`
3. `Dimensions.Infrastructure/Persistence`
4. `DatabaseInitializer`

## 新增一支 API 的標準步驟

### 業務 API

建議順序：

1. 在 `Dimensions.Contracts` 新增 request / response
2. 在 `Dimensions.Application/Interfaces` 定義 service 或 repository 需求
3. 在 `Dimensions.Application/Services` 補流程
4. 在 `Dimensions.Infrastructure/Persistence/Repositories` 補資料存取
5. 在 `Dimensions.Api/Controllers` 新增 endpoint
6. 在 `Dimensions.Api/Validation` 補 validator
7. 在 `tests/Dimensions.Api.Tests` 補 integration test
8. 更新 `README.md` 與必要文件

### 管理 API

建議順序：

1. 在 `Dimensions.Contracts` 新增 request / response
2. 在 `Dimensions.Application` 補 service 流程
3. 在 `Dimensions.Infrastructure` 補 repository
4. 在 `Dimensions.Admin.Api/Controllers` 新增 endpoint
5. 在 `Dimensions.Admin.Api/Validation` 補 validator
6. 在 `tests/Dimensions.Admin.Api.Tests` 補 integration test
7. 更新 `README.md` 與必要文件

## 哪些檔案是最常碰的

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

## 手動測試建議順序

### 管理 API

1. `POST /admin-api/auth/login`
2. `GET /admin-api/auth/me`
3. `POST /admin-api/token/create`
4. `POST /admin-api/log/request/list`
5. `POST /admin-api/log/exception/list`
6. 視需要再測 `reissue / renew / revoke`

### 業務 API

1. 先從 `Admin.Api` 建 token
2. 再呼叫 `/api/customer/query`
3. 檢查 request / exception / payload log 是否有寫入

## 新手最容易卡住的地方

### 1. 把 controller 寫太重

建議：

- controller 只做收參數、呼叫 service、回 response

### 2. 把 request / response model 放錯地方

建議：

- request / response 放 `Dimensions.Contracts`
- application 內部流程模型才放 `Dimensions.Application/Models`

### 3. 改完 code 沒補測試

建議：

- 至少補 integration test
- 如果是業務規則，再看要不要補 application test

### 4. 改完文件沒同步

建議：

- 只要有影響使用方式、路由、設定、流程，就一起檢查 `README.md`

## 目前最方便的開發工具組合

- 啟動 API：`.\scripts\run-business-api.ps1` / `.\scripts\run-admin-api.ps1`
- 手動測試：`.http`
- 驗證：`.\scripts\build-and-test.ps1`
- 文件入口：
  - [00_master_index.md](00_master_index.md)
  - [09_project_structure_guide.md](09_project_structure_guide.md)
  - [planning/README.md](../planning/README.md)

## 一句話版本

如果你只想先快速開始：

1. 先看 [09_project_structure_guide.md](09_project_structure_guide.md)
2. 啟動對應 API
3. 用 `.http` 測
4. 改完跑 `.\scripts\build-and-test.ps1`
