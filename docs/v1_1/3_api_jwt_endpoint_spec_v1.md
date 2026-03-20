# API / JWT 規格文件 V1.1

## 一、文件目標

定義本系統的：

1. API 分類
2. JWT / Policy 使用規則
3. Token 類型與使用邊界
4. 每支 API 的輸入 / 輸出規格
5. 需管理員的 API
6. 可匿名的 API
7. 每支 API 對應的資料表紀錄

---

## 二、API 分類總覽

### 1. 系統 / 驗證類

用途：
- 管理員登入
- 取得管理員登入資訊

### 2. Token 管理類

用途：
- Token 清單 / 明細
- 建立 / 撤銷 / 重發 / 延長
- 使用紀錄 / 操作紀錄

### 3. Device 管理類

用途：
- Device 清單
- Device 建立
- Device 停用

### 4. 公開查詢類

用途：
- 不敏感 GET API

### 5. 受保護業務類

用途：
- 受保護 POST API
- 模組查詢 / 異動 / 內部操作

---

## 三、共通 API 規則

### 3.1 Base Route

```text
/api
```

### 3.2 Header 規範

#### Content-Type

POST API：

```text
application/json
```

#### Authorization

需 JWT 時：

```text
Authorization: Bearer {token}
```

#### Device Header

需裝置辨識時：

```text
X-Device-Id: {device-id}
```

### 3.3 Response 共通格式

所有 API 一律回傳：

- `timestamp`
- `success`
- `method`
- `path`
- `caseId`
- `systemCode`
- `data`

### 3.4 `caseId` 規則

- 每支 API 都必須產生 `caseId`
- 成功與失敗都必須回傳
- 必須寫入 DB

---

## 四、JWT 與 Policy 規則

### 4.1 TokenType

V1.1 至少支援：

- `AdminSession`
- `UserAccess`
- `Integration`
- `Service`

### 4.2 Policy

V1.1 至少支援：

- `AdminOnly`
- `AuthenticatedUser`
- `TokenManage`

### 4.3 使用邊界

#### `AdminSession`

適用：
- `/api/auth/me`
- `/api/token/*`
- `/api/device/*`

#### `UserAccess`

適用：
- 一般受保護業務 API

#### `Integration`

適用：
- 系統對系統 API
- 無人值守整合

#### `Service`

適用：
- 內部服務呼叫

### 4.4 Device 規則

- 若 token `IsSingleDevice = true`
- request 必須帶 `X-Device-Id`
- header 值必須等於 DB 綁定 `DeviceId`

---

## 五、JWT Claim 建議

至少包含：

- `sub`
- `name`
- `role`
- `scope`
- `token_type`
- `token_id`
- `jti`

---

## 六、Auth API

### 6.1 管理員登入

**路徑**

```text
POST /api/auth/login
```

**是否需 JWT**
- 否

**Policy**
- 無

**用途**
- 驗證管理員帳密
- 簽發 `AdminSession`

**Request Body**

```json
{
  "loginAccount": "admin",
  "password": "********"
}
```

**Success Data 範例**

```json
{
  "tokenType": "AdminSession",
  "tokenId": "ADM2026000001",
  "jwtId": "7d7d0c7b-1b8f-4ef9-a3d4-111111111111",
  "accessToken": "jwt-token-string",
  "issuedAt": "2026-03-20T10:00:00Z",
  "effectiveAt": "2026-03-20T10:00:00Z",
  "expireAt": "2026-04-19T10:00:00Z",
  "userId": "ADMIN001",
  "displayName": "System Admin"
}
```

**DB 寫入**
- `ApiRequestLog`
- `JwtToken`
- `JwtTokenActionLog`

### 6.2 取得目前登入管理員資訊

**路徑**

```text
GET /api/auth/me
```

**是否需 JWT**
- 是

**Policy**
- `AdminOnly`

**允許 TokenType**
- `AdminSession`

---

## 七、Token 管理 API

### 7.1 查詢 Token 清單

**路徑**

```text
POST /api/token/list
```

**Policy**
- `TokenManage`

**允許 TokenType**
- `AdminSession`

**Request Body**

```json
{
  "tokenType": null,
  "userId": "USER001",
  "tokenId": null,
  "status": "Active",
  "isSingleDevice": null,
  "deviceId": null,
  "issuedAtStart": null,
  "issuedAtEnd": null,
  "expireAtStart": null,
  "expireAtEnd": null,
  "pageNo": 1,
  "pageSize": 20
}
```

### 7.2 查詢 Token 明細

**路徑**

```text
POST /api/token/detail
```

**Policy**
- `TokenManage`

**允許 TokenType**
- `AdminSession`

### 7.3 新建 Token

**路徑**

```text
POST /api/token/create
```

**Policy**
- `TokenManage`

**允許 TokenType**
- `AdminSession`

**Request Body**

```json
{
  "tokenType": "UserAccess",
  "userId": "USER001",
  "userName": "Kevin",
  "tokenName": "Integration Token",
  "isSingleDevice": true,
  "deviceId": "DEVICE-001",
  "deviceName": "Kevin Laptop",
  "effectiveAt": "2026-03-20T10:00:00Z",
  "expireAt": "2026-04-19T10:00:00Z",
  "isPermanent": false,
  "canReissue": true,
  "canRenew": true,
  "purpose": "UserAccess",
  "remark": "manual created token"
}
```

**規則**
- `tokenType` 不可為 `AdminSession`
- 若 `isSingleDevice = true`，則 `deviceId` 必填

### 7.4 撤銷 Token

**路徑**

```text
POST /api/token/revoke
```

**Policy**
- `TokenManage`

### 7.5 重發 Token

**路徑**

```text
POST /api/token/reissue
```

**Policy**
- `TokenManage`

### 7.6 延長 Token

**路徑**

```text
POST /api/token/renew
```

**Policy**
- `TokenManage`

### 7.7 查詢 Token 使用紀錄

**路徑**

```text
POST /api/token/usage
```

**Policy**
- `TokenManage`

### 7.8 查詢 Token 操作紀錄

**路徑**

```text
POST /api/token/action-log
```

**Policy**
- `TokenManage`

---

## 八、Device 管理 API

### 8.1 查詢裝置清單

**路徑**

```text
POST /api/device/list
```

**Policy**
- `TokenManage`

### 8.2 建立裝置

**路徑**

```text
POST /api/device/create
```

**Policy**
- `TokenManage`

**Request Body**

```json
{
  "userId": "USER001",
  "deviceId": "DEVICE-001",
  "deviceName": "Kevin Laptop",
  "deviceType": "Windows",
  "remark": "主工作站"
}
```

**規則**
- `deviceId` 不可重複
- `userId` 必填

### 8.3 停用裝置

**路徑**

```text
POST /api/device/disable
```

**Policy**
- `TokenManage`

---

## 九、公開 GET API

路徑形式：

```text
GET /api/public/{resource}
```

規則：
- 不需 JWT
- 僅限不敏感資料
- 仍需產生 `caseId`
- 仍需寫 `ApiRequestLog`

---

## 十、受保護業務 POST API

路徑形式：

```text
POST /api/{module}/{action}
```

例如：
- `POST /api/customer/query`
- `POST /api/order/create`
- `POST /api/account/update`

**Policy**
- `AuthenticatedUser`

**允許 TokenType**
- `UserAccess`
- `Integration`
- `Service`

**規則**
- 一律需 JWT
- 驗章
- 查 DB token 狀態
- 必要時檢查 `scope`
- 若為單裝置 token，需檢查 `X-Device-Id`

---

## 十一、JWT 驗證失敗錯誤碼建議

| errorCode | 說明 |
|---|---|
| Auth.TokenMissing | 未提供 Token |
| Auth.TokenInvalid | Token 格式錯誤 |
| Auth.TokenSignatureInvalid | Token 簽章無效 |
| Auth.TokenNotFound | Token 不存在 |
| Auth.TokenRevoked | Token 已撤銷 |
| Auth.TokenExpired | Token 已過期 |
| Auth.TokenNotEffective | Token 尚未生效 |
| Auth.TokenDisabled | Token 已停用 |
| Auth.TokenTypeInvalid | Token 類型不符 |
| Auth.DeviceIdMissing | 缺少 DeviceId |
| Auth.DeviceMismatch | DeviceId 不符 |
| Auth.AdminOnly | 僅限管理員 |
| Auth.LoginFailed | 帳號或密碼錯誤 |

---

## 十二、系統錯誤碼建議

| errorCode | 說明 |
|---|---|
| System.RouteNotFound | API 路徑不存在 |
| System.MethodNotAllowed | HTTP Method 不允許 |
| System.BadRequest | 請求格式錯誤 |
| System.ValidationError | 參數驗證失敗 |
| System.Unauthorized | 請先登入後再執行操作 |
| System.Forbidden | 無操作權限 |
| System.InternalError | 系統發生錯誤 |
| System.DbError | 資料處理失敗 |

---

## 十三、管理頁對應 API

1. 登入頁
   - `POST /api/auth/login`
2. 目前登入者資訊
   - `GET /api/auth/me`
3. Token 清單頁
   - `POST /api/token/list`
4. Token 明細頁
   - `POST /api/token/detail`
5. 建立 Token
   - `POST /api/token/create`
6. 撤銷 Token
   - `POST /api/token/revoke`
7. 重發 Token
   - `POST /api/token/reissue`
8. 延長 Token
   - `POST /api/token/renew`
9. Token 使用紀錄
   - `POST /api/token/usage`
10. Token 操作紀錄
   - `POST /api/token/action-log`
11. Device 管理
   - `POST /api/device/list`
   - `POST /api/device/create`
   - `POST /api/device/disable`
