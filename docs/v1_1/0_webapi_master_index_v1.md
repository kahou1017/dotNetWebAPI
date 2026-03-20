# WebAPI + JWT 系統設計總索引（Master Index V1.1）

> 本文件為整體專案所有設計文件的入口索引  
> 目的：提供「閱讀順序 + 實作順序 + 文件用途」統一管理  
> 本版已納入 V1.1 修正：`TokenType`、Policy 模型、Response 責任收斂、Device 綁定規則明確化、API 本機 log 採 `log4net`

---

# 一、文件總覽

目前核心設計文件共 7 份，另加 1 份專案套件清單：

0. Master Index  
1. 可含或不含管理前端的 WebAPI 專案藍圖  
2. .NET WebAPI 專案架構規格  
3. API / JWT 端點規格文件  
4. SQL Server 建表腳本  
5. SQL Server 初始化 INSERT Script  
6. SQL Server 初始化資料與測試資料規格  
7. Dimensions 各 Project 的 NuGet 套件清單

---

# 二、文件用途對照

| 文件 | 用途 | 階段 |
|------|------|------|
| 1 藍圖文件 | 定義系統邊界與前後端關係 | Phase 1 |
| 2 架構文件 | 定義專案分層與責任切分 | Phase 1 |
| 3 API 文件 | 定義契約、授權與 Token 使用規則 | Phase 1 |
| 4 DB Schema | 定義資料表、索引、約束 | Phase 2 |
| 6 Seed 規格 | 定義測試資料與情境 | Phase 2 |
| 5 Insert Script | 建立可直接執行的測試資料 | Phase 2 |
| 7 NuGet 套件清單 | 記錄各專案使用的 NuGet 套件 | 補充文件 |

---

# 三、建議閱讀順序

建議第一次閱讀照此順序：

1. `1_webapi_architecture_with_optional_admin_ui.md`
2. `2_dotnet_webapi_architecture_spec_v1.md`
3. `3_api_jwt_endpoint_spec_v1.md`
4. `4_sqlserver_jwt_api_schema_v1.md`
5. `6_sqlserver_seeddata_spec_v1.md`
6. `5_sqlserver_seed_insert_v1.md`
7. `7_dimensions_nuget_packages.md`

---

# 四、V1.1 定案重點

本版相較 V1，新增下列核心原則：

1. `JwtToken` 新增 `TokenType`
2. `ApiResponse<T>` 只保留一份，放在 `Contracts`
3. `CaseIdMiddleware` 只建立 `caseId`，不包裝 JSON
4. 不採用 `UnifiedResponseFilter`
5. 授權模型至少包含：
   - `AdminOnly`
   - `AuthenticatedUser`
   - `TokenManage`
6. `JwtDeviceRegistry.UserId` 視為必填
7. `IsSingleDevice = 1` 時，`DeviceId` 必填且需比對 request header
8. API 本機檔案 log 採 `log4net`

---

# 五、實作順序（開發流程）

## Phase 1：文件定版

1. 確認藍圖文件  
2. 確認架構文件  
3. 確認 API 文件  
4. 確認 DB Schema

產出：
- Solution 結構
- API 契約
- 資料表設計

## Phase 2：資料庫

5. 執行建表腳本  
6. 執行初始化 INSERT Script

產出：
- DB Schema
- DEV / TEST 資料

## Phase 3：核心管線

7. `ApiResponse<T>` / `ApiErrorData`
8. `CaseIdMiddleware`
9. `ExceptionHandlingMiddleware`
10. Authentication / Authorization
11. `ApiRequestLogMiddleware`

產出：
- 可運作的 API Pipeline

## Phase 4：授權與 Token

12. `AdminOnly`
13. `AuthenticatedUser`
14. `TokenManage`
15. `TokenType` 驗證規則
16. Device 綁定規則

產出：
- 可區分 Admin 與業務 Token 的安全模型

## Phase 5：API 實作

17. `AuthController`
18. `TokenController`
19. `DeviceController`
20. `PublicController`

## Phase 6：測試

21. Postman / Bruno
22. Login / Token / Device / Log / Business API 測試

## Phase 7：前端（可選）

23. 建立 `AdminWeb`
24. 只作為 API Consumer
25. 串接管理員 API

---

# 六、開發優先順序

若要直接開始開發，建議依下列順序：

1. `Contracts` 共用回傳模型
2. `CaseIdMiddleware`
3. `ExceptionHandlingMiddleware`
4. Authentication / Authorization / Policy
5. `AuthController`
6. `TokenController`
7. `DeviceController`
8. Log 與 Usage 記錄
9. `log4net` 本機檔案 logging

---

# 七、系統核心（必須先完成）

- `caseId` 流程
- 統一 JSON Response
- JWT 驗章
- DB Token 狀態驗證
- `TokenType` 分型
- Device 綁定驗證
- API Log
- Exception Log
- Token Action / Usage Log
- 本機檔案 log

---

# 八、系統可選模組

- `AdminWeb`
- 報表 / Dashboard
- 進階權限（Role / RBAC）
- Module Scope
- Rate Limit

---

# 九、結論

本專案採：

- WebAPI 為核心
- Admin UI 可選
- API 契約驅動
- Middleware + Policy 為核心骨幹
- Token 與 Device 規則在後端收斂
- 本機檔案 log 由 `log4net` 提供

---

# 十、開發口訣

先定責任  
再做安全  
再做 API  
最後才做 UI
