# 可含或不含管理前端的 WebAPI 專案藍圖規格 V1.1

## 一、目標

建立一套具備以下特性的系統架構：

- JWT 驗證與管理
- Token 可查詢 / 建立 / 撤銷 / 重發 / 延長
- 支援 Device 綁定
- 支援 API 稽核與追蹤
- Admin UI 可插拔
- WebAPI 可獨立運作

---

## 二、核心設計原則

### 原則 1：後端核心獨立
WebAPI 必須在沒有任何前端時仍可完整運作。

### 原則 2：Token 管理完全 API 化
所有 Token 操作皆由 API 提供，不把核心邏輯放到 UI。

### 原則 3：前端是 Consumer，不是核心
Admin UI 僅呼叫 API、顯示資料、提供操作介面。

### 原則 4：安全規則全部在後端
JWT 驗章、Policy、Token 狀態、Device 綁定都由後端決定。

### 原則 5：管理員 Token 與業務 Token 必須可區分
V1.1 起，所有 JWT 至少應有 `TokenType`。

---

## 三、整體架構

### 方案 A（推薦）

```text
YourCompany.YourSystem.sln
├─ src
│  ├─ YourSystem.Api
│  ├─ YourSystem.Application
│  ├─ YourSystem.Domain
│  ├─ YourSystem.Infrastructure
│  ├─ YourSystem.Contracts
│  └─ YourSystem.AdminWeb   ← 可選
└─ tests
```

---

## 四、兩種運作模式

### 模式 1：Headless

系統只提供 API，不提供 UI。

使用方式：
- Postman / Bruno
- Swagger
- 內部系統串接

適合：
- 系統整合
- 後端先行開發
- 無 UI 維護需求的場景

### 模式 2：Admin UI

提供管理頁給管理員操作。

前端功能：
- 管理員登入
- Token 清單 / 明細
- Token 建立 / 撤銷 / 重發 / 延長
- 使用紀錄 / 操作紀錄
- Device 管理

適合：
- 維運人員操作
- 需要低門檻管理介面的場景

---

## 五、前端與後端的角色切分

### 前端負責

- 送出 API request
- 顯示 API response
- 顯示錯誤訊息與 `caseId`
- 儲存管理員登入後取得的 JWT

### 前端不負責

- Token 狀態判斷
- Device 驗證
- 權限判斷
- JWT 驗章

---

## 六、V1.1 Token 模型

V1.1 起，至少區分以下 Token 類型：

- `AdminSession`
- `UserAccess`
- `Integration`
- `Service`

### Admin UI 使用的 Token

Admin UI 只使用：
- `AdminSession`

### 業務 API 使用的 Token

視業務需求使用：
- `UserAccess`
- `Integration`
- `Service`

### 重要原則

- `AdminSession` 不應直接視為一般業務 Token
- 管理員登入成功後取得的是管理用途 JWT
- 受保護業務 API 是否接受 `AdminSession`，必須明確定義，不可預設混用

---

## 七、授權模型

V1.1 至少保留以下 Policy：

- `AdminOnly`
- `AuthenticatedUser`
- `TokenManage`

未來可擴充：

- `ModuleAccess:{module}`
- `Scope:{scope}`
- Role / RBAC

---

## 八、前端必要能力

- 儲存 JWT
- 自動附加：
  - `Authorization`
  - `X-Device-Id`
- 解析標準 `ApiResponse`
- 顯示 `caseId`
- 對 401 / 403 做統一處理

---

## 九、部署策略

### 無前端

- 只部署 API

### 有前端

- API 與 `AdminWeb` 可分開部署
- 或由同一站台提供

---

## 十、最終定案

- WebAPI 為核心
- `AdminWeb` 為可選模組
- Token 管理完全 API 化
- `TokenType` 為必要欄位
- 安全規則全部由後端處理
- 前端僅作為操作與顯示層

---

## 十一、建議實作順序

1. 完成 WebAPI 骨架
2. 完成共用 `ApiResponse`
3. 完成 `caseId` / Exception / Auth / Policy
4. 完成 Auth / Token / Device API
5. 補上 Log 與測試
6. 視需要加入 `AdminWeb`

---

## 十二、結論

此架構具備：

- 高彈性
- 低耦合
- 可 Headless 運作
- 可逐步加入前端
- 可從 V1 延伸到更完整的權限模型
