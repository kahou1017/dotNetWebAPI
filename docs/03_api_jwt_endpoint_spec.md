# API 與 JWT 端點規格

## 目的

這份文件用來定義：

- API 邊界
- JWT 使用規則
- token 類型
- admin API 與業務 API 的分工

目前系統有兩個 API 面向：

- `Dimensions.Api`
- `Dimensions.Admin.Api`

## API 面向切分

### Dimensions.Admin.Api

用途：

- 管理員登入
- 管理員 session 驗證
- token 管理
- device 管理
- 管理查詢與 log

建議路由前綴：

```text
/admin-api
```

### Dimensions.Api

用途：

- 業務 API
- 整合 API
- 需要 token 驗證的 consumer API

建議路由前綴：

```text
/api
```

## 共用 header 規則

### Authorization

```text
Authorization: Bearer {token}
```

### Device Header

當 token 是單裝置綁定時，需要帶：

```text
X-Device-Id: {device-id}
```

## Token 類型

- `AdminSession`
- `UserAccess`
- `Integration`
- `Service`

## Policy 規則

### Admin API

- `AdminOnly`
- `TokenManage`

接受的 token 類型：

- `AdminSession`

### 業務 API

- `AuthenticatedUser`
- 未來可擴充 scope/module policy

接受的 token 類型：

- `UserAccess`
- `Integration`
- `Service`

拒絕的 token 類型：

- `AdminSession`

## Admin API 端點

### 1. 管理員登入

#### `POST /admin-api/auth/login`

用途：

- 驗證管理員帳號密碼
- 簽發 `AdminSession`

### 2. 取得目前管理員資訊

#### `GET /admin-api/auth/me`

用途：

- 回傳目前登入中的管理員資訊

Policy：

- `AdminOnly`

### 3. Token 管理

#### `POST /admin-api/token/list`
#### `POST /admin-api/token/detail`
#### `POST /admin-api/token/create`
#### `POST /admin-api/token/revoke`
#### `POST /admin-api/token/reissue`
#### `POST /admin-api/token/renew`
#### `POST /admin-api/token/usage`
#### `POST /admin-api/token/action-log`

Policy：

- `TokenManage`

接受 token 類型：

- `AdminSession`

### 4. Device 管理

#### `POST /admin-api/device/list`
#### `POST /admin-api/device/create`
#### `POST /admin-api/device/disable`

Policy：

- `TokenManage`

接受 token 類型：

- `AdminSession`

## 業務 API 端點

### 範例：客戶查詢

#### `POST /api/customer/query`

Policy：

- `AuthenticatedUser`

接受 token 類型：

- `UserAccess`
- `Integration`
- `Service`

驗證規則：

- 驗 JWT 簽章
- 驗 DB token 狀態
- 驗 token 類型
- 驗 scope
- 驗 effective / expire 時間
- 如果是單裝置 token，驗 `X-Device-Id`

## Token 管理規則

### 建立 token

`Dimensions.Admin.Api` 可以簽發：

- `UserAccess`
- `Integration`
- `Service`
- 管理登入流程需要的 `AdminSession`

### 撤銷 / 補發 / 續期

這些操作只屬於 `Dimensions.Admin.Api`。

### 單裝置綁定

當 `IsSingleDevice = true`：

- `DeviceId` 必填
- 業務 API request 必須帶 `X-Device-Id`
- request 帶入的 device 必須與 DB 綁定資料一致

## JWT Claim 最低需求

- `sub`
- `name`
- `role`
- `scope`
- `token_type`
- `token_id`
- `jti`

## 回應格式

所有 API 都應回傳共用格式：

- `timestamp`
- `success`
- `method`
- `path`
- `caseId`
- `systemCode`
- `data`
- `error`

## 常見錯誤碼方向

### 驗證與授權類

- `Auth.TokenMissing`
- `Auth.TokenInvalid`
- `Auth.TokenSignatureInvalid`
- `Auth.TokenNotFound`
- `Auth.TokenRevoked`
- `Auth.TokenExpired`
- `Auth.TokenNotEffective`
- `Auth.TokenDisabled`
- `Auth.TokenTypeInvalid`
- `Auth.DeviceIdMissing`
- `Auth.DeviceMismatch`
- `Auth.AdminOnly`
- `Auth.LoginFailed`

### 系統與驗證類

- `Validation.InvalidRequest`
- `System.RouteNotFound`
- `System.MethodNotAllowed`
- `System.BadRequest`
- `System.Unauthorized`
- `System.Forbidden`
- `System.InternalError`
- `System.DbError`

## Admin Web 對接方向

`Dimensions.Admin.Web` 應呼叫 `Dimensions.Admin.Api`，不直接呼叫 `Dimensions.Api` 做管理操作。

主要對接內容：

- 管理員登入
- 取得目前管理員資訊
- token 清單 / 詳情 / 建立 / 撤銷 / 補發 / 續期
- token 使用紀錄 / 操作紀錄
- device 清單 / 建立 / 停用

## 給新手的簡單例子

### 例子 1：管理員建立一個給外部系統使用的 token

流程如下：

1. 管理員登入 `Admin.Web`
2. `Admin.Web` 呼叫 `Admin.Api`
3. `Admin.Api` 建立一個 `Integration` token
4. 外部系統拿這個 token 呼叫 `Dimensions.Api`

### 例子 2：為什麼外部系統不能打 login

因為 `Dimensions.Api` 本來就不提供 login。

外部系統的入口是「拿已簽發 token 呼叫業務 API」，不是「先登入再拿 token」。
