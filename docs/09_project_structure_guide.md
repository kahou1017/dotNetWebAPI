# 專案與目錄說明指南

## 目的

這份文件專門幫助新加入專案的人快速理解：

- `src/` 底下每個 project 的用途
- 每個 project 常見目錄的用途
- 新功能應該放在哪裡
- 哪些目錄通常不需要手動修改

如果你剛加入專案，建議在看完：

- [01_webapi_architecture_with_optional_admin_ui.md](01_webapi_architecture_with_optional_admin_ui.md)
- [02_dotnet_webapi_architecture_spec.md](02_dotnet_webapi_architecture_spec.md)

之後，接著看這份。

## 先記住一個簡單觀念

可以先把整個 solution 想成三層：

1. 入口層
   - `Dimensions.Api`
   - 未來的 `Dimensions.Admin.Api`
   - 未來的 `Dimensions.Admin.Web`
2. 應用與規則層
   - `Dimensions.Application`
   - `Dimensions.Domain`
   - `Dimensions.Contracts`
3. 基礎設施層
   - `Dimensions.Infrastructure`

簡單說：

- `Api / Admin.Api / Admin.Web` 負責「接請求、跑流程、回結果」
- `Application / Domain / Contracts` 負責「定規則、定模型、定流程」
- `Infrastructure` 負責「連資料庫、寫 repository、處理外部依賴」

## 目前 `src/` project 一覽

### 1. `Dimensions.Api`

用途：

- 提供業務 API
- 驗證業務 token
- 套用 middleware、policy、validation、Swagger、logging

這個 project 目前是整個專案的執行入口。

重點提醒：

- 這裡是業務 API，不是 token 發放中心
- 不應放管理員登入流程
- 之後 `TokenController`、`DeviceController` 應切到 `Dimensions.Admin.Api`

### 2. `Dimensions.Application`

用途：

- 放應用層服務
- 串接 use case 流程
- 定義 repository 介面
- 放抽象介面，例如 current user、caseId、DB 連線

你可以把它想成：

- 「controller 收到請求後，真正把流程串起來的人」

### 3. `Dimensions.Contracts`

用途：

- 放 request model
- 放 response model
- 放共用 API 回應格式
- 放分頁結果等共用 contract

這裡主要是「資料長什麼樣子」，不是放業務邏輯。

### 4. `Dimensions.Domain`

用途：

- 放 enum
- 放常數
- 放共用語意與核心規則

如果一個概念不應依賴 ASP.NET Core、DB、UI，通常就適合放這裡。

### 5. `Dimensions.Infrastructure`

用途：

- 放資料庫連線
- 放 Dapper repository
- 放 DB 初始化
- 放 provider 切換
- 放基礎設施實作

你可以把它想成：

- 「把 Application 需要的介面，真正做出來的地方」

## `Dimensions.Api` 目錄說明

目前常見目錄如下：

### `Authentication`

用途：

- 放 authentication handler
- 放 JWT 驗章相關元件

什麼時候放這裡：

- 需要處理使用者身分建立
- 需要客製 authentication flow

### `Constants`

用途：

- 放 API 專案內部會重複使用的常數

例如：

- HttpContext item key
- header key

### `Controllers`

用途：

- 放 API endpoint 入口

原則：

- controller 只負責收 request、呼叫 service、回 response
- 不要在 controller 寫太多業務判斷

例子：

- `CustomerController`
- `PublicController`

### `Extensions`

用途：

- 放 DI 註冊
- 放 authentication / Swagger / service collection 擴充方法

這類檔案通常是在 `Program.cs` 裡被呼叫。

### `Middleware`

用途：

- 放 request pipeline 中的共用處理

例子：

- `CaseIdMiddleware`
- `ExceptionHandlingMiddleware`
- `ApiRequestLogMiddleware`

### `Options`

用途：

- 放 `appsettings` 對應的設定類別

例子：

- `JwtOptions`

### `Policies`

用途：

- 放 authorization policy 相關常數或設定

### `Responses`

用途：

- 放 API 專案自己用的回應輔助模型

如果是跨 project 共用的 response model，優先考慮放 `Dimensions.Contracts`。

### `Services`

用途：

- 放 API 層專屬服務

例子：

- JWT 產生器
- 目前登入者資訊存取器

如果是跨 API 流程的應用邏輯，通常應放 `Dimensions.Application`。

### `Validation`

用途：

- 放 request validator
- 放 validation filter
- 放共用 validation rule helper

這裡主要處理「進來的 request 合不合法」。

### `Properties`

用途：

- 放專案屬性與啟動設定相關檔案

一般不用常改。

### `bin` / `obj`

用途：

- 編譯輸出目錄

通常：

- 不要手動修改
- 不需要放文件
- 也不應提交到版控

### `Logs`

用途：

- 本機執行時產生的 log

通常：

- 不要手動維護
- 不應提交到版控

## `Dimensions.Application` 目錄說明

### `Extensions`

用途：

- 放應用層 DI 註冊相關延伸方法

### `Interfaces`

用途：

- 放 repository 介面
- 放抽象服務介面

例子：

- `IDbConnectionFactory`
- `ICaseIdAccessor`

### `Models`

用途：

- 放 application service 內部使用的模型

如果模型是提供 API request / response 用，優先放 `Dimensions.Contracts`。

### `Services`

用途：

- 放應用層服務

例子：

- `AuthService`
- `TokenService`
- `DeviceService`
- `ApiLogService`

這裡是整個流程的核心位置之一。

## `Dimensions.Contracts` 目錄說明

目前目錄依功能分組：

- `Auth`
- `Business`
- `Common`
- `Device`
- `Token`

建議規則：

- 與登入有關的 request / response 放 `Auth`
- 與業務 API 有關的 contract 放 `Business`
- 與 token 管理有關的 contract 放 `Token`
- 與 device 管理有關的 contract 放 `Device`
- 共用回應格式、分頁結果放 `Common`

## `Dimensions.Domain` 目錄說明

### `Constants`

用途：

- 放 domain 共用常數

### `Enums`

用途：

- 放列舉

例子：

- `TokenType`
- `TokenStatus`

這裡應該保持乾淨，不要放 ASP.NET Core 或資料庫相依邏輯。

## `Dimensions.Infrastructure` 目錄說明

### `Extensions`

用途：

- 放 Infrastructure 的 DI 註冊

### `Options`

用途：

- 放 DB provider、connection、infra 設定

### `Persistence`

用途：

- 放資料存取相關實作

常見內容：

- `DbConnectionFactory`
- `DatabaseInitializer`
- `Repositories`
- `TypeHandlers`

如果跟資料庫、Dapper、初始化、seed、provider 有關，通常會放這裡。

## 新功能應該放哪裡

### 情境 1：新增一支業務 API

通常會改到：

1. `Dimensions.Contracts`
   - 新增 request / response model
2. `Dimensions.Application`
   - 新增 service 方法或流程
3. `Dimensions.Infrastructure`
   - 若需要 DB 存取，新增或修改 repository
4. `Dimensions.Api`
   - 新增 controller、validator、必要設定

### 情境 2：新增 token 管理功能

正式方向應該是：

1. `Dimensions.Contracts`
2. `Dimensions.Application`
3. `Dimensions.Infrastructure`
4. `Dimensions.Admin.Api`
5. `Dimensions.Admin.Web`

不建議再把新的 token 管理入口繼續塞回 `Dimensions.Api`。

### 情境 3：只改資料庫存取

通常優先看：

- `Dimensions.Application/Interfaces`
- `Dimensions.Infrastructure/Persistence`

## 哪些地方新手最容易放錯

### 把業務邏輯放進 Controller

不建議。

原因：

- controller 會變很肥
- 很難測試
- 後面搬到 `Admin.Api` 或其他入口會很麻煩

### 把 API request model 放到 Application

不建議。

原因：

- request / response 屬於 contract
- Application 應盡量不要依賴某個 API 入口長什麼樣

### 把資料庫實作放進 Application

不建議。

原因：

- Application 應依賴介面
- 真正實作應放 `Infrastructure`

## 給新手的實作順序建議

如果你準備開始改 code，建議先照這個順序：

1. 先看 [00_master_index.md](00_master_index.md)
2. 再看 [02_dotnet_webapi_architecture_spec.md](02_dotnet_webapi_architecture_spec.md)
3. 接著看這份目錄說明
4. 確認你要改的是哪一層
5. 最後再動手修改程式碼

這樣比較不容易一開始就把檔案放錯位置。
