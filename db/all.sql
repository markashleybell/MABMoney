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
		[Deleted] [bit] NOT NULL CONSTRAINT [DF_dbo.Users_Deleted] DEFAULT 0,
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
		[Deleted] [bit] NOT NULL CONSTRAINT [DF_dbo.Categories_Deleted] DEFAULT 0,
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
		[Deleted] [bit] NOT NULL CONSTRAINT [DF_dbo.Transactions_Deleted] DEFAULT 0,
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
		[Deleted] [bit] NOT NULL CONSTRAINT [DF_dbo.Accounts_Deleted] DEFAULT 0,
		[DeletedBy] [int] NULL,
		[DeletedDate] [datetime] NULL,
		[Default] [bit] NOT NULL,
		[Type] [int] NOT NULL CONSTRAINT [DF_dbo.Accounts_Type] DEFAULT 0,
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
		[Deleted] [bit] NOT NULL CONSTRAINT [DF_dbo.Budgets_Deleted] DEFAULT 0,
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
		[Deleted] [bit] NOT NULL CONSTRAINT [DF_dbo.Categories_Budgets_Deleted] DEFAULT 0,
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
		[Deleted] [bit] NOT NULL CONSTRAINT [DF_dbo.Sessions_Deleted] DEFAULT 0,
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

IF OBJECT_ID('[dbo].[mm_Accounts_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Accounts_Read] 
END 
GO

CREATE PROC [dbo].[mm_Accounts_Read] 
    @UserID int,
    @AccountID int = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [AccountID], 
            [Name], 
            [StartingBalance], 
            [User_UserID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [Default], 
            [Type], 
            [TransactionDescriptionHistory], 
            [CurrentBalance], 
            [DisplayOrder] 
        FROM   
            [dbo].[vAccounts] 
        WHERE  
            [User_UserID] = @UserID
        AND
            [AccountID] = CASE WHEN @AccountID IS NULL THEN [AccountID] ELSE @AccountID END
        ORDER BY
            [DisplayOrder]

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Accounts_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Accounts_Create] 
END 
GO

CREATE PROC [dbo].[mm_Accounts_Create] 
    @UserID int,
    @Name nvarchar(MAX),
    @StartingBalance decimal(18, 2),
    @Default bit,
    @Type int,
    @DisplayOrder int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        DECLARE @Now datetime = GETDATE()
    
        INSERT INTO 
            [dbo].[Accounts] (
				[Name], 
				[StartingBalance], 
				[User_UserID], 
				[CreatedBy], 
				[CreatedDate], 
				[LastModifiedBy], 
				[LastModifiedDate], 
				[Default], 
				[Type], 
				[DisplayOrder]
			)
        SELECT 
            @Name, 
            @StartingBalance, 
            @UserID, 
            @UserID, 
            @Now, 
            @UserID, 
            @Now, 
            @Default, 
            @Type, 
            @DisplayOrder
        
        SELECT 
            [AccountID], 
            [Name], 
            [StartingBalance], 
            [User_UserID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate],
            [Default], 
            [Type], 
            [TransactionDescriptionHistory], 
            [CurrentBalance], 
            [DisplayOrder]
        FROM   
            [dbo].[vAccounts]
        WHERE  
            [User_UserID] = @UserID
        AND
            [AccountID] = SCOPE_IDENTITY()
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Accounts_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Accounts_Update] 
END 
GO

CREATE PROC [dbo].[mm_Accounts_Update] 
    @UserID int,
    @AccountID int,
    @Name nvarchar(MAX),
    @StartingBalance decimal(18, 2),
    @Default bit,
    @Type int,
    @TransactionDescriptionHistory nvarchar(MAX) = NULL,
    @DisplayOrder int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DECLARE @Now datetime = GETDATE()

        UPDATE 
            [dbo].[Accounts]
        SET    
            [Name] = @Name, 
            [StartingBalance] = @StartingBalance, 
            [LastModifiedBy] = @UserID, 
            [LastModifiedDate] = @Now, 
            [Default] = @Default, 
            [Type] = @Type, 
            [TransactionDescriptionHistory] = @TransactionDescriptionHistory, 
            [DisplayOrder] = @DisplayOrder
        WHERE  
            [User_UserID] = @UserID
        AND
            [AccountID] = @AccountID
        
        SELECT 
            [AccountID], 
            [Name], 
            [StartingBalance], 
            [User_UserID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [Default], 
            [Type], 
            [TransactionDescriptionHistory], 
            [CurrentBalance], 
            [DisplayOrder]
        FROM   
            [dbo].[vAccounts]
        WHERE  
            [User_UserID] = @UserID
        AND
            [AccountID] = @AccountID 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Accounts_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Accounts_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Accounts_Delete]
    @UserID int,
    @AccountID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE   
            [dbo].[Accounts]
        SET  
            [Deleted] = 1,
            [DeletedBy] = @UserID,
            [DeletedDate] = GETDATE()
        WHERE
            [User_UserID] = @UserID
        AND
            [AccountID] = @AccountID
            
        SELECT 
            [AccountID], 
            [Name], 
            [StartingBalance], 
            [User_UserID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [Default], 
            [Type], 
            [TransactionDescriptionHistory], 
            [CurrentBalance], 
            [DisplayOrder]
        FROM   
            [dbo].[Accounts]
        WHERE  
            [User_UserID] = @UserID
        AND
            [AccountID] = @AccountID 

    COMMIT
GO

----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

IF OBJECT_ID('[dbo].[mm_Budgets_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Budgets_Read] 
END 
GO

CREATE PROC [dbo].[mm_Budgets_Read] 
    @BudgetID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [BudgetID], 
            [Start], 
            [End], 
            [Account_AccountID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate] 
        FROM   
            [dbo].[Budgets] 
        WHERE  
            ([BudgetID] = @BudgetID OR @BudgetID IS NULL) 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Budgets_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Budgets_Create] 
END 
GO

CREATE PROC [dbo].[mm_Budgets_Create] 
    @Start datetime,
    @End datetime,
    @Account_AccountID int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        INSERT INTO 
            [dbo].[Budgets] (
                [Start], 
                [End], 
                [Account_AccountID], 
                [CreatedBy], 
                [CreatedDate], 
                [LastModifiedBy], 
                [LastModifiedDate], 
                [Deleted], 
                [DeletedBy], 
                [DeletedDate]
            )
        SELECT 
            @Start, 
            @End, 
            @Account_AccountID, 
            @CreatedBy, 
            @CreatedDate, 
            @LastModifiedBy, 
            @LastModifiedDate, 
            @Deleted, 
            @DeletedBy, 
            @DeletedDate
        
        SELECT 
            [BudgetID], 
            [Start], 
            [End], 
            [Account_AccountID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Budgets]
        WHERE  
            [BudgetID] = SCOPE_IDENTITY()
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Budgets_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Budgets_Update] 
END 
GO

CREATE PROC [dbo].[mm_Budgets_Update] 
    @BudgetID int,
    @Start datetime,
    @End datetime,
    @Account_AccountID int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE 
            [dbo].[Budgets]
        SET    
            [Start] = @Start, 
            [End] = @End, 
            [Account_AccountID] = @Account_AccountID, 
            [CreatedBy] = @CreatedBy, 
            [CreatedDate] = @CreatedDate, 
            [LastModifiedBy] = @LastModifiedBy, 
            [LastModifiedDate] = @LastModifiedDate, 
            [Deleted] = @Deleted, 
            [DeletedBy] = @DeletedBy, 
            [DeletedDate] = @DeletedDate
        WHERE  
            [BudgetID] = @BudgetID
        
        SELECT 
            [BudgetID], 
            [Start], 
            [End], 
            [Account_AccountID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Budgets]
        WHERE  
            [BudgetID] = @BudgetID 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Budgets_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Budgets_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Budgets_Delete] 
    @BudgetID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DELETE
        FROM   
            [dbo].[Budgets]
        WHERE  
            [BudgetID] = @BudgetID

    COMMIT
GO

----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

IF OBJECT_ID('[dbo].[mm_Categories_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Read] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Read] 
    @CategoryID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [CategoryID], 
            [Name], 
            [Account_AccountID], 
            [Type], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate] 
        FROM   
            [dbo].[Categories] 
        WHERE  
            ([CategoryID] = @CategoryID OR @CategoryID IS NULL) 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Create] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Create] 
    @Name nvarchar(MAX),
    @Account_AccountID int,
    @Type int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        INSERT INTO 
            [dbo].[Categories] (
                [Name], 
                [Account_AccountID], 
                [Type], 
                [CreatedBy], 
                [CreatedDate], 
                [LastModifiedBy], 
                [LastModifiedDate], 
                [Deleted], 
                [DeletedBy], 
                [DeletedDate]
            )
        SELECT 
            @Name, 
            @Account_AccountID, 
            @Type, 
            @CreatedBy, 
            @CreatedDate, 
            @LastModifiedBy, 
            @LastModifiedDate, 
            @Deleted, 
            @DeletedBy, 
            @DeletedDate
        
        SELECT 
            [CategoryID], 
            [Name], 
            [Account_AccountID], 
            [Type], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Categories]
        WHERE  
            [CategoryID] = SCOPE_IDENTITY()
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Update] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Update] 
    @CategoryID int,
    @Name nvarchar(MAX),
    @Account_AccountID int,
    @Type int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE 
            [dbo].[Categories]
        SET    
            [Name] = @Name, 
            [Account_AccountID] = @Account_AccountID, 
            [Type] = @Type, 
            [CreatedBy] = @CreatedBy, 
            [CreatedDate] = @CreatedDate, 
            [LastModifiedBy] = @LastModifiedBy, 
            [LastModifiedDate] = @LastModifiedDate, 
            [Deleted] = @Deleted, 
            [DeletedBy] = @DeletedBy, 
            [DeletedDate] = @DeletedDate
        WHERE  
            [CategoryID] = @CategoryID
        
        SELECT 
            [CategoryID], 
            [Name], 
            [Account_AccountID], 
            [Type], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Categories]
        WHERE  
            [CategoryID] = @CategoryID 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Delete] 
    @CategoryID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DELETE
        FROM   
            [dbo].[Categories]
        WHERE  
            [CategoryID] = @CategoryID

    COMMIT
GO

----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Read] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Read] 
    @Budget_BudgetID int,
    @Category_CategoryID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [Budget_BudgetID], 
            [Category_CategoryID], 
            [Amount], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate] 
        FROM   
            [dbo].[Categories_Budgets] 
        WHERE  
            ([Budget_BudgetID] = @Budget_BudgetID OR @Budget_BudgetID IS NULL) 
	    AND 
	        ([Category_CategoryID] = @Category_CategoryID OR @Category_CategoryID IS NULL) 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Create] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Create] 
    @Budget_BudgetID int,
    @Category_CategoryID int,
    @Amount decimal(18, 2),
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        INSERT INTO 
            [dbo].[Categories_Budgets] (
                [Budget_BudgetID], 
                [Category_CategoryID], 
                [Amount], 
                [CreatedBy], 
                [CreatedDate], 
                [LastModifiedBy], 
                [LastModifiedDate], 
                [Deleted], 
                [DeletedBy], 
                [DeletedDate]
            )
        SELECT 
            @Budget_BudgetID, 
            @Category_CategoryID, 
            @Amount, 
            @CreatedBy, 
            @CreatedDate, 
            @LastModifiedBy, 
            @LastModifiedDate, 
            @Deleted, 
            @DeletedBy, 
            @DeletedDate
        
        SELECT 
            [Budget_BudgetID], 
            [Category_CategoryID], 
            [Amount], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Categories_Budgets]
        WHERE  
            [Budget_BudgetID] = @Budget_BudgetID
	    AND 
	        [Category_CategoryID] = @Category_CategoryID
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Update] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Update] 
    @Budget_BudgetID int,
    @Category_CategoryID int,
    @Amount decimal(18, 2),
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE 
            [dbo].[Categories_Budgets]
        SET    
            [Budget_BudgetID] = @Budget_BudgetID, 
            [Category_CategoryID] = @Category_CategoryID, 
            [Amount] = @Amount, 
            [CreatedBy] = @CreatedBy, 
            [CreatedDate] = @CreatedDate, 
            [LastModifiedBy] = @LastModifiedBy, 
            [LastModifiedDate] = @LastModifiedDate, 
            [Deleted] = @Deleted, 
            [DeletedBy] = @DeletedBy, 
            [DeletedDate] = @DeletedDate
        WHERE  
            [Budget_BudgetID] = @Budget_BudgetID
	    AND 
	        [Category_CategoryID] = @Category_CategoryID
        
        SELECT 
            [Budget_BudgetID], 
            [Category_CategoryID], 
            [Amount], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Categories_Budgets]
        WHERE  
            [Budget_BudgetID] = @Budget_BudgetID
	    AND 
	        [Category_CategoryID] = @Category_CategoryID 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Delete] 
    @Budget_BudgetID int,
    @Category_CategoryID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DELETE
        FROM   
            [dbo].[Categories_Budgets]
        WHERE  
            [Budget_BudgetID] = @Budget_BudgetID
	    AND 
	        [Category_CategoryID] = @Category_CategoryID

    COMMIT
GO

----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

IF OBJECT_ID('[dbo].[mm_Sessions_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Sessions_Read] 
END 
GO

CREATE PROC [dbo].[mm_Sessions_Read] 
    @SessionID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [SessionID], 
            [Key], 
            [Expiry], 
            [User_UserID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate] 
        FROM   
            [dbo].[Sessions] 
        WHERE  
            ([SessionID] = @SessionID OR @SessionID IS NULL) 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Sessions_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Sessions_Create] 
END 
GO

CREATE PROC [dbo].[mm_Sessions_Create] 
    @Key nvarchar(MAX),
    @Expiry datetime,
    @User_UserID int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        INSERT INTO 
            [dbo].[Sessions] (
                [Key], 
                [Expiry], 
                [User_UserID], 
                [CreatedBy], 
                [CreatedDate], 
                [LastModifiedBy], 
                [LastModifiedDate], 
                [Deleted], 
                [DeletedBy], 
                [DeletedDate]
            )
        SELECT 
            @Key, 
            @Expiry, 
            @User_UserID, 
            @CreatedBy, 
            @CreatedDate, 
            @LastModifiedBy, 
            @LastModifiedDate, 
            @Deleted, 
            @DeletedBy, 
            @DeletedDate
        
        SELECT 
            [SessionID], 
            [Key], 
            [Expiry], 
            [User_UserID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Sessions]
        WHERE  
            [SessionID] = SCOPE_IDENTITY()
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Sessions_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Sessions_Update] 
END 
GO

CREATE PROC [dbo].[mm_Sessions_Update] 
    @SessionID int,
    @Key nvarchar(MAX),
    @Expiry datetime,
    @User_UserID int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE 
            [dbo].[Sessions]
        SET    
            [Key] = @Key, 
            [Expiry] = @Expiry, 
            [User_UserID] = @User_UserID, 
            [CreatedBy] = @CreatedBy, 
            [CreatedDate] = @CreatedDate, 
            [LastModifiedBy] = @LastModifiedBy, 
            [LastModifiedDate] = @LastModifiedDate, 
            [Deleted] = @Deleted, 
            [DeletedBy] = @DeletedBy, 
            [DeletedDate] = @DeletedDate
        WHERE  
            [SessionID] = @SessionID
        
        SELECT 
            [SessionID], 
            [Key], 
            [Expiry], 
            [User_UserID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Sessions]
        WHERE  
            [SessionID] = @SessionID 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Sessions_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Sessions_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Sessions_Delete] 
    @SessionID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DELETE
        FROM   
            [dbo].[Sessions]
        WHERE  
            [SessionID] = @SessionID

    COMMIT
GO

----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

IF OBJECT_ID('[dbo].[mm_Transactions_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Transactions_Read] 
END 
GO

CREATE PROC [dbo].[mm_Transactions_Read] 
    @TransactionID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [TransactionID], 
            [Date], 
            [Description], 
            [Amount], 
            [Category_CategoryID], 
            [Account_AccountID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [TransferGUID], 
            [Note] 
        FROM   
            [dbo].[Transactions] 
        WHERE  
            ([TransactionID] = @TransactionID OR @TransactionID IS NULL) 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Transactions_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Transactions_Create] 
END 
GO

CREATE PROC [dbo].[mm_Transactions_Create] 
    @Date datetime,
    @Description nvarchar(MAX) = NULL,
    @Amount decimal(18, 2),
    @Category_CategoryID int = NULL,
    @Account_AccountID int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL,
    @TransferGUID nvarchar(MAX) = NULL,
    @Note nvarchar(MAX) = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        INSERT INTO 
            [dbo].[Transactions] (
                [Date], 
                [Description], 
                [Amount], 
                [Category_CategoryID], 
                [Account_AccountID], 
                [CreatedBy], 
                [CreatedDate], 
                [LastModifiedBy], 
                [LastModifiedDate], 
                [Deleted], 
                [DeletedBy], 
                [DeletedDate], 
                [TransferGUID], 
                [Note]
            )
        SELECT 
            @Date, 
            @Description, 
            @Amount, 
            @Category_CategoryID, 
            @Account_AccountID, 
            @CreatedBy, 
            @CreatedDate, 
            @LastModifiedBy, 
            @LastModifiedDate, 
            @Deleted, 
            @DeletedBy, 
            @DeletedDate, 
            @TransferGUID, 
            @Note
        
        SELECT 
            [TransactionID], 
            [Date], 
            [Description], 
            [Amount], 
            [Category_CategoryID], 
            [Account_AccountID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [TransferGUID], 
            [Note]
        FROM   
            [dbo].[Transactions]
        WHERE  
            [TransactionID] = SCOPE_IDENTITY()
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Transactions_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Transactions_Update] 
END 
GO

CREATE PROC [dbo].[mm_Transactions_Update] 
    @TransactionID int,
    @Date datetime,
    @Description nvarchar(MAX) = NULL,
    @Amount decimal(18, 2),
    @Category_CategoryID int = NULL,
    @Account_AccountID int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL,
    @TransferGUID nvarchar(MAX) = NULL,
    @Note nvarchar(MAX) = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE 
            [dbo].[Transactions]
        SET    
            [Date] = @Date, 
            [Description] = @Description, 
            [Amount] = @Amount, 
            [Category_CategoryID] = @Category_CategoryID, 
            [Account_AccountID] = @Account_AccountID, 
            [CreatedBy] = @CreatedBy, 
            [CreatedDate] = @CreatedDate, 
            [LastModifiedBy] = @LastModifiedBy, 
            [LastModifiedDate] = @LastModifiedDate, 
            [Deleted] = @Deleted, 
            [DeletedBy] = @DeletedBy, 
            [DeletedDate] = @DeletedDate, 
            [TransferGUID] = @TransferGUID, 
            [Note] = @Note
        WHERE  
            [TransactionID] = @TransactionID
        
        SELECT 
            [TransactionID], 
            [Date], 
            [Description], 
            [Amount], 
            [Category_CategoryID], 
            [Account_AccountID], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [TransferGUID], 
            [Note]
        FROM   
            [dbo].[Transactions]
        WHERE  
            [TransactionID] = @TransactionID 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Transactions_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Transactions_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Transactions_Delete] 
    @TransactionID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DELETE
        FROM   
            [dbo].[Transactions]
        WHERE  
            [TransactionID] = @TransactionID

    COMMIT
GO

----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

IF OBJECT_ID('[dbo].[mm_Users_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Users_Read] 
END 
GO

CREATE PROC [dbo].[mm_Users_Read] 
    @UserID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [UserID], 
            [Forename], 
            [Surname], 
            [Email], 
            [Password], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [IsAdmin], 
            [PasswordResetGUID], 
            [PasswordResetExpiry] 
        FROM   
            [dbo].[vUsers] 
        WHERE  
            [UserID] = @UserID

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Users_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Users_Create] 
END 
GO

CREATE PROC [dbo].[mm_Users_Create] 
    @Forename nvarchar(MAX) = NULL,
    @Surname nvarchar(MAX) = NULL,
    @Email nvarchar(MAX),
    @Password nvarchar(MAX),
    @IsAdmin bit
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        DECLARE @Now datetime = GETDATE()
    
        INSERT INTO 
            [dbo].[Users] (
                [Forename], 
                [Surname], 
                [Email], 
                [Password], 
                [CreatedBy], 
                [CreatedDate], 
                [LastModifiedBy], 
                [LastModifiedDate], 
                [IsAdmin]
            )
        SELECT 
            @Forename, 
            @Surname, 
            @Email, 
            @Password, 
            -1, 
            @Now, 
            -1, 
            @Now, 
            @IsAdmin
        
        SELECT 
            [UserID], 
            [Forename], 
            [Surname], 
            [Email], 
            [Password], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [IsAdmin], 
            [PasswordResetGUID], 
            [PasswordResetExpiry]
        FROM   
            [dbo].[vUsers]
        WHERE  
            [UserID] = SCOPE_IDENTITY()
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Users_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Users_Update] 
END 
GO

CREATE PROC [dbo].[mm_Users_Update] 
    @UserID int,
    @Forename nvarchar(MAX) = NULL,
    @Surname nvarchar(MAX) = NULL,
    @Email nvarchar(MAX),
    @Password nvarchar(MAX),
    @PasswordResetGUID nvarchar(512) = NULL,
    @PasswordResetExpiry datetime = NULL,
    @IsAdmin bit
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

		DECLARE @Now datetime = GETDATE()
		
        UPDATE 
            [dbo].[Users]
        SET    
            [Forename] = @Forename, 
            [Surname] = @Surname, 
            [Email] = @Email, 
            [Password] = @Password, 
            [LastModifiedBy] = @UserID, 
            [LastModifiedDate] = @Now, 
            [IsAdmin] = @IsAdmin, 
            [PasswordResetGUID] = @PasswordResetGUID, 
            [PasswordResetExpiry] = @PasswordResetExpiry
        WHERE  
            [UserID] = @UserID
        
        SELECT 
            [UserID], 
            [Forename], 
            [Surname], 
            [Email], 
            [Password], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [IsAdmin], 
            [PasswordResetGUID], 
            [PasswordResetExpiry]
        FROM   
            [dbo].[vUsers]
        WHERE  
            [UserID] = @UserID 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Users_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Users_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Users_Delete] 
    @UserID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE   
            [dbo].[Users]
        SET  
            [Deleted] = 1,
            [DeletedBy] = @UserID,
            [DeletedDate] = GETDATE()
        WHERE
            [UserID] = @UserID
            
        SELECT 
            [UserID], 
            [Forename], 
            [Surname], 
            [Email], 
            [Password], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [IsAdmin], 
            [PasswordResetGUID], 
            [PasswordResetExpiry] 
        FROM   
            [dbo].[Users] 
        WHERE  
            [UserID] = @UserID

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Users_GetByEmailAddress]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Users_GetByEmailAddress] 
END 
GO

CREATE PROC [dbo].[mm_Users_GetByEmailAddress] 
    @Email nvarchar(256)
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [UserID], 
            [Forename], 
            [Surname], 
            [Email], 
            [Password], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [IsAdmin], 
            [PasswordResetGUID], 
            [PasswordResetExpiry] 
        FROM   
            [dbo].[vUsers] 
        WHERE  
            [Email] = @Email

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Users_GetByPasswordResetGUID]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Users_GetByPasswordResetGUID] 
END 
GO

CREATE PROC [dbo].[mm_Users_GetByPasswordResetGUID] 
    @GUID nvarchar(128)
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [UserID], 
            [Forename], 
            [Surname], 
            [Email], 
            [Password], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate], 
            [IsAdmin], 
            [PasswordResetGUID], 
            [PasswordResetExpiry] 
        FROM   
            [dbo].[vUsers] 
        WHERE  
            [PasswordResetGUID] = @GUID

    COMMIT
GO

----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

