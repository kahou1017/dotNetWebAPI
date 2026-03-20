# SQL Server Seed Insert Script 說明

## 目的

這份文件說明 seed SQL 的用途與建議執行順序。

它的定位是：

- 給 DEV / TEST 使用
- 幫助快速建立基本測試資料

## 建議執行順序

1. 管理員資料
2. device 資料
3. token 資料
4. action log / usage log 測試資料

## 為什麼不要一開始就先看這份

因為這份是「可執行資料」，不是「設計說明」。

新手如果先看這份，通常會知道資料長怎樣，但不容易理解：

- 為什麼有這些欄位
- token 類型怎麼分
- 哪些資料是給 admin 用
- 哪些資料是給業務 API 用

所以建議先看：

1. 架構文件
2. API 規格
3. schema
4. 再看這份

## 建議 seed 內容

### 管理員

至少準備：

- `ADMIN001`
- `ADMIN002`

### device

至少準備：

- `USER001` 的主要 device
- `USER001` 的次要 device
- `USER002` 的測試 device

### token

至少準備：

- 1 筆 `AdminSession`
- 1 筆 `UserAccess`
- 1 筆 `Integration`

## 給新手的理解方式

你可以把 seed 想成：

- 不是正式資料
- 是讓 API 可以立刻測起來的練習資料

例如：

- 管理員登入要有帳號
- 單裝置 token 要有對應 device
- token 清單頁要先有 token 可以查
