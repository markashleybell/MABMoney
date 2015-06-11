SET IDENTITY_INSERT [dbo].[Users] ON
GO

-- Test user password 'test123'
INSERT INTO [dbo].[Users]
([UserID], [Forename], [Surname], [Email], [Password], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [Deleted], [DeletedBy], [DeletedDate], [PasswordResetGUID], [PasswordResetExpiry])
SELECT 1, N'Test', N'User', N'user@test.com', N'AJNzdwx56R+U3ls50NZbLTYQBm8j5Txr+F9mz3jQwzNjjIYjIjFuwBr/2l5VnjhQnw==', -1, '20150101 00:00:00.000', -1, '20150101 00:00:00.000', 0, NULL, NULL, N'7cc68dbb-3d12-487b-8295-e9b226cda017', '20160101 00:00:00.000' UNION ALL
SELECT 2, N'Deleted', N'User', N'deleted@test.com', N'AGhkTBPbmkYCxuLt5A0SXO35COIYPF6doq7dALqmR9A3t1y6tr4h58ZjjtLNizQGUw==', -1, '20150101 00:00:00.000', -1, '20150101 00:00:00.000', 1, -1, '20150101 00:00:00.000', N'5b977b67-e7e6-4399-9866-6c011750249f', '20160101 00:00:00.000'
GO

SET IDENTITY_INSERT [dbo].[Users] OFF
GO

SET IDENTITY_INSERT [dbo].[Accounts] ON
GO

INSERT INTO [dbo].[Accounts]
([AccountID], [Name], [StartingBalance], [User_UserID], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [Deleted], [DeletedBy], [DeletedDate], [Default], [Type], [TransactionDescriptionHistory], [CurrentBalance], [DisplayOrder])
SELECT 1, N'Current', 100.00, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, 1, 0, N'TEST1CURRENT|TEST2CURRENT', 100.00, 100 UNION ALL
SELECT 2, N'Savings', 500.00, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, 0, 1, N'TEST1SAVINGS|TEST2SAVINGS', 500.00, 200 UNION ALL
SELECT 3, N'Credit', 250.00, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, 0, 2, N'TEST1CREDIT|TEST2CREDIT', 250.00, 300 UNION ALL
SELECT 4, N'Deleted', 0.00, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000', 0, 0, N'TEST1DELETED|TEST2DELETED', 0.00, 400 UNION ALL
SELECT 5, N'Current', 100.00, 2, 2, '20150101 00:00:00.000', 2, '20150101 00:00:00.000', 0, NULL, NULL, 1, 0, N'TEST1CURRENT2|TEST2CURRENT2', 100.00, 100
GO

SET IDENTITY_INSERT [dbo].[Accounts] OFF
GO

SET IDENTITY_INSERT [dbo].[Categories] ON
GO

INSERT INTO [dbo].[Categories]
([CategoryID], [Name], [Account_AccountID], [Type], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [Deleted], [DeletedBy], [DeletedDate])
SELECT 1, N'Salary', 1, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 2, N'Rent', 1, 0, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 3, N'Food', 1, 0, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 4, N'Bills', 1, 0, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 5, N'Deleted', 1, 0, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000' UNION ALL
SELECT 6, N'Payments', 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 7, N'Bills', 3, 0, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 8, N'Deleted', 3, 0, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000' UNION ALL
SELECT 9, N'Salary', 5, 1, 2, '20150101 00:00:00.000', 2, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 10, N'Rent', 5, 0, 2, '20150101 00:00:00.000', 2, '20150101 00:00:00.000', 0, NULL, NULL
GO

SET IDENTITY_INSERT [dbo].[Categories] OFF
GO

SET IDENTITY_INSERT [dbo].[Sessions] ON
GO

INSERT INTO [dbo].[Sessions]([SessionID], [Key], [Expiry], [User_UserID], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [Deleted], [DeletedBy], [DeletedDate])
SELECT 1, N'USER1SESSION', '20160101 00:00:00.000', 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 2, N'USER1SESSIONEXPIRED', '20140101 00:00:00.000', 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 3, N'USER1SESSIONDELETED', '20160101 00:00:00.000', 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000' UNION ALL
SELECT 4, N'USER2SESSION', '20160101 00:00:00.000', 2, 2, '20150101 00:00:00.000', 2, '20150101 00:00:00.000', 0, NULL, NULL
GO

SET IDENTITY_INSERT [dbo].[Sessions] OFF
GO

SET IDENTITY_INSERT [dbo].[Transactions] ON
GO

INSERT INTO [dbo].[Transactions]([TransactionID], [Date], [Description], [Amount], [Category_CategoryID], [Account_AccountID], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [Deleted], [DeletedBy], [DeletedDate], [TransferGUID], [Note])
-- User 1 Current Account
SELECT 1, '20150101 00:00:00.000', N'USER1CURRENT1', 1000.00, 1, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, 'Salary' UNION ALL
SELECT 2, '20150101 00:00:00.000', N'USER1CURRENT2', -500.00, 2, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 3, '20150101 00:00:00.000', N'USER1CURRENT3', -10.00, 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 4, '20150102 00:00:00.000', N'USER1CURRENT4', -44.50, 4, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, 'Gas' UNION ALL
SELECT 5, '20150102 00:00:00.000', N'USER1CURRENT5', -10.00, 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 6, '20150103 00:00:00.000', N'USER1CURRENT6', -15.50, 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 7, '20150103 00:00:00.000', N'USER1CURRENT7', -20.00, 4, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, 'Mobile' UNION ALL
SELECT 8, '20150103 00:00:00.000', N'USER1CURRENT8', -18.00, 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 9, '20150103 00:00:00.000', N'USER1CURRENT9', -2.00, 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 10, '20150104 00:00:00.000', N'USER1CURRENT10', -30.00, 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 11, '20150105 00:00:00.000', N'USER1CURRENT11', -15.00, 4, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, 'Electricity' UNION ALL
SELECT 12, '20150105 00:00:00.000', N'USER1CURRENT12', -50.00, 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 13, '20150105 00:00:00.000', N'USER1CURRENT13', -10.00, 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 14, '20150106 00:00:00.000', N'USER1CURRENT14', -25.00, 4, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, 'Water' UNION ALL
SELECT 15, '20150103 00:00:00.000', N'USER1CURRENTDELETED', -250.00, 3, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000', NULL, NULL UNION ALL
-- User 1 Savings Account
SELECT 16, '20150101 00:00:00.000', N'USER1SAVINGS16', 10.00, NULL, 2, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 17, '20150101 00:00:00.000', N'USER1SAVINGS17', 20.25, NULL, 2, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 18, '20150102 00:00:00.000', N'USER1SAVINGS18', 10.25, NULL, 2, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 19, '20150103 00:00:00.000', N'USER1SAVINGS19', 9.50, NULL, 2, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 20, '20150102 00:00:00.000', N'USER1SAVINGSDELETED', -20.00, NULL, 2, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000', NULL, NULL UNION ALL
-- User 2 Current Account
SELECT 21, '20150101 00:00:00.000', N'USER2CURRENT21', 50.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 22, '20150101 00:00:00.000', N'USER2CURRENT22', -25.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 23, '20150102 00:00:00.000', N'USER2CURRENT23', -00.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 24, '20150102 00:00:00.000', N'USER2CURRENT24', -00.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 25, '20150103 00:00:00.000', N'USER2CURRENT25', -00.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 26, '20150103 00:00:00.000', N'USER2CURRENT26', -00.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 27, '20150103 00:00:00.000', N'USER2CURRENT27', -00.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 28, '20150104 00:00:00.000', N'USER2CURRENT28', -00.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 29, '20150104 00:00:00.000', N'USER2CURRENT29', -00.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL UNION ALL
SELECT 30, '20150102 00:00:00.000', N'USER2CURRENTDELETED', -00.00, NULL, 5, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000', NULL, NULL UNION ALL
-- User 1 unallocated
SELECT 31, '20150107 00:00:00.000', N'USER1CURRENT15', -5.00, NULL, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, NULL, NULL

SET IDENTITY_INSERT [dbo].[Transactions] OFF
GO

SET IDENTITY_INSERT [dbo].[Budgets] ON
GO

INSERT INTO [dbo].[Budgets]([BudgetID], [Start], [End], [Account_AccountID], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [Deleted], [DeletedBy], [DeletedDate])
SELECT 1, '20150101 00:00:00.000', '20150131 00:00:00.000', 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 2, '20150201 00:00:00.000', '20150228 00:00:00.000', 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 3, '20150101 00:00:00.000', '20150131 00:00:00.000', 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000' UNION ALL
SELECT 4, '20150101 00:00:00.000', '20150131 00:00:00.000', 5, 2, '20150101 00:00:00.000', 2, '20150101 00:00:00.000', 0, NULL, NULL
GO

SET IDENTITY_INSERT [dbo].[Budgets] OFF
GO

INSERT INTO [dbo].[Categories_Budgets]([Budget_BudgetID], [Category_CategoryID], [Amount], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [Deleted], [DeletedBy], [DeletedDate])
SELECT 1, 2, 500.00, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 1, 3, 250.00, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL UNION ALL
SELECT 1, 5, 100.00, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000' UNION ALL
SELECT 4, 9, 2000.00, 2, '20150101 00:00:00.000', 2, '20150101 00:00:00.000', 0, NULL, NULL

