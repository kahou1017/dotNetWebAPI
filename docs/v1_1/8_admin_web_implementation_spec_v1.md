# AdminWeb 前端實作規格 V1.0

> 這份文件屬於 `docs/v1_1/` 舊版素材。
>
> 目前正式文件請改看：
> - [docs/08_admin_web_implementation_spec.md](../08_admin_web_implementation_spec.md)
>
> 補充說明：
> - 這份舊版內容以 `Blazor Web App` 為方向
> - 目前正式主線已改為 `ASP.NET Core MVC`
> - 如需了解目前開發與閱讀順序，請從 [docs/00_master_index.md](../00_master_index.md) 開始

## 一、文件目的

本文件定義 `Dimensions.AdminWeb` 的第一版實作範圍，目標是提供一個可操作的管理前端，讓管理者能透過瀏覽器完成：

- 管理者登入與 session 驗證
- Token 查詢、建立、撤銷、補發、續期
- Device 查詢、建立、停用
- Token 使用紀錄與操作紀錄查詢

本文件聚焦於前端的資訊架構、頁面流程、狀態管理、API 對接方式與安全邊界，不重複定義後端 JWT 規格與資料表細節。

關聯文件：

- `1_webapi_architecture_with_optional_admin_ui.md`
- `3_api_jwt_endpoint_spec_v1.md`
- `src/Dimensions.Contracts/*`

---

## 二、版本目標

### V1 目標

- 建立可登入的 Admin UI
- 讓管理者可直接透過 UI 操作既有 Token / Device API
- 以最少頁面完成主要管理工作
- 不額外改動既有 API contract 即可上線

### V1 非目標

- 不做 RBAC 細粒度角色管理
- 不做 dashboard 圖表
- 不做通知中心
- 不做批次匯入匯出
- 不做多語系
- 不做複雜主題切換

---

## 三、技術方向

### 建議前端專案型態

新增專案：

```text
src/Dimensions.AdminWeb
```

建議採用：

- `.NET 10`
- `ASP.NET Core Blazor Web App`

### 採用 Blazor 的原因

- 與現有 solution 同技術棧，維護成本低
- 可共用 `Dimensions.Contracts` 型別或至少共用欄位設計
- 部署與建置流程可維持在 .NET toolchain
- 適合管理後台型 CRUD / 查詢介面

### V1 架構原則

- 前端只負責操作流程、畫面呈現、狀態管理
- 所有授權判斷仍以 API 為準
- 前端不自行推導 token 是否有效，僅依 API 回應更新狀態
- 所有 API 存取統一經過同一層 API Client

---

## 四、資訊架構

### 導覽結構

```text
/login
/tokens
/tokens/create
/tokens/{tokenId}
/devices
/devices/create
/logs/token-usage
/logs/token-action
```

### 主選單

- Token 管理
- Device 管理
- 使用紀錄
- 操作紀錄
- 目前登入者資訊
- 登出

### 預設登入後首頁

登入成功後導向：

```text
/tokens
```

原因：

- Token 管理是 Admin UI 的核心功能
- Token list 也是最常見的營運入口

---

## 五、頁面規格

## 5.1 登入頁 `/login`

### 目的

讓管理者取得 `AdminSession` token，並建立前端登入狀態。

### 畫面元素

- `loginAccount` 輸入框
- `password` 輸入框
- 登入按鈕
- 錯誤訊息區塊
- 系統名稱 / 簡短說明

### API

- `POST /api/auth/login`

### 成功後行為

- 儲存 `accessToken`
- 儲存最小必要登入資訊：
  - `tokenType`
  - `tokenId`
  - `expireAt`
  - `userId`
  - `displayName`
- 成功後以 `GET /api/auth/me` 補齊：
  - `loginAccount`
  - `lastLoginAt`
- 導向 `/tokens`

### 失敗後行為

- 顯示 `errorCode` 與 `message`
- 若有 `caseId`，一併顯示給使用者

### 驗證規則

- `loginAccount` 必填
- `password` 必填
- 送出時避免重複點擊

---

## 5.2 Token 列表頁 `/tokens`

### 目的

提供管理者查詢與篩選 token，並作為後續詳情與操作入口。

### 查詢欄位

- `tokenType`
- `userId`
- `tokenId`
- `status`
- `isSingleDevice`
- `deviceId`
- `issuedAtStart`
- `issuedAtEnd`
- `expireAtStart`
- `expireAtEnd`
- `pageNo`
- `pageSize`

### 列表欄位

- `tokenId`
- `tokenType`
- `userId`
- `userName`
- `tokenName`
- `status`
- `isSingleDevice`
- `deviceId`
- `issuedAt`
- `effectiveAt`
- `expireAt`
- `lastUsedAt`
- `createdBy`

### 列表操作

- 查看詳情
- 前往建立新 token
- 對單筆 token 執行：
  - revoke
  - reissue
  - renew

### API

- `POST /api/token/list`

### 頁面狀態

- 初次載入自動查詢第一頁
- 查詢條件變更後由使用者按下搜尋才送出
- 保留查詢條件於頁面狀態，切回列表時不遺失

---

## 5.3 Token 建立頁 `/tokens/create`

### 目的

建立新的 `UserAccess` / `Integration` / `Service` token。

### 表單欄位

- `tokenType`
- `userId`
- `userName`
- `tokenName`
- `isSingleDevice`
- `deviceId`
- `deviceName`
- `effectiveAt`
- `expireAt`
- `isPermanent`
- `canReissue`
- `canRenew`
- `purpose`
- `remark`

### 表單規則

- 不允許建立 `AdminSession`
- `tokenType` 必填
- `userId` 必填
- `userName` 必填
- `tokenName` 必填
- `effectiveAt` 必填
- 若 `isPermanent = false`，則 `expireAt` 必填
- 若 `isSingleDevice = true`，則 `deviceId` 必填
- 若 `isSingleDevice = true` 且使用者手動輸入新 device，可同時要求 `deviceName`

### API

- `POST /api/token/create`

### 成功後行為

- 顯示建立成功結果
- 明確顯示新產生的 `accessToken`
- 提示此 token 為敏感資料，需立即複製保存
- 可提供兩個後續操作：
  - 返回 token 列表
  - 前往 token 詳情

### 備註

前端不得將新產生的業務 token 視為自己的登入 token；該 token 僅供管理者複製或查看。

---

## 5.4 Token 詳情頁 `/tokens/{tokenId}`

### 目的

顯示單一 token 的完整資訊，並提供進一步操作。

### API

- `POST /api/token/detail`

### 顯示欄位

- `tokenId`
- `jwtId`
- `tokenType`
- `tokenName`
- `userId`
- `userName`
- `status`
- `isRevoked`
- `isSingleDevice`
- `isEnabled`
- `canReissue`
- `canRenew`
- `deviceId`
- `deviceName`
- `issuedAt`
- `effectiveAt`
- `expireAt`
- `lastUsedAt`
- `purpose`
- `remark`
- `createdBy`
- `createdAt`

### 操作

- revoke
- reissue
- renew
- 查看 usage log
- 查看 action log

### 畫面區塊建議

- 基本資料
- 生效資訊
- 裝置限制
- 管理操作
- 相關紀錄捷徑

---

## 5.5 Token 撤銷流程

### 觸發點

- Token 列表頁
- Token 詳情頁

### UI 形式

- 確認對話框
- 欄位：`reason`

### API

- `POST /api/token/revoke`

### 成功後行為

- 關閉對話框
- 顯示成功訊息
- 重新整理列表或詳情

---

## 5.6 Token 補發流程

### 觸發點

- Token 列表頁
- Token 詳情頁

### UI 形式

- 對話框或抽屜表單

### 欄位

- `effectiveAt`
- `expireAt`
- `deviceId`
- `deviceName`
- `reason`

### API

- `POST /api/token/reissue`

### 成功後行為

- 顯示 `newTokenId`
- 顯示新 `accessToken`
- 提示舊 token 已被替換
- 允許導向新 token 詳情頁

---

## 5.7 Token 續期流程

### 觸發點

- Token 列表頁
- Token 詳情頁

### 欄位

- `newExpireAt`
- `reason`

### API

- `POST /api/token/renew`

### 成功後行為

- 顯示新到期時間
- 更新畫面資料

---

## 5.8 Device 列表頁 `/devices`

### 目的

提供管理者查詢與維護可綁定的裝置資料。

### 查詢欄位

- `userId`
- `deviceId`
- `isEnabled`
- `pageNo`
- `pageSize`

### 列表欄位

- `userId`
- `deviceId`
- `deviceName`
- `deviceType`
- `isEnabled`

### 操作

- 建立新 device
- 停用 device

### API

- `POST /api/device/list`

---

## 5.9 Device 建立頁 `/devices/create`

### 表單欄位

- `userId`
- `deviceId`
- `deviceName`
- `deviceType`
- `remark`

### API

- `POST /api/device/create`

### 成功後行為

- 顯示成功訊息
- 返回 device 列表並帶入剛建立的 `deviceId`

---

## 5.10 Device 停用流程

### 觸發點

- Device 列表頁

### 欄位

- `reason`

### API

- `POST /api/device/disable`

### 成功後行為

- 刷新列表
- 顯示停用成功訊息

---

## 5.11 Token 使用紀錄頁 `/logs/token-usage`

### 目的

查詢 token 被使用的 API 紀錄，協助追蹤異常呼叫。

### 查詢欄位

- `tokenId`
- `userId`
- `deviceId`
- `clientIp`
- `isSuccess`
- `requestTimeStart`
- `requestTimeEnd`
- `pageNo`
- `pageSize`

### 列表欄位

- `caseId`
- `tokenId`
- `userId`
- `requestTime`
- `httpMethod`
- `requestPath`
- `clientIp`
- `deviceId`
- `isSuccess`
- `failureReason`

### API

- `POST /api/token/usage`

---

## 5.12 Token 操作紀錄頁 `/logs/token-action`

### 目的

查詢管理者對 token 執行的建立、撤銷、補發、續期等操作紀錄。

### 查詢欄位

- `tokenId`
- `actionType`
- `operatorUserId`
- `createdAtStart`
- `createdAtEnd`
- `pageNo`
- `pageSize`

### 列表欄位

- `caseId`
- `tokenId`
- `actionType`
- `actionReason`
- `actionResult`
- `operatorUserId`
- `operatorUserName`
- `beforeStatus`
- `afterStatus`
- `createdAt`

### API

- `POST /api/token/action-log`

---

## 六、前端狀態管理

### 狀態切分

- `AuthState`
  - 是否已登入
  - 目前管理者資訊
  - 目前 `AdminSession` token
- `AppShellState`
  - 目前頁面
  - 全域通知訊息
- `PageState`
  - 查詢條件
  - 列表資料
  - 分頁資訊
  - loading / error / empty 狀態

### 儲存策略

V1 建議：

- 將 `AdminSession` token 放在 `sessionStorage`
- 記住使用者偏好與查詢條件時，可放在 `localStorage`

### 理由

- `sessionStorage` 關閉分頁後即失效，風險較低
- 實作成本低，符合 V1 快速落地目標
- 不需先建立 BFF 或 cookie session 機制

### 後續可升級方向

若未來要提高安全性，可改為：

- `AdminWeb` 走 server-side session
- 前端不直接持有 JWT
- 由 BFF 代送 Bearer token 到 API

---

## 七、API Client 規格

### 統一行為

所有 API 呼叫需經過統一 client，處理以下事項：

- 自動附加 `Authorization: Bearer {token}`
- 統一解析 `ApiResponse<T>`
- 統一處理 `success = false`
- 統一攔截 `401 / 403`
- 統一記錄與顯示 `caseId`

### 共同處理規則

- `401`
  - 清除登入狀態
  - 導向 `/login`
  - 顯示 session 已失效
- `403`
  - 顯示權限不足
- `success = false`
  - 顯示 `ErrorCode`、`ErrorMessage`、`caseId`
- 網路失敗
  - 顯示系統暫時無法連線

### 型別模型

前端需有共同模型：

- `ApiResponse<T>`
- `ApiErrorData`
- `PagedResult<T>`

---

## 八、頁面流程

## 8.1 啟動流程

```text
開啟網站
  -> 檢查 sessionStorage 是否有 accessToken
  -> 無 token：導向 /login
  -> 有 token：呼叫 GET /api/auth/me
     -> 成功：建立 AuthState，進入 /tokens
     -> 失敗：清除 token，導向 /login
```

## 8.2 登入流程

```text
/login 輸入帳密
  -> 呼叫 POST /api/auth/login
  -> 成功：儲存 token 與登入者資訊
  -> 呼叫 GET /api/auth/me 做二次確認
  -> 導向 /tokens
```

## 8.3 登出流程

```text
使用者按下登出
  -> 清除 sessionStorage
  -> 清除記憶中的 AuthState
  -> 導向 /login
```

## 8.4 Token 建立流程

```text
/tokens/create 填表
  -> 前端驗證
  -> 呼叫 POST /api/token/create
  -> 成功後顯示 accessToken
  -> 使用者複製後回到列表或詳情
```

## 8.5 Token 例外流程

```text
使用者在任一頁操作
  -> API 回傳 401
  -> 顯示 session 已過期
  -> 清除登入狀態
  -> 導向 /login
```

---

## 九、權限與安全邊界

### 前端信任邊界

- 前端不得自行認定使用者有權限
- 前端僅根據 API 是否成功來更新畫面
- 前端不得生成或修改 JWT claim

### 前端需注意事項

- 不把管理者密碼寫入 localStorage
- 不把業務 token 預設保存為登入 token
- 顯示 access token 時提供明確敏感資訊提示
- 錯誤畫面需顯示 `caseId` 以利追查

### 顯示層限制

即使前端隱藏某些按鈕，也不能取代後端 policy：

- `AdminOnly`
- `TokenManage`

---

## 十、元件與模組切分建議

### Core

- `AuthenticationService`
- `CurrentUserState`
- `ApiClient`
- `NavigationGuard`

### Shared UI

- `AppLayout`
- `SideNav`
- `PageHeader`
- `SearchPanel`
- `DataTable`
- `ConfirmDialog`
- `ErrorAlert`
- `EmptyState`
- `LoadingOverlay`

### Feature Modules

- `Features/Auth`
- `Features/Tokens`
- `Features/Devices`
- `Features/Logs`

### 型別

- `Models/Common`
- `Models/Auth`
- `Models/Tokens`
- `Models/Devices`
- `Models/Logs`

---

## 十一、V1 驗收標準

### 功能驗收

- 可使用 `admin / admin` 成功登入
- 重新整理頁面後可維持登入狀態
- token 過期或失效時會回到登入頁
- 可查詢 token 列表
- 可建立 token 並取得 `accessToken`
- 可對 token 執行 revoke / reissue / renew
- 可查詢 device 列表並建立、停用 device
- 可查詢 token usage log 與 action log

### 體驗驗收

- 每個頁面都有 loading / empty / error 狀態
- 錯誤訊息可看到 `caseId`
- 關鍵動作都有確認流程
- 查詢表單與列表欄位名稱與後端 contract 一致

### 技術驗收

- 前端 API 呼叫集中管理
- 不在頁面內散落 Bearer token 邏輯
- 不直接耦合後端 controller 實作細節
- solution 可一鍵 build

---

## 十二、實作順序建議

### Phase A：前端骨架

- 建立 `Dimensions.AdminWeb`
- 建立 layout、路由、導覽
- 建立 API client 與 auth state

### Phase B：認證

- `/login`
- 啟動驗證
- 登出流程

### Phase C：Token 管理

- token list
- token detail
- create / revoke / reissue / renew

### Phase D：Device 管理

- device list
- create / disable

### Phase E：Log 查詢

- usage log
- action log

### Phase F：補強

- 表單驗證
- 共用錯誤處理
- 體驗與樣式調整

---

## 十三、後續延伸方向

- Dashboard 首頁
- Token 匯出
- Device 與 Token 關聯視圖
- Role / RBAC
- 模組與 scope 可視化
- BFF / HttpOnly Cookie session
- 更完整的自動化 UI 測試

---

## 十四、結論

`AdminWeb` 的 V1 應以「先把管理流程跑通」為核心，而不是先追求複雜前端架構。  
只要完成登入、Token 管理、Device 管理、Log 查詢四塊，就能支撐目前文件中定義的管理後台需求，並與既有 `Auth / Token / Device` API 無縫對接。
