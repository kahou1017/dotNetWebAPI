# 可含管理後台的 WebAPI 架構說明

## 目的

這份文件說明 `Dimensions` 的整體架構方向。

目前系統分成 3 個主要入口：

- `Dimensions.Api`
- `Dimensions.Admin.Api`
- `Dimensions.Admin.Web`

這樣切的核心目的很簡單：

- 業務 API 專心做業務
- 管理 API 專心做 token / device 管理
- 管理前台專心做操作畫面

## 一句話理解

可以先把整體想成：

- `Admin.Web` 負責操作
- `Admin.Api` 負責登入、發 token、管理 token
- `Api` 負責驗 token、執行業務 API

## 核心原則

### 架構原則總覽

這套架構的核心目標有兩個：

- 方便快速導入新專案
- 方便後續擴充、重構與模組化成長

因此整體設計採用以下原則：

1. 平台能力優先穩定
2. 業務模組各自獨立成長
3. 新功能以新增為優先
4. 依賴方向固定
5. 管理與業務永遠分流
6. 契約優先穩定

簡單理解：

- 先把可重用的地基做穩
- 再把新功能像疊積木一樣往上加
- 盡量不要因為一個新需求就到處改舊程式

### 原則 0：平台能力先穩，再長模組

在功能擴充之前，應先把平台共用能力穩定下來，例如：

- 驗證
- JWT 驗章與授權
- 統一回應格式
- 例外處理
- logging
- pagination
- options / configuration

這樣之後新增業務模組時，才不需要每個模組都各做一套。

### 原則 1：業務 API 不提供登入

`Dimensions.Api` 不提供帳號密碼登入。

也就是說：

- 外部系統不會透過 `Dimensions.Api` 取得 token
- 外部系統只會拿「已經簽發好的 token」來呼叫業務 API

這樣的好處是：

- 業務 API 比較單純
- 安全邊界更清楚
- 管理登入與業務存取不會混在一起

### 原則 2：token 的簽發與管理放在管理端

以下功能都屬於管理端：

- token 建立
- token 撤銷
- token 補發
- token 續期
- token 使用紀錄查詢
- device 管理

也就是：

- `Dimensions.Admin.Api`
- `Dimensions.Admin.Web`

### 原則 3：業務 API 是 token consumer

`Dimensions.Api` 的責任是：

- 驗 JWT 簽章
- 驗 DB token 狀態
- 驗 token 類型
- 驗 scope / policy
- 驗有效時間與過期時間
- 驗單裝置欄位與後續綁定流程
- 執行業務 API

它不負責簽發 token。

### 原則 4：管理 token 與業務 token 要明確區分

目前 token 類型定義為：

- `AdminSession`
- `UserAccess`
- `Integration`
- `Service`

用途如下：

- `AdminSession`
  - 只給 `Dimensions.Admin.Api` 與 `Dimensions.Admin.Web`
- `UserAccess`
  - 給一般受保護業務 API
- `Integration`
  - 給系統對系統整合 API
- `Service`
  - 保留給內部服務用途

### 原則 5：Admin Web 是正式管理入口

`Dimensions.Admin.Web` 採用 ASP.NET Core MVC。

這個選擇的理由是：

- 管理後台多半是表單、查詢、清單、維護
- MVC 對這類需求很穩定
- 與整體 .NET solution 技術棧一致
- 對新加入的人比較容易上手

### 原則 6：新功能以新增為優先

新增功能時，應優先採用：

- 新增 request / response
- 新增 validator
- 新增 service
- 新增 repository
- 新增 endpoint

而不是大量修改既有核心檔案。

這樣做的好處是：

- 舊功能比較不容易被影響
- 模組邊界會更清楚
- 後續重構與移植會比較容易

### 原則 7：依賴方向固定

整體依賴方向應維持單向：

- `Web/API -> Application`
- `Application -> Domain / Contracts`
- `Infrastructure` 負責實作 `Application` 定義的介面

這樣可避免：

- API 直接依賴資料存取細節
- Application 反向依賴 Web
- 業務規則綁死在特定資料來源或框架上

### 原則 8：契約優先穩定

`Dimensions.Contracts` 應作為系統內外溝通的穩定契約層。

它應主要放：

- request / response model
- DTO
- 統一回應模型
- 分頁模型
- 共用錯誤模型

它不應承載：

- SQL 細節
- Infrastructure 實作
- 特定框架耦合內容

## Solution 方向

```text
Dimensions.sln
src/
  Dimensions.Api
  Dimensions.Admin.Api
  Dimensions.Admin.Web
  Dimensions.Application
  Dimensions.Domain
  Dimensions.Infrastructure
  Dimensions.Contracts
tests/
  Dimensions.Api.Tests
  Dimensions.Admin.Api.Tests
  Dimensions.Application.Tests
```

## 各 project 的定位

### Dimensions.Api

用途：

- 提供業務 API
- 提供整合 API
- 驗 token 後執行業務流程

範例：

- `POST /api/customer/query`
- 未來的 `order`、`account` 等模組

不包含：

- 管理員登入
- token 管理
- device 管理

### Dimensions.Admin.Api

用途：

- 管理員登入
- 取得目前登入管理員資訊
- token 管理
- device 管理
- 管理查詢與稽核相關 API

範例：

- `POST /admin-api/auth/login`
- `GET /admin-api/auth/me`
- `POST /admin-api/token/create`
- `POST /admin-api/device/list`

### Dimensions.Admin.Web

用途：

- 管理員登入畫面
- token 管理畫面
- device 管理畫面
- 管理查詢與 log 畫面

前端技術：

- ASP.NET Core MVC

## 共用層

以下 project 先共用：

- `Dimensions.Application`
- `Dimensions.Domain`
- `Dimensions.Infrastructure`
- `Dimensions.Contracts`

目前先不再切出獨立的 `TokenManagement.Application` 或 `TokenManagement.Domain`。

原因是：

- 現在規模還沒大到需要切那麼細
- 先把入口 project 切清楚，比較有價值

## 安全邊界

### 管理端

`Dimensions.Admin.Web` 呼叫 `Dimensions.Admin.Api`。

管理端負責：

- 管理員登入
- 簽發 `AdminSession`
- 簽發業務 token
- 管理 token / device

### 業務端

`Dimensions.Api` 只接受「已經發好的 token」。

業務端負責：

- 驗 token
- 驗權限
- 驗裝置
- 執行業務 API

## 建議實作順序

1. 先定架構邊界
2. 再定 project 切分
3. 再定 API 路由與 JWT 規則
4. 再定 DB schema
5. 再定 seed 與測試情境
6. 最後做 Admin Web

## 補充說明

### 為什麼 `AdminSession` 不能當業務 token 用

因為它代表的是「管理員登入狀態」，不是一般使用者或整合系統的存取憑證。

簡單例子：

- 管理員登入後看到 token 管理畫面，用的是 `AdminSession`
- 管理員建立了一個給外部系統使用的 `Integration` token
- 這兩個 token 不能混用

### 為什麼新手要先理解這一點

因為後面的 controller、policy、DB schema、UI 流程，幾乎都圍繞這個邊界展開。
