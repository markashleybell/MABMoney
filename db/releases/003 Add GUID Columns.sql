IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Accounts' AND COLUMN_NAME = 'GUID') BEGIN

    ALTER TABLE [dbo].[Accounts] ADD [GUID] [uniqueidentifier] NULL

END
GO

UPDATE [dbo].[Accounts] SET [GUID] = NEWID() WHERE [GUID] IS NULL
GO

ALTER TABLE [dbo].[Accounts] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Budgets' AND COLUMN_NAME = 'GUID') BEGIN

    ALTER TABLE [dbo].[Budgets] ADD [GUID] [uniqueidentifier] NULL

END
GO

UPDATE [dbo].[Budgets] SET [GUID] = NEWID() WHERE [GUID] IS NULL
GO

ALTER TABLE [dbo].[Budgets] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Categories' AND COLUMN_NAME = 'GUID') BEGIN

    ALTER TABLE [dbo].[Categories] ADD [GUID] [uniqueidentifier] NULL

END
GO

UPDATE [dbo].[Categories] SET [GUID] = NEWID() WHERE [GUID] IS NULL
GO

ALTER TABLE [dbo].[Categories] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Categories_Budgets' AND COLUMN_NAME = 'GUID') BEGIN

    ALTER TABLE [dbo].[Categories_Budgets] ADD [GUID] [uniqueidentifier] NULL

END
GO

UPDATE [dbo].[Categories_Budgets] SET [GUID] = NEWID() WHERE [GUID] IS NULL
GO

ALTER TABLE [dbo].[Categories_Budgets] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Sessions' AND COLUMN_NAME = 'GUID') BEGIN

    ALTER TABLE [dbo].[Sessions] ADD [GUID] [uniqueidentifier] NULL

END
GO

UPDATE [dbo].[Sessions] SET [GUID] = NEWID() WHERE [GUID] IS NULL
GO

ALTER TABLE [dbo].[Sessions] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Transactions' AND COLUMN_NAME = 'GUID') BEGIN

    ALTER TABLE [dbo].[Transactions] ADD [GUID] [uniqueidentifier] NULL

END
GO

UPDATE [dbo].[Transactions] SET [GUID] = NEWID() WHERE [GUID] IS NULL
GO

ALTER TABLE [dbo].[Transactions] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'GUID') BEGIN

    ALTER TABLE [dbo].[Users] ADD [GUID] [uniqueidentifier] NULL

END
GO

UPDATE [dbo].[Users] SET [GUID] = NEWID() WHERE [GUID] IS NULL
GO

ALTER TABLE [dbo].[Users] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL
GO

