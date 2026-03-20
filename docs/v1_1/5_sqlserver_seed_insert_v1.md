# SQL Server 初始化資料 INSERT Script（V1.1，可直接執行）

> 根據 V1.1 規格，提供可直接執行的 SQL Server 測試資料  
> 僅限 DEV / TEST 環境

---

## 一、注意事項

1. 時間使用 `SYSUTCDATETIME()`
2. PasswordHash / TokenHash 皆為測試用假資料
3. 本版已加入 `TokenType`
4. `JwtDeviceRegistry.UserId` 為必填

---

## 二、JwtAdminUser 初始化

```sql
INSERT INTO dbo.JwtAdminUser
(UserId, LoginAccount, DisplayName, PasswordHash, PasswordSalt, Email, IsEnabled, IsLocked, CreatedBy)
VALUES
('ADMIN001','admin','System Admin','HASHEDPWD1','SALT1','admin@example.local',1,0,'SYSTEM'),
('ADMIN002','admin2','Backup Admin','HASHEDPWD2','SALT2','admin2@example.local',1,0,'SYSTEM');
```

---

## 三、JwtDeviceRegistry 初始化

```sql
INSERT INTO dbo.JwtDeviceRegistry
(DeviceId, UserId, DeviceName, DeviceType, IsEnabled, Remark)
VALUES
('DEVICE-001','USER001','Kevin Laptop','Windows',1,'主工作站'),
('DEVICE-002','USER001','Kevin Desktop','Windows',1,'備用裝置'),
('DEVICE-003','USER002','Test Notebook','Windows',1,'測試裝置');
```

---

## 四、JwtToken 初始化

```sql
INSERT INTO dbo.JwtToken
(TokenId, JwtId, TokenType, UserId, UserName, TokenName, TokenHash, TokenPrefix, Scope, Status, IsRevoked, IsSingleDevice, IsEnabled, CanReissue, CanRenew, DeviceId, IssuedAt, EffectiveAt, ExpireAt, Purpose, CreatedSource, CreatedBy)
VALUES
-- Admin session
('ADM2026000001','aaaa1111-1111-1111-1111-111111111111','AdminSession','ADMIN001','System Admin','Admin Session','HASH-ADM-1','ADM2026','token.manage','Active',0,0,1,0,0,NULL,SYSUTCDATETIME(),SYSUTCDATETIME(),DATEADD(DAY,30,SYSUTCDATETIME()),'AdminLogin','SeedData','SYSTEM'),

-- Active user access token
('TK2026000001','11111111-1111-1111-1111-111111111111','UserAccess','USER001','Kevin','Main Token','HASH1','TK2026','customer.query order.create','Active',0,1,1,1,1,'DEVICE-001',SYSUTCDATETIME(),SYSUTCDATETIME(),DATEADD(DAY,30,SYSUTCDATETIME()),'UserAccess','SeedData','SYSTEM'),

-- Revoked user access token
('TK2026000002','22222222-2222-2222-2222-222222222222','UserAccess','USER001','Kevin','Revoked Token','HASH2','TK2026','customer.query','Revoked',1,0,1,1,1,NULL,SYSUTCDATETIME(),SYSUTCDATETIME(),DATEADD(DAY,30,SYSUTCDATETIME()),'UserAccess','SeedData','SYSTEM'),

-- Expired user access token
('TK2026000003','33333333-3333-3333-3333-333333333333','UserAccess','USER001','Kevin','Expired Token','HASH3','TK2026','customer.query','Expired',0,0,1,1,1,NULL,DATEADD(DAY,-40,SYSUTCDATETIME()),DATEADD(DAY,-40,SYSUTCDATETIME()),DATEADD(DAY,-10,SYSUTCDATETIME()),'UserAccess','SeedData','SYSTEM'),

-- Permanent integration token
('TK2026000004','44444444-4444-4444-4444-444444444444','Integration','USER002','TestUser','Permanent Integration Token','HASH4','TK2026','customer.query account.update','Active',0,0,1,1,1,NULL,SYSUTCDATETIME(),SYSUTCDATETIME(),NULL,'Integration','SeedData','SYSTEM');
```

---

## 五、JwtTokenActionLog 初始化

```sql
INSERT INTO dbo.JwtTokenActionLog
(CaseId, TokenId, ActionType, ActionResult, OperatorUserId, OperatorUserName, CreatedAt)
VALUES
(NEWID(),'ADM2026000001','Login','Success','ADMIN001','System Admin',SYSUTCDATETIME()),
(NEWID(),'TK2026000001','CreateToken','Success','ADMIN001','System Admin',SYSUTCDATETIME()),
(NEWID(),'TK2026000002','RevokeToken','Success','ADMIN001','System Admin',SYSUTCDATETIME()),
(NEWID(),'TK2026000001','ReissueToken','Success','ADMIN002','Backup Admin',SYSUTCDATETIME()),
(NEWID(),'TK2026000001','RenewToken','Success','ADMIN001','System Admin',SYSUTCDATETIME());
```

---

## 六、JwtTokenUsageLog 初始化

```sql
INSERT INTO dbo.JwtTokenUsageLog
(CaseId, TokenId, JwtId, TokenType, UserId, RequestTime, HttpMethod, RequestPath, ClientIp, DeviceId, IsSuccess, FailureReason)
VALUES
-- Admin session success
(NEWID(),'ADM2026000001','aaaa1111-1111-1111-1111-111111111111','AdminSession','ADMIN001',SYSUTCDATETIME(),'GET','/api/auth/me','10.0.0.10',NULL,1,NULL),

-- User access success
(NEWID(),'TK2026000001','11111111-1111-1111-1111-111111111111','UserAccess','USER001',SYSUTCDATETIME(),'POST','/api/customer/query','10.0.0.1','DEVICE-001',1,NULL),

-- Revoked token fail
(NEWID(),'TK2026000002','22222222-2222-2222-2222-222222222222','UserAccess','USER001',SYSUTCDATETIME(),'POST','/api/customer/query','10.0.0.2','DEVICE-001',0,'Auth.TokenRevoked'),

-- Device mismatch fail
(NEWID(),'TK2026000001','11111111-1111-1111-1111-111111111111','UserAccess','USER001',SYSUTCDATETIME(),'POST','/api/customer/query','10.0.0.3','DEVICE-999',0,'Auth.DeviceMismatch'),

-- Expired token fail
(NEWID(),'TK2026000003','33333333-3333-3333-3333-333333333333','UserAccess','USER001',SYSUTCDATETIME(),'POST','/api/customer/query','10.0.0.4','DEVICE-001',0,'Auth.TokenExpired');
```

---

## 七、驗證建議

```sql
SELECT * FROM dbo.JwtAdminUser;
SELECT * FROM dbo.JwtDeviceRegistry;
SELECT * FROM dbo.JwtToken;
SELECT * FROM dbo.JwtTokenActionLog;
SELECT * FROM dbo.JwtTokenUsageLog;
```

---

## 八、完成

此腳本完成後，系統可直接驗證：

- 管理員登入與 `AdminSession`
- `UserAccess` Token 驗證
- `Integration` Token 永久有效行為
- 單裝置驗證成功與失敗
- Token 操作紀錄
- Token 使用紀錄
