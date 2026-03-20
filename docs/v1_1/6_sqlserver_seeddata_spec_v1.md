# SQL Server 初始化資料與測試資料規格 V1.1

## 一、目標

本文件定義：

1. DEV / TEST 環境初始化資料
2. 管理員資料規格
3. Device 資料規格
4. Token 測試資料規格
5. 測試情境建議

---

## 二、初始化資料原則

### 1. 僅適用於 DEV / TEST

- 不直接套用到 PROD

### 2. 密碼不可使用正式明文

- 僅保存測試用 `PasswordHash` / `PasswordSalt`

### 3. Token 若預建

- 僅限測試環境
- 正式環境應由登入或管理流程建立

### 4. 時間資料使用 UTC

### 5. V1.1 起必須區分 `TokenType`

---

## 三、建議初始化資料清單

1. `JwtAdminUser`
2. `JwtDeviceRegistry`
3. `JwtToken`
4. `JwtTokenActionLog`
5. `JwtTokenUsageLog`

---

## 四、管理員帳號資料規格

### 建議至少 2 組

#### 系統管理員

- `UserId`: `ADMIN001`
- `LoginAccount`: `admin`
- `DisplayName`: `System Admin`
- `Email`: `admin@example.local`
- `IsEnabled`: `1`
- `IsLocked`: `0`
- `CreatedBy`: `SYSTEM`

#### 次要管理員

- `UserId`: `ADMIN002`
- `LoginAccount`: `admin2`
- `DisplayName`: `Backup Admin`
- `Email`: `admin2@example.local`
- `IsEnabled`: `1`
- `IsLocked`: `0`
- `CreatedBy`: `SYSTEM`

---

## 五、管理員密碼規格建議

### V1.1 建議

- 固定測試密碼，例如：`Admin@123456`
- DB 中保存：
  - `PasswordHash`
  - `PasswordSalt`

---

## 六、Device 資料規格

表：
- `JwtDeviceRegistry`

### 規則

- `UserId` 必填
- `DeviceId` 不重複
- Device 與 User 關係必須明確

### 建議建立 3 筆

#### Device 1

- `DeviceId`: `DEVICE-001`
- `UserId`: `USER001`
- `DeviceName`: `Kevin Laptop`
- `DeviceType`: `Windows`
- `IsEnabled`: `1`

#### Device 2

- `DeviceId`: `DEVICE-002`
- `UserId`: `USER001`
- `DeviceName`: `Kevin Desktop`
- `DeviceType`: `Windows`
- `IsEnabled`: `1`

#### Device 3

- `DeviceId`: `DEVICE-003`
- `UserId`: `USER002`
- `DeviceName`: `Test Notebook`
- `DeviceType`: `Windows`
- `IsEnabled`: `1`

---

## 七、Token 測試資料規格

表：
- `JwtToken`

### 建議至少建立 5 類

#### 1. `AdminSession`

用途：
- 測試管理員登入後的管理操作

建議資料：
- `TokenId`: `ADM2026000001`
- `TokenType`: `AdminSession`
- `UserId`: `ADMIN001`
- `Status`: `Active`

#### 2. Active `UserAccess`

用途：
- 測試正常業務 API 呼叫

建議資料：
- `TokenId`: `TK2026000001`
- `TokenType`: `UserAccess`
- `UserId`: `USER001`
- `Status`: `Active`
- `IsSingleDevice`: `1`
- `DeviceId`: `DEVICE-001`

#### 3. Revoked `UserAccess`

用途：
- 測試已撤銷驗證失敗

建議資料：
- `TokenId`: `TK2026000002`
- `TokenType`: `UserAccess`
- `Status`: `Revoked`
- `IsRevoked`: `1`

#### 4. Expired `UserAccess`

用途：
- 測試已過期驗證失敗

建議資料：
- `TokenId`: `TK2026000003`
- `TokenType`: `UserAccess`
- `Status`: `Expired`
- `ExpireAt`: 早於現在

#### 5. Permanent `Integration`

用途：
- 測試永久整合 Token

建議資料：
- `TokenId`: `TK2026000004`
- `TokenType`: `Integration`
- `Status`: `Active`
- `ExpireAt`: `NULL`

---

## 八、Token 操作紀錄建議

表：
- `JwtTokenActionLog`

### 建議至少有以下動作

1. `Login`
2. `CreateToken`
3. `RevokeToken`
4. `ReissueToken`
5. `RenewToken`

---

## 九、Token 使用紀錄建議

表：
- `JwtTokenUsageLog`

### 建議建立以下情境

1. `AdminSession` 呼叫 `/api/auth/me` 成功
2. `UserAccess` 呼叫業務 API 成功
3. 已撤銷 Token 失敗
4. 單裝置不符失敗
5. 已過期 Token 失敗

---

## 十、初始化資料對應測試情境

### 情境 1：管理員登入成功

- 帳號：`admin`
- 預期：取得 `AdminSession`

### 情境 2：管理員登入失敗

- 錯誤密碼
- 預期：`Auth.LoginFailed`

### 情境 3：管理員可查 Token 清單

- 使用 `AdminSession`
- 預期：可存取 `/api/token/list`

### 情境 4：`AdminSession` 不可當一般業務 Token 使用

- 視系統政策決定是否拒絕
- 建議 V1.1 預設拒絕混用

### 情境 5：Active `UserAccess` 可呼叫業務 API

- Token 綁 `DEVICE-001`
- Header 帶 `X-Device-Id: DEVICE-001`

### 情境 6：單裝置驗證失敗

- Token 綁 `DEVICE-001`
- Header 帶 `X-Device-Id: DEVICE-999`

### 情境 7：撤銷 Token 驗證失敗

- 對 `TK2026000002`
- 預期：`Auth.TokenRevoked`

### 情境 8：過期 Token 驗證失敗

- 對 `TK2026000003`
- 預期：`Auth.TokenExpired`

### 情境 9：永久 Integration Token 驗證

- 對 `TK2026000004`
- 預期：不因過期失敗

---

## 十一、初始化資料筆數建議

### 最小可用

- 管理員：2 筆
- Device：3 筆
- Token：5 筆
- ActionLog：5 筆以上
- UsageLog：5 筆以上

### 較完整測試

- Token：10 筆以上
- UsageLog：20 筆以上

---

## 十二、正式環境不建議初始化的資料

- 測試密碼
- 測試 Token
- 測試使用紀錄
- 測試操作紀錄

正式環境建議僅保留：

- 初始管理員帳號
- 必要的系統裝置資料

---

## 十三、後續可延伸項

1. Postman / Bruno 測試案例
2. API 驗證 checklist
3. 權限與 scope 測試
4. 業務 API 的 TokenType 白名單測試
