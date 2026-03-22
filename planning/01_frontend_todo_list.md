# 前端 TODO 清單

## 開始前先看
建議先依序閱讀：
1. [../docs/00_master_index.md](../docs/00_master_index.md)
2. [../docs/02_dotnet_webapi_architecture_spec.md](../docs/02_dotnet_webapi_architecture_spec.md)
3. [../docs/08_admin_web_implementation_spec.md](../docs/08_admin_web_implementation_spec.md)
4. [../docs/09_project_structure_guide.md](../docs/09_project_structure_guide.md)

這份清單聚焦於：
- `Dimensions.Admin.Web`
- ASP.NET Core MVC 管理後台
- 透過 `Dimensions.Admin.Api` 呼叫管理 API

## 狀態規則
- `[ ]` 未完成
- `[x]` 已完成
- 完成後補上時間：
  - `Completed: YYYY-MM-DD HH:mm`

## Priority 1

### 1. 專案初始化

- [x] 建立 `Dimensions.Admin.Web`
  - `Completed: 2026-03-22 09:00`
- [x] 採用 ASP.NET Core MVC
  - `Completed: 2026-03-22 09:00`
- [x] 納入 `Dimensions.sln`
  - `Completed: 2026-03-22 09:00`
- [x] 設定 `AdminApi:BaseUrl`
  - `Completed: 2026-03-22 09:00`

### 2. 登入流程

- [x] `/Account/Login`
  - `Completed: 2026-03-22 09:00`
- [x] 串接 `POST /admin-api/auth/login`
  - `Completed: 2026-03-22 09:00`
- [x] 建立 `AdminSession`
  - `Completed: 2026-03-22 09:00`
- [x] 登入後導頁
  - `Completed: 2026-03-22 09:00`

### 3. App Shell

- [x] Layout
  - `Completed: 2026-03-22 09:20`
- [x] Side menu
  - `Completed: 2026-03-22 09:20`
- [x] Page header
  - `Completed: 2026-03-22 09:20`
- [x] 基本 empty / error state
  - `Completed: 2026-03-22 09:20`

## Priority 2

### 4. Token 管理

- [x] `/Tokens`
  - `Completed: 2026-03-22 09:20`
- [x] `/Tokens/Create`
  - `Completed: 2026-03-22 09:20`
- [x] `/tokens/{tokenId}`
  - `Completed: 2026-03-22 09:20`
- [x] revoke 流程
  - `Completed: 2026-03-22 09:20`
- [x] reissue 流程
  - `Completed: 2026-03-22 09:20`
- [x] renew 流程
  - `Completed: 2026-03-22 09:20`

### 5. Device 管理

- [x] `/Devices`
  - `Completed: 2026-03-22 09:20`
- [x] `/Devices/Create`
  - `Completed: 2026-03-22 09:20`
- [x] disable 流程
  - `Completed: 2026-03-22 09:20`

### 6. Log 查詢

- [x] `/Logs/Requests`
  - `Completed: 2026-03-22 09:20`
- [x] `/Logs/Exceptions`
  - `Completed: 2026-03-22 09:20`

## Priority 3

### 7. 共用 UI 元件

- [x] `SearchPanel`
  - `Completed: 2026-03-22 14:10`
- [x] `DataTable`
  - `Completed: 2026-03-22 14:10`
- [x] `ErrorAlert`
  - `Completed: 2026-03-22 13:05`
- [x] `EmptyState`
  - `Completed: 2026-03-22 13:05`
- [ ] `LoadingOverlay`
- [ ] `ConfirmDialog`

### 8. API Client 強化

- [x] GET / POST helper
  - `Completed: 2026-03-22 09:00`
- [x] Bearer token header
  - `Completed: 2026-03-22 09:00`
- [x] `ApiResponse<T>` 解析
  - `Completed: 2026-03-22 09:00`
- [x] `401 / 403` 統一處理
  - `Completed: 2026-03-22 13:05`
- [x] `caseId` 顯示
  - `Completed: 2026-03-22 09:20`

## 下一步建議
1. 補上 Token / Device / Log 頁面的操作確認體驗
2. 加入 `LoadingOverlay`
3. 補 `ConfirmDialog`
4. 規劃 `Dimensions.Admin.Web` 的 UI / integration tests
