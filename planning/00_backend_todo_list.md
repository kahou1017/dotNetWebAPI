# 後端開發 TODO 清單

## 開始前先看

建議先閱讀以下正式文件，再開始動手：

1. [00_master_index.md](../docs/00_master_index.md)
2. [02_dotnet_webapi_architecture_spec.md](../docs/02_dotnet_webapi_architecture_spec.md)
3. [03_api_jwt_endpoint_spec.md](../docs/03_api_jwt_endpoint_spec.md)
4. [09_project_structure_guide.md](../docs/09_project_structure_guide.md)

這份清單主要承接：

- 後端 API
- JWT / token / device 規則
- 資料存取
- logging
- 測試與 CI

## 使用方式

- `[ ]` 代表尚未完成
- `[x]` 代表已完成
- 完成後請補上時間：
  - `Completed: YYYY-MM-DD HH:mm`

## 目前已完成

- [x] `.NET 10` WebAPI solution 骨架
  Completed: 2026-03-20 21:12
- [x] `Dimensions.Api / Application / Domain / Infrastructure / Contracts / tests`
  Completed: 2026-03-20 21:12
- [x] `log4net`
  Completed: 2026-03-20 21:12
- [x] SQLite 開發模式
  Completed: 2026-03-20 21:12
- [x] JWT Bearer 驗證
  Completed: 2026-03-20 21:12
- [x] Swagger / OpenAPI
  Completed: 2026-03-20 21:12
- [x] 第一批 request validation
  Completed: 2026-03-20 21:12
- [x] 第二批 request validation
  Completed: 2026-03-20 22:00

## Priority 1

### 1. Token / Device 規則補強

- [ ] 補 `revoke / renew / reissue` 的限制條件
- [ ] 補 expired token 的驗證邏輯
- [ ] 補 revoked token 的驗證邏輯
- [ ] 補 disabled token 的驗證邏輯
- [ ] 補 single-device token 驗證細節
- [ ] 補 device 停用後的 token 行為
- [ ] 補 token 與 device / user 一致性規則

### 2. 管理 API 與業務 API 切分實作

- [ ] 建立 `Dimensions.Admin.Api`
- [ ] 將 admin login 移到 `Dimensions.Admin.Api`
- [ ] 將 `TokenController` 移到 `Dimensions.Admin.Api`
- [ ] 將 `DeviceController` 移到 `Dimensions.Admin.Api`
- [ ] 調整 `Dimensions.Api` 只保留業務 API
- [ ] 調整 route 與 Swagger

## Priority 2

### 3. Backend Business API 擴充

- [ ] 定義第一個正式業務模組
- [ ] 補 request / response contract
- [ ] 補 service / repository / controller flow
- [ ] 補 `AuthenticatedUser` 權限驗證
- [ ] 規劃 scope 規則

### 4. Logging 完整化

- [ ] 補 `ActionLog / UsageLog / ExceptionLog / PayloadLog`
- [ ] 寫入 DB
- [ ] 補 sensitive data masking 規則

## Priority 3

### 5. 開發體驗

- [ ] 補 `.http` 測試案例
- [ ] 規劃 Postman collection
- [ ] 規劃 Bruno collection
- [ ] 更新 README

### 6. 測試策略

- [ ] `Admin AuthController` integration test
- [ ] `Dimensions.Api` token validation / business API flow test
- [ ] SQLite integration test baseline

## Priority 4

### 7. 交付與 CI

- [ ] branch / release flow
- [ ] CI build pipeline
- [ ] `develop -> main` 流程
- [ ] secrets / environment config strategy

## 建議開發順序

1. Token / Device 規則補強
2. Admin API 切分
3. Business API 擴充
4. Logging 完整化
5. 測試
6. CI / release

## 補充說明

- 如果不確定目前應先做後端還是前端，預設先做這份
- `Dimensions.Admin.Web` 的前端工作請看 [01_frontend_todo_list.md](01_frontend_todo_list.md)
