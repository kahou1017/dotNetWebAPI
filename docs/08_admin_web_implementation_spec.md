# Admin Web 實作規格

## 目的

這份文件定義 `Dimensions.Admin.Web` 的第一版實作方向。

它的角色很單純：

- 提供管理員登入畫面
- 提供 token 管理畫面
- 提供 device 管理畫面
- 提供管理查詢畫面

## 技術選型

`Dimensions.Admin.Web` 採用：

- ASP.NET Core MVC

選這個的原因：

- 適合管理後台
- 與整體 .NET solution 技術棧一致
- 新手比較容易上手
- 表單、查詢、清單、維護頁面都很適合

## 與其他專案的關係

### 呼叫方向

```text
Dimensions.Admin.Web
  -> Dimensions.Admin.Api
     -> Application / Domain / Infrastructure / Contracts
```

### 不直接呼叫的對象

`Dimensions.Admin.Web` 不應直接呼叫 `Dimensions.Api` 來做 token / device 管理。

管理操作應全部走：

- `Dimensions.Admin.Api`

## 第一版功能範圍

### 1. 登入

頁面：

- `/login`

功能：

- 管理員登入
- 驗證目前登入狀態
- 登出

### 2. Token 管理

頁面：

- `/tokens`
- `/tokens/create`
- `/tokens/{tokenId}`

功能：

- 查 token 清單
- 看 token 詳情
- 建立 token
- 撤銷 token
- 補發 token
- 續期 token

補充：

- `renew` 成功後，畫面應能取得新的 `accessToken`
- `DeviceId` 欄位目前先保留，不強制 client 端先提供
- 第一版可先建立「未綁定 device 的單裝置 token」
- 後續可由管理頁面從 usage log 挑選候選來源，再做手動綁定

### 3. Device 管理

頁面：

- `/devices`
- `/devices/create`

功能：

- 查 device 清單
- 建立 device
- 停用 device

補充：

- 停用 device 功能先保留
- 等正式啟用 device 綁定後，再完整套用綁定 token 的停用規則

### 4. Log 查詢

頁面：

- `/logs/token-usage`
- `/logs/token-action`

功能：

- 查 token 使用紀錄
- 查 token 操作紀錄

## UI 原則

### 原則 1：先把流程做順

第一版不要追求太多花俏效果，先把：

- 登入
- 查詢
- 建立
- 維護

這些主流程做穩。

### 原則 2：錯誤訊息要清楚

畫面應能顯示：

- `ErrorCode`
- `ErrorMessage`
- `caseId`

這樣開發與除錯都會比較容易。

### 原則 3：共用元件要簡單

建議先準備這些共用元件：

- `SearchPanel`
- `DataTable`
- `ErrorAlert`
- `EmptyState`
- `LoadingOverlay`
- `ConfirmDialog`

## Session 與登入狀態

第一版建議：

- 將 `AdminSession` 暫存在 `sessionStorage`
- 每次進入保護頁面時，呼叫 `GET /admin-api/auth/me` 驗證目前 session

## 給新手的例子

### 例子：建立一個給外部系統使用的 token

1. 管理員登入 `Admin.Web`
2. 進入 `/tokens/create`
3. 選擇 `Integration`
4. 填入使用者、名稱、有效時間
5. 送出後取得新的 token
6. 外部系統再拿這個 token 呼叫 `Dimensions.Api`

重點：

- `Admin.Web` 自己用的是 `AdminSession`
- 新建立的業務 token 不是 `Admin.Web` 自己的登入 token

## 第一版不做的事

- 複雜 dashboard
- 細粒度 RBAC
- token 內容編輯
- 高互動前端框架優化
- BFF / cookie session 架構

## 建議實作順序

1. 建 `Dimensions.Admin.Web` 專案
2. 建登入流程
3. 建 token 清單與建立頁
4. 建 token 詳情與操作流程
5. 建 device 清單與維護流程
6. 建 usage / action log 查詢
