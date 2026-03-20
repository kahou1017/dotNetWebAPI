# Dimensions Backend TODO List

> Purpose:
> 這份文件只整理目前後端主線的待辦、優先順序、依賴關係與建議執行路徑。
> 不包含 Admin Web 相關內容，避免和其他 thread 混在一起。

---

## Checklist Rules

- 已完成項目使用 `[x]`
- 未完成項目使用 `[ ]`
- 每個完成項目後面補上 `Completed: yyyy-MM-dd HH:mm:ss zzz`
- 若是補記先前已完成事項，可在同一行註明 `Recorded` 時間

---

## Current Status

以下項目已完成，先統一補記目前整理時間：

- [x] `.NET 10` WebAPI solution skeleton
  Completed: 2026-03-20 21:12:24 +08:00
- [x] `Api / Application / Domain / Infrastructure / Contracts / tests` 分層
  Completed: 2026-03-20 21:12:24 +08:00
- [x] `log4net` 本機檔案 logging
  Completed: 2026-03-20 21:12:24 +08:00
- [x] SQLite 開發模式
  Completed: 2026-03-20 21:12:24 +08:00
- [x] SQLite 自動建表與 seed
  Completed: 2026-03-20 21:12:24 +08:00
- [x] 真實 JWT Bearer 驗章
  Completed: 2026-03-20 21:12:24 +08:00
- [x] Admin login / token management / device management API skeleton
  Completed: 2026-03-20 21:12:24 +08:00
- [x] usage log / action log / `LastUsedAt`
  Completed: 2026-03-20 21:12:24 +08:00
- [x] `AuthenticatedUser` business demo API：`POST /api/customer/query`
  Completed: 2026-03-20 21:12:24 +08:00
- [x] Swagger / OpenAPI
  Completed: 2026-03-20 21:12:24 +08:00

---

## Priority 1

### 1. Request Validation

目標：
- 為後端 API 的 request model 補 validation
- 統一 validation error 回傳格式

Checklist:
- [x] 決定是否導入 `FluentValidation`
  Completed: 2026-03-20 21:12:24 +08:00
- [x] 為 `LoginRequest` 建 validator
  Completed: 2026-03-20 21:12:24 +08:00
- [x] 為 `CreateTokenRequest` 建 validator
  Completed: 2026-03-20 21:12:24 +08:00
- [x] 為 `ReissueTokenRequest` 建 validator
  Completed: 2026-03-20 21:12:24 +08:00
- [x] 為 `CreateDeviceRequest` 建 validator
  Completed: 2026-03-20 21:12:24 +08:00
- [x] 為 `CustomerQueryRequest` 建 validator
  Completed: 2026-03-20 21:12:24 +08:00
- [x] 將 validation failure 整合進既有 exception / response pipeline
  Completed: 2026-03-20 21:12:24 +08:00
- [x] 規劃第二批 request validators
  Completed: 2026-03-20 22:00:00 +08:00

原因：
- 現在 API 可跑，但輸入邊界仍偏鬆
- 越早補 validation，後面擴 API 越穩

---

### 2. Token / Device Rule Hardening

目標：
- 把目前偏 skeleton 的安全規則收緊

Checklist:
- [ ] 補 `revoke / renew / reissue` 前置條件檢查
- [ ] 補 expired token 一致性處理
- [ ] 補 revoked token 一致性處理
- [ ] 補 disabled token 一致性處理
- [ ] 定義 single-device token 換綁規則
- [ ] 定義 device disable 後 token 的反應規則
- [ ] 收斂 token 與 device / user 關聯規則

原因：
- 目前是「可運作」
- 還不是「可放心擴張」

---

## Priority 2

### 3. Backend Business API Expansion

目標：
- 從 demo 型 `customer/query` 長成正式後端 business module

候選方向：
- `Customer`
- `Order`
- `Account`
- 或實際要對接的舊系統 / ASMX 模組

Checklist:
- [ ] 定義第一個正式 backend module
- [ ] 定義 module boundary
- [ ] 補 request / response contract
- [ ] 建 repository / service / controller flow
- [ ] 定義哪些 API 用 `AuthenticatedUser`
- [ ] 定義哪些 API 需要更細的 scope

---

### 4. Logging Completeness

目標：
- 讓後端 log 具備更完整追蹤性

Checklist:
- [ ] 收斂 `ActionLog / UsageLog / ExceptionLog / PayloadLog` 欄位一致性
- [ ] 定義哪些資料寫 DB
- [ ] 定義哪些資料只寫 `log4net`
- [ ] 補 sensitive data masking 規則

---

## Priority 3

### 5. Backend Developer Experience

目標：
- 讓後端開發與測試更快

Checklist:
- [ ] 補更多 `.http` 測試案例
- [ ] 規劃 Postman collection
- [ ] 規劃 Bruno collection
- [ ] 補 Swagger 說明內容
- [ ] 持續同步 README

---

### 6. Test Strategy

目標：
- 讓目前後端 skeleton 開始有實際測試保護

Checklist:
- [ ] 補 `AuthController` integration test
- [ ] 補 token create / usage / customer query flow test
- [ ] 補 SQLite integration test baseline

---

## Priority 4

### 7. Deployment / CI Preparation

目標：
- 為後端穩定交付做準備

Checklist:
- [ ] 明文化 branch / release flow
- [ ] 建立 CI build pipeline
- [ ] 定義 `develop -> main` 發布規則
- [ ] 定義 secrets / environment configuration strategy

---

## Recommended Execution Order

建議依這個順序往下做：

1. Request Validation
2. Token / Device Rule Hardening
3. Backend Business API Expansion
4. Logging Completeness
5. Test Strategy
6. Deployment / CI Preparation

---

## Pause Notes

目前有已知分工：

- [ ] `docs/v1_1/8_admin_web_implementation_spec_v1.md`
  Note: 屬於另一個 thread，不修改，不納入 commit

- [ ] `docs/v1_1/9_admin_web_development_todo_v1.md`
  Note: 屬於 Admin Web 規劃，不修改，不納入 commit

- [ ] `docs/planning/01_frontend_todo_list.md`
  Note: 屬於 frontend thread，不修改，不納入 commit

---

## Next Recommended Task

如果現在重新開工，最建議直接做：

- [ ] Token / Device Rule Hardening

建議先做：
- `revoke / renew / reissue` 前置條件檢查
- expired / revoked / disabled token 一致性處理
- single-device token 換綁規則

原因：
- Request Validation 第一批已完成
- 下一步最值得補的是安全規則完整性
