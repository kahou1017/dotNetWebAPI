# 前端開發 TODO 清單

## 開始前先看

建議先閱讀以下正式文件，再開始動手：

1. [00_master_index.md](../docs/00_master_index.md)
2. [02_dotnet_webapi_architecture_spec.md](../docs/02_dotnet_webapi_architecture_spec.md)
3. [08_admin_web_implementation_spec.md](../docs/08_admin_web_implementation_spec.md)
4. [09_project_structure_guide.md](../docs/09_project_structure_guide.md)

這份清單主要承接：

- `Dimensions.Admin.Web`
- ASP.NET Core MVC 管理前端
- 與 `Dimensions.Admin.Api` 的整合

## 適用範圍

這份清單主要給：

- `Dimensions.Admin.Web`

也就是管理後台前端。

## 使用方式

- `[ ]` 代表尚未完成
- `[x]` 代表已完成
- 完成後請補上：
  - `Completed: YYYY-MM-DD HH:mm`

## Priority 1

### 1. 專案建立

- [ ] 建立 `Dimensions.Admin.Web`
- [ ] 採用 ASP.NET Core MVC
- [ ] 加入 solution
- [ ] 建立 appsettings 與 Admin API Base URL

### 2. 登入流程

- [ ] `/login`
- [ ] 呼叫 `POST /admin-api/auth/login`
- [ ] 儲存 `AdminSession`
- [ ] 呼叫 `GET /admin-api/auth/me`
- [ ] 登出流程

### 3. App Shell

- [ ] layout
- [ ] side menu
- [ ] page header
- [ ] error / loading / empty state

## Priority 2

### 4. Token 管理

- [ ] `/tokens`
- [ ] `/tokens/create`
- [ ] `/tokens/{tokenId}`
- [ ] revoke dialog
- [ ] reissue dialog
- [ ] renew dialog

### 5. Device 管理

- [ ] `/devices`
- [ ] `/devices/create`
- [ ] disable dialog

### 6. Log 查詢

- [ ] `/logs/token-usage`
- [ ] `/logs/token-action`

## Priority 3

### 7. 共用元件

- [ ] `SearchPanel`
- [ ] `DataTable`
- [ ] `ErrorAlert`
- [ ] `EmptyState`
- [ ] `LoadingOverlay`
- [ ] `ConfirmDialog`

### 8. API Client

- [ ] GET / POST helper
- [ ] Bearer token header
- [ ] `ApiResponse<T>` 解析
- [ ] `401 / 403` 錯誤處理
- [ ] `caseId` 顯示

## 建議開發順序

1. 建專案
2. 做登入
3. 做 layout
4. 做 token 管理
5. 做 device 管理
6. 做 log 查詢
7. 最後整理共用元件

## 補充說明

- 這份清單應建立在 `Dimensions.Admin.Api` 邊界已定案之後
- 如果後端 API 還在調整中，建議先回去看 [00_backend_todo_list.md](00_backend_todo_list.md)
