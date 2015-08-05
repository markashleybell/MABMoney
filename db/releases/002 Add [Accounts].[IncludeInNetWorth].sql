IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Accounts' AND COLUMN_NAME = 'IncludeInNetWorth') BEGIN

	ALTER TABLE [dbo].[Accounts] ADD [IncludeInNetWorth] [bit] NOT NULL CONSTRAINT [DF_dbo.Transactions_IncludeInNetWorth] DEFAULT 1

END
GO

IF OBJECT_ID('[dbo].[vAccounts]') IS NOT NULL
BEGIN 
    DROP VIEW [dbo].[vAccounts] 
END 
GO

CREATE VIEW [dbo].[vAccounts] AS
SELECT 
    *
FROM 
    [dbo].[Accounts]
WHERE 
    [Deleted] = 0
GO

