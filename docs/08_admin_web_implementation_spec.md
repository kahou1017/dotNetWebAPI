# Admin Web 實作規格

## 目的

`Dimensions.Admin.Web` 是提供管理者使用的 MVC 後台，主要目的如下：

- 管理員登入與登出
- 管理 token
- 管理 device
- 查詢 request / exception 等系統紀錄

## 技術選型

- 專案名稱：`Dimensions.Admin.Web`
- 前端型態：`ASP.NET Core MVC`
- API 來源：`Dimensions.Admin.Api`
- Session 型態：伺服器端 Session

## 專案依賴方向

```text
Dimensions.Admin.Web
  -> Dimensions.Contracts
  -> Dimensions.Admin.Api (HTTP 呼叫)
```

說明：

- `Admin.Web` 不直接連資料庫
- `Admin.Web` 不直接呼叫 `Dimensions.Api`
- 所有管理功能都應透過 `Dimensions.Admin.Api`

## 目前已完成的 MVP

### 1. 登入流程

已完成：

- `/Account/Login`
- 提交登入表單至 `POST /admin-api/auth/login`
- 登入成功後建立 `AdminSession`
- 可登出並清除 Session

### 2. Dashboard 首頁

已完成：

- `/`
- 顯示登入者資訊
- 顯示快速入口

### 3. Token 管理

已完成：

- `/Tokens`
- `/Tokens/Create`
- `/tokens/{tokenId}`
- 撤銷 Token
- 補發 Token
- 續期 Token

### 4. Device 管理

已完成：

- `/Devices`
- `/Devices/Create`
- 停用 Device

### 5. Log 查詢

已完成：

- `/Logs/Requests`
- `/Logs/Exceptions`

## Session 規則

- `AdminSession` 只存在 `Dimensions.Admin.Web`
- Web 端將 `accessToken` 保存在伺服器端 Session
- 每次呼叫 `Dimensions.Admin.Api` 時，自動帶入 Bearer Token

## 畫面規則

第一版畫面以「先可用、再擴充」為主：

- 保留清楚的側邊選單
- 每頁都有一致的標題與錯誤區塊
- 支援空資料狀態
- 支援 `caseId` 顯示，方便對照 API log

## 後續建議

下一階段建議依序補強：

1. Token / Device / Log 的搜尋條件 UI
2. 共用元件
   - `SearchPanel`
   - `DataTable`
   - `ErrorAlert`
   - `EmptyState`
   - `ConfirmDialog`
3. 更完整的登入失效處理
4. Admin.Web 自己的 UI / integration tests
