SET IDENTITY_INSERT [dbo].[Users] ON
GO

INSERT INTO [dbo].[Users]([UserID], [Forename], [Surname], [Email], [Password], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [Deleted], [DeletedBy], [DeletedDate], [IsAdmin], [PasswordResetGUID], [PasswordResetExpiry])
SELECT 1, N'Test', N'User', N'user@test.com', N'AJNzdwx56R+U3ls50NZbLTYQBm8j5Txr+F9mz3jQwzNjjIYjIjFuwBr/2l5VnjhQnw==', -1, '20150101 00:00:00.000', -1, '20150101 00:00:00.000', 0, NULL, NULL, 0, N'7cc68dbb-3d12-487b-8295-e9b226cda017', '20160101 00:00:00.000'
GO

SET IDENTITY_INSERT [dbo].[Users] OFF
GO

SET IDENTITY_INSERT [dbo].[Accounts] ON
GO

INSERT INTO [dbo].[Accounts]([AccountID], [Name], [StartingBalance], [User_UserID], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [Deleted], [DeletedBy], [DeletedDate], [Default], [Type], [TransactionDescriptionHistory], [CurrentBalance], [DisplayOrder])
SELECT 1, N'Current', 100.00, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, 1, 0, N'TEST1CURRENT|TEST2CURRENT', 100.00, 100 UNION ALL
SELECT 2, N'Savings', 500.00, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, 0, 1, N'TEST1SAVINGS|TEST2SAVINGS', 500.00, 200 UNION ALL
SELECT 3, N'Credit', 250.00, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 0, NULL, NULL, 0, 2, N'TEST1CREDIT|TEST2CREDIT', 250.00, 300 UNION ALL
SELECT 4, N'Deleted', 0.00, 1, 1, '20150101 00:00:00.000', 1, '20150101 00:00:00.000', 1, 1, '20150101 00:00:00.000', 0, 0, N'TEST1DELETED|TEST2DELETED', 0.00, 400
GO

SET IDENTITY_INSERT [dbo].[Accounts] OFF
GO

