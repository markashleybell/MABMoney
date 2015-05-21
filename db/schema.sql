IF OBJECT_ID('[dbo].[Users]') IS NULL
BEGIN
	CREATE TABLE [dbo].[Users] (
		[UserID] [int] IDENTITY(1,1) NOT NULL,
		[Forename] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
		[Surname] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
		[Email] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
		[Password] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
		[CreatedBy] [int] NOT NULL,
		[CreatedDate] [datetime] NOT NULL,
		[LastModifiedBy] [int] NOT NULL,
		[LastModifiedDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[DeletedBy] [int] NULL,
		[DeletedDate] [datetime] NULL,
		[IsAdmin] [bit] NOT NULL CONSTRAINT [DF_dbo.Users_IsAdmin] DEFAULT 0,
		[PasswordResetGUID] [nvarchar](512) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
		[PasswordResetExpiry] [datetime] NULL,
		CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED (
			[UserID] ASC
		) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
	)
END
GO

IF OBJECT_ID('[dbo].[Categories]') IS NULL
BEGIN
	CREATE TABLE [dbo].[Categories] (
		[CategoryID] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
		[Account_AccountID] [int] NOT NULL,
		[Type] [int] NOT NULL,
		[CreatedBy] [int] NOT NULL,
		[CreatedDate] [datetime] NOT NULL,
		[LastModifiedBy] [int] NOT NULL,
		[LastModifiedDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[DeletedBy] [int] NULL,
		[DeletedDate] [datetime] NULL,
		CONSTRAINT [PK_dbo.Categories] PRIMARY KEY CLUSTERED (
			[CategoryID] ASC
		) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
	)
END
GO

IF OBJECT_ID('[dbo].[Transactions]') IS NULL
BEGIN
	CREATE TABLE [dbo].[Transactions] (
		[TransactionID] [int] IDENTITY(1,1) NOT NULL,
		[Date] [datetime] NOT NULL,
		[Description] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
		[Amount] [decimal](18, 2) NOT NULL,
		[Category_CategoryID] [int] NULL,
		[Account_AccountID] [int] NOT NULL,
		[CreatedBy] [int] NOT NULL,
		[CreatedDate] [datetime] NOT NULL,
		[LastModifiedBy] [int] NOT NULL,
		[LastModifiedDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[DeletedBy] [int] NULL,
		[DeletedDate] [datetime] NULL,
		[TransferGUID] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
		[Note] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
		CONSTRAINT [PK_dbo.Transactions] PRIMARY KEY CLUSTERED (
			[TransactionID] ASC
		) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
	)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Transactions]') AND name = N'IX_dbo.Transactions_Date_Account_AccountID_Deleted')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_dbo.Transactions_Date_Account_AccountID_Deleted] ON [dbo].[Transactions] (
		[Date] ASC,
		[Account_AccountID] ASC,
		[Deleted] ASC
	)
	INCLUDE ( 
		[TransactionID],
		[Description],
		[Amount],
		[Category_CategoryID],
		[CreatedBy],
		[CreatedDate],
		[LastModifiedBy],
		[LastModifiedDate],
		[DeletedBy],
		[DeletedDate],
		[TransferGUID]
	) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
END
GO

IF OBJECT_ID(N'[dbo].[Transactions_UpdateAccountBalance]') IS NOT NULL
BEGIN
	DROP TRIGGER [dbo].[Transactions_UpdateAccountBalance]
END
GO

CREATE TRIGGER [dbo].[Transactions_UpdateAccountBalance]
ON [dbo].[Transactions]
FOR INSERT, UPDATE, DELETE
AS
BEGIN
	
	DECLARE @DeletedFromAccountID int, @InsertedIntoAccountID int, @AccountID int

	SELECT @DeletedFromAccountID = [Account_AccountID] FROM DELETED
	SELECT @InsertedIntoAccountID = [Account_AccountID] FROM INSERTED
	
	IF @DeletedFromAccountID IS NOT NULL AND @InsertedIntoAccountID IS NULL
	BEGIN
		-- PRINT ''DELETED transaction from account '' + CAST(@DeletedFromAccountID AS nvarchar(10))
		SET @AccountID = @DeletedFromAccountID
	END
	
	IF @DeletedFromAccountID IS NOT NULL AND @InsertedIntoAccountID IS NOT NULL
	BEGIN
		-- PRINT ''UPDATED transaction in account '' + CAST(@DeletedFromAccountID AS nvarchar(10))
		SET @AccountID = @InsertedIntoAccountID
	END
	
	IF @DeletedFromAccountID IS NULL AND @InsertedIntoAccountID IS NOT NULL
	BEGIN
		-- PRINT ''INSERTED transaction into account '' + CAST(@InsertedIntoAccountID AS nvarchar(10))
		SET @AccountID = @InsertedIntoAccountID
	END
		
	UPDATE Accounts SET CurrentBalance = StartingBalance + ISNULL((SELECT SUM(Amount) FROM Transactions WHERE Deleted = 0 AND Account_AccountID = @AccountID), 0) WHERE AccountID = @AccountID
	
END
GO

IF OBJECT_ID(N'[dbo].[Accounts]') IS NULL
BEGIN
	CREATE TABLE [dbo].[Accounts] (
		[AccountID] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
		[StartingBalance] [decimal](18, 2) NOT NULL,
		[User_UserID] [int] NOT NULL,
		[CreatedBy] [int] NOT NULL,
		[CreatedDate] [datetime] NOT NULL,
		[LastModifiedBy] [int] NOT NULL,
		[LastModifiedDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[DeletedBy] [int] NULL,
		[DeletedDate] [datetime] NULL,
		[Default] [bit] NOT NULL,
		[Type] [int] NOT NULL CONSTRAINT [DF_dbo.Transactions_Type] DEFAULT 0,
		[TransactionDescriptionHistory] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
		[CurrentBalance] [decimal](18, 2) NOT NULL CONSTRAINT [DF_dbo.Transactions_CurrentBalance] DEFAULT 0,
		[DisplayOrder] [int] NOT NULL CONSTRAINT [DF_dbo.Transactions_DisplayOrder] DEFAULT 0,
		CONSTRAINT [PK_dbo.Accounts] PRIMARY KEY CLUSTERED (
			[AccountID] ASC
		) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
	)
END
GO

IF OBJECT_ID(N'[dbo].[Accounts_UpdateAccountBalance]') IS NOT NULL
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Accounts_UpdateAccountBalance]'))
BEGIN
	DROP TRIGGER [dbo].[Accounts_UpdateAccountBalance]
END
GO

CREATE TRIGGER [dbo].[Accounts_UpdateAccountBalance]
ON [dbo].[Accounts]
FOR INSERT, UPDATE, DELETE
AS
BEGIN
	
	DECLARE @DeletedFromAccountID int, @InsertedIntoAccountID int, @AccountID int

	SELECT @DeletedFromAccountID = [AccountID] FROM DELETED
	SELECT @InsertedIntoAccountID = [AccountID] FROM INSERTED
	
	IF @DeletedFromAccountID IS NOT NULL AND @InsertedIntoAccountID IS NULL
	BEGIN
		-- PRINT ''DELETED transaction from account '' + CAST(@DeletedFromAccountID AS nvarchar(10))
		SET @AccountID = @DeletedFromAccountID
	END
	
	IF @DeletedFromAccountID IS NOT NULL AND @InsertedIntoAccountID IS NOT NULL
	BEGIN
		-- PRINT ''UPDATED transaction in account '' + CAST(@DeletedFromAccountID AS nvarchar(10))
		SET @AccountID = @InsertedIntoAccountID
	END
	
	IF @DeletedFromAccountID IS NULL AND @InsertedIntoAccountID IS NOT NULL
	BEGIN
		-- PRINT ''INSERTED transaction into account '' + CAST(@InsertedIntoAccountID AS nvarchar(10))
		SET @AccountID = @InsertedIntoAccountID
	END
		
	UPDATE Accounts SET CurrentBalance = StartingBalance + ISNULL((SELECT SUM(Amount) FROM Transactions WHERE Deleted = 0 AND Account_AccountID = @AccountID), 0) WHERE AccountID = @AccountID
	
END
GO

IF OBJECT_ID(N'[dbo].[Budgets]') IS NULL
BEGIN
	CREATE TABLE [dbo].[Budgets](
		[BudgetID] [int] IDENTITY(1,1) NOT NULL,
		[Start] [datetime] NOT NULL,
		[End] [datetime] NOT NULL,
		[Account_AccountID] [int] NOT NULL,
		[CreatedBy] [int] NOT NULL,
		[CreatedDate] [datetime] NOT NULL,
		[LastModifiedBy] [int] NOT NULL,
		[LastModifiedDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[DeletedBy] [int] NULL,
		[DeletedDate] [datetime] NULL,
		CONSTRAINT [PK_dbo.Budgets] PRIMARY KEY CLUSTERED (
			[BudgetID] ASC
		) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
	)
END
GO

IF OBJECT_ID(N'[dbo].[Categories_Budgets]') IS NULL
BEGIN
	CREATE TABLE [dbo].[Categories_Budgets](
		[Budget_BudgetID] [int] NOT NULL,
		[Category_CategoryID] [int] NOT NULL,
		[Amount] [decimal](18, 2) NOT NULL,
		[CreatedBy] [int] NOT NULL,
		[CreatedDate] [datetime] NOT NULL,
		[LastModifiedBy] [int] NOT NULL,
		[LastModifiedDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[DeletedBy] [int] NULL,
		[DeletedDate] [datetime] NULL,
		CONSTRAINT [PK_dbo.Categories_Budgets] PRIMARY KEY CLUSTERED (
			[Budget_BudgetID] ASC,
			[Category_CategoryID] ASC
		) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
	)
END
GO

IF OBJECT_ID(N'[dbo].[Sessions]') IS NULL
BEGIN
	CREATE TABLE [dbo].[Sessions](
		[SessionID] [int] IDENTITY(1,1) NOT NULL,
		[Key] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
		[Expiry] [datetime] NOT NULL,
		[User_UserID] [int] NOT NULL,
		[CreatedBy] [int] NOT NULL,
		[CreatedDate] [datetime] NOT NULL,
		[LastModifiedBy] [int] NOT NULL,
		[LastModifiedDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[DeletedBy] [int] NULL,
		[DeletedDate] [datetime] NULL,
		CONSTRAINT [PK_dbo.Sessions] PRIMARY KEY CLUSTERED (
			[SessionID] ASC
		) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
	)
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Categories_dbo.Accounts_Account_AccountID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Categories]'))
    ALTER TABLE [dbo].[Categories] WITH NOCHECK ADD CONSTRAINT [FK_dbo.Categories_dbo.Accounts_Account_AccountID] FOREIGN KEY([Account_AccountID])
    REFERENCES [dbo].[Accounts] ([AccountID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Categories_dbo.Accounts_Account_AccountID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Categories]'))
    ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_dbo.Categories_dbo.Accounts_Account_AccountID]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Transactions_dbo.Accounts_Account_AccountID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
    ALTER TABLE [dbo].[Transactions] WITH NOCHECK ADD CONSTRAINT [FK_dbo.Transactions_dbo.Accounts_Account_AccountID] FOREIGN KEY([Account_AccountID])
    REFERENCES [dbo].[Accounts] ([AccountID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Transactions_dbo.Accounts_Account_AccountID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
    ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.Accounts_Account_AccountID]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Transactions_dbo.Categories_Category_CategoryID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
    ALTER TABLE [dbo].[Transactions] WITH NOCHECK ADD CONSTRAINT [FK_dbo.Transactions_dbo.Categories_Category_CategoryID] FOREIGN KEY([Category_CategoryID])
    REFERENCES [dbo].[Categories] ([CategoryID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Transactions_dbo.Categories_Category_CategoryID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
    ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.Categories_Category_CategoryID]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Accounts_dbo.Users_User_UserID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Accounts]'))
    ALTER TABLE [dbo].[Accounts] WITH NOCHECK ADD CONSTRAINT [FK_dbo.Accounts_dbo.Users_User_UserID] FOREIGN KEY([User_UserID])
    REFERENCES [dbo].[Users] ([UserID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Accounts_dbo.Users_User_UserID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Accounts]'))
    ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_dbo.Accounts_dbo.Users_User_UserID]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Budgets_dbo.Accounts_Account_AccountID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Budgets]'))
    ALTER TABLE [dbo].[Budgets] WITH NOCHECK ADD CONSTRAINT [FK_dbo.Budgets_dbo.Accounts_Account_AccountID] FOREIGN KEY([Account_AccountID])
    REFERENCES [dbo].[Accounts] ([AccountID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Budgets_dbo.Accounts_Account_AccountID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Budgets]'))
    ALTER TABLE [dbo].[Budgets] CHECK CONSTRAINT [FK_dbo.Budgets_dbo.Accounts_Account_AccountID]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Categories_Budgets_dbo.Budgets_Budget_BudgetID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Categories_Budgets]'))
    ALTER TABLE [dbo].[Categories_Budgets] WITH NOCHECK ADD CONSTRAINT [FK_dbo.Categories_Budgets_dbo.Budgets_Budget_BudgetID] FOREIGN KEY([Budget_BudgetID])
    REFERENCES [dbo].[Budgets] ([BudgetID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Categories_Budgets_dbo.Budgets_Budget_BudgetID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Categories_Budgets]'))
    ALTER TABLE [dbo].[Categories_Budgets] CHECK CONSTRAINT [FK_dbo.Categories_Budgets_dbo.Budgets_Budget_BudgetID]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Categories_Budgets_dbo.Categories_Category_CategoryID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Categories_Budgets]'))
    ALTER TABLE [dbo].[Categories_Budgets] WITH NOCHECK ADD CONSTRAINT [FK_dbo.Categories_Budgets_dbo.Categories_Category_CategoryID] FOREIGN KEY([Category_CategoryID])
    REFERENCES [dbo].[Categories] ([CategoryID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Categories_Budgets_dbo.Categories_Category_CategoryID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Categories_Budgets]'))
    ALTER TABLE [dbo].[Categories_Budgets] CHECK CONSTRAINT [FK_dbo.Categories_Budgets_dbo.Categories_Category_CategoryID]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Sessions_dbo.Users_User_UserID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Sessions]'))
    ALTER TABLE [dbo].[Sessions] WITH CHECK ADD CONSTRAINT [FK_dbo.Sessions_dbo.Users_User_UserID] FOREIGN KEY([User_UserID])
    REFERENCES [dbo].[Users] ([UserID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Sessions_dbo.Users_User_UserID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Sessions]'))
    ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_dbo.Sessions_dbo.Users_User_UserID]
GO

IF OBJECT_ID('[dbo].[vUsers]') IS NOT NULL
BEGIN 
    DROP VIEW [dbo].[vUsers] 
END 
GO

CREATE VIEW [dbo].[vUsers] AS
SELECT 
    *
FROM 
    [dbo].[Users]
WHERE 
    [Deleted] = 0
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

