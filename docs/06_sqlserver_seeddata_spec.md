# SQL Server Seed Data 規格

## 目的

這份文件說明 seed data 應該準備哪些資料，以及這些資料主要用來驗證哪些情境。

## Seed data 的角色

seed data 主要不是為了正式環境，而是為了：

- DEV / TEST 快速驗證
- API smoke test
- token / device 規則測試
- 管理流程演練

## 建議準備的資料

### 1. 管理員資料

建議至少有兩筆：

- 系統管理員
- 備援管理員

用途：

- 驗證 admin login
- 驗證 token 管理

### 2. Device 資料

建議至少有：

- `USER001` 的主裝置
- `USER001` 的其他裝置
- `USER002` 的測試裝置

用途：

- 驗證單裝置 token
- 驗證 device 停用

### 3. Token 資料

建議至少有：

- `AdminSession`
- `UserAccess`
- `Integration`

可再補：

- 已撤銷 token
- 已過期 token
- 停用 token

用途：

- 驗證 token list
- 驗證 token detail
- 驗證 revoke / renew / reissue
- 驗證業務 API 存取

## 建議測試情境

### 情境 1：管理員登入成功

目的：

- 驗證 admin login 與 `AdminSession`

### 情境 2：管理員登入失敗

目的：

- 驗證錯誤回應格式

### 情境 3：管理員可查 token 清單

目的：

- 驗證 `TokenManage` policy

### 情境 4：業務 token 可呼叫業務 API

目的：

- 驗證 `Dimensions.Api` 的 JWT 驗章、DB token 狀態與 scope 規則

### 情境 5：單裝置 token 驗證

目的：

- 先驗證單裝置欄位與預留流程
- 後續再驗證 `X-Device-Id`
- 後續再驗證 token 與 device 正式綁定

## 新手實作建議

如果你剛加入，建議先用 seed data 跑這條線：

1. 管理員登入
2. 建立 token
3. 用 token 呼叫 `Dimensions.Api`
4. 查 usage log

這條線跑通，就會對整個架構很有感覺。
