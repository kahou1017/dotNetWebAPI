# Planning 說明

## 目的

`planning/` 不是正式規格文件，而是開發中的待辦與執行清單。

簡單說：

- `docs/` 回答「系統怎麼設計」
- `planning/` 回答「接下來先做什麼」

## 建議使用順序

開發前，請先看正式文件：

1. [00_master_index.md](../docs/00_master_index.md)
2. [02_dotnet_webapi_architecture_spec.md](../docs/02_dotnet_webapi_architecture_spec.md)
3. [09_project_structure_guide.md](../docs/09_project_structure_guide.md)

確認方向後，再進來看待辦：

1. [00_backend_todo_list.md](00_backend_todo_list.md)
2. [01_frontend_todo_list.md](01_frontend_todo_list.md)

## 目前建議開發節奏

1. 先整理與完成後端主線
2. 後端 API 邊界穩定後，再建立 `Dimensions.Admin.Web`
3. 前端開發以 `Dimensions.Admin.Api` 定案後的 API 為準

## 補充說明

- `00_backend_todo_list.md`
  - 主要處理後端架構、API、資料存取、驗證、logging、測試與 CI
- `01_frontend_todo_list.md`
  - 主要處理 `Dimensions.Admin.Web` 的 MVC 頁面與前端整合

如果不確定現在該做哪一份，預設先看後端。
