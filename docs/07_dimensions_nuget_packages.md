# Dimensions 專案 NuGet 套件清單

## 目的

這份文件整理 solution 目前各 project 直接使用到的 NuGet 套件，方便新手快速知道：

- 哪個 project 用了哪些套件
- 這些套件是拿來做什麼的

## 建議閱讀方式

如果你剛加入，先看：

1. API host 用哪些套件
2. 資料存取用哪些套件
3. logging 與 Swagger 用哪些套件

## 目前已建立的 project

- `Dimensions.Api`
- `Dimensions.Application`
- `Dimensions.Domain`
- `Dimensions.Infrastructure`
- `Dimensions.Contracts`
- `Dimensions.Api.Tests`
- `Dimensions.Application.Tests`

以上 project 是目前 solution 中已經存在、可直接看到 `.csproj` 的專案。

## 已定案、待建立的 project

以下 project 是架構上已定案，但目前尚未建立的專案：

- `Dimensions.Admin.Api`
- `Dimensions.Admin.Web`
- `Dimensions.Admin.Api.Tests`

等這些 project 建立後，也要把對應的 NuGet 套件補進這份文件。

## Dimensions.Api

主要用途：

- API host
- JWT 驗證
- Swagger
- request validation
- logging

常見套件：

- `FluentValidation.DependencyInjectionExtensions`
  - request validation
- `Microsoft.AspNetCore.Authentication.JwtBearer`
  - JWT bearer authentication
- `Microsoft.Extensions.Logging.Log4Net.AspNetCore`
  - `log4net` provider
- `Swashbuckle.AspNetCore`
  - Swagger / OpenAPI

## Dimensions.Infrastructure

主要用途：

- DB 存取
- repository implementation

常見套件：

- `Dapper`
  - 輕量 ORM
- `Microsoft.Data.SqlClient`
  - SQL Server provider
- `Microsoft.Data.Sqlite`
  - SQLite provider

## 其他已建立 project

目前以下 project 沒有特別列出直接安裝的 NuGet 套件，主要原因是：

- `Dimensions.Application`
  - 目前以介面、服務邏輯與共用規則為主
- `Dimensions.Domain`
  - 目前以 domain model 與列舉為主
- `Dimensions.Contracts`
  - 目前以 request / response model 為主
- `Dimensions.Api.Tests`
  - 後續會依實際測試框架再補齊
- `Dimensions.Application.Tests`
  - 後續會依實際測試框架再補齊

## 新手怎麼看這份

如果你只想先理解一件事，可以先記住：

- API 相關功能大多看 `Dimensions.Api`
- DB 相關功能大多看 `Dimensions.Infrastructure`

等之後 `Dimensions.Admin.Api` 與 `Dimensions.Admin.Web` 建好，再把它們的套件一起補進來。
