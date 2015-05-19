IF OBJECT_ID('[dbo].[mm_Accounts_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Accounts_Read] 
END 
GO

CREATE PROC [dbo].[mm_Accounts_Read] 
    @AccountID int
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
            [dbo].[Accounts] 
        WHERE  
            ([AccountID] = @AccountID OR @AccountID IS NULL) 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Accounts_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Accounts_Create] 
END 
GO

CREATE PROC [dbo].[mm_Accounts_Create] 
    @Name nvarchar(MAX),
    @StartingBalance decimal(18, 2),
    @User_UserID int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL,
    @Default bit,
    @Type int,
    @TransactionDescriptionHistory nvarchar(MAX) = NULL,
    @CurrentBalance decimal(18, 2),
    @DisplayOrder int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        INSERT INTO 
            [dbo].[Accounts] (
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
			)
        SELECT 
            @Name, 
            @StartingBalance, 
            @User_UserID, 
            @CreatedBy, 
            @CreatedDate, 
            @LastModifiedBy, 
            @LastModifiedDate, 
            @Deleted, 
            @DeletedBy, 
            @DeletedDate, 
            @Default, 
            @Type, 
            @TransactionDescriptionHistory, 
            @CurrentBalance, 
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
            [dbo].[Accounts]
        WHERE  
            [AccountID] = SCOPE_IDENTITY()
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Accounts_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Accounts_Update] 
END 
GO

CREATE PROC [dbo].[mm_Accounts_Update] 
    @AccountID int,
    @Name nvarchar(MAX),
    @StartingBalance decimal(18, 2),
    @User_UserID int,
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL,
    @Default bit,
    @Type int,
    @TransactionDescriptionHistory nvarchar(MAX) = NULL,
    @CurrentBalance decimal(18, 2),
    @DisplayOrder int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE 
            [dbo].[Accounts]
        SET    
            [Name] = @Name, 
            [StartingBalance] = @StartingBalance, 
            [User_UserID] = @User_UserID, 
            [CreatedBy] = @CreatedBy, 
            [CreatedDate] = @CreatedDate, 
            [LastModifiedBy] = @LastModifiedBy, 
            [LastModifiedDate] = @LastModifiedDate, 
            [Deleted] = @Deleted, 
            [DeletedBy] = @DeletedBy, 
            [DeletedDate] = @DeletedDate, 
            [Default] = @Default, 
            [Type] = @Type, 
            [TransactionDescriptionHistory] = @TransactionDescriptionHistory, 
            [CurrentBalance] = @CurrentBalance, 
            [DisplayOrder] = @DisplayOrder
        WHERE  
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
            [AccountID] = @AccountID 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Accounts_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Accounts_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Accounts_Delete] 
    @AccountID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DELETE
        FROM   
            [dbo].[Accounts]
        WHERE  
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
            [dbo].[Users] 
        WHERE  
            ([UserID] = @UserID OR @UserID IS NULL) 

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
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL,
    @IsAdmin bit,
    @PasswordResetGUID nvarchar(512) = NULL,
    @PasswordResetExpiry datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
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
                [Deleted], 
                [DeletedBy], 
                [DeletedDate], 
                [IsAdmin], 
                [PasswordResetGUID], 
                [PasswordResetExpiry]
            )
        SELECT 
            @Forename, 
            @Surname, 
            @Email, 
            @Password, 
            @CreatedBy, 
            @CreatedDate, 
            @LastModifiedBy, 
            @LastModifiedDate, 
            @Deleted, 
            @DeletedBy, 
            @DeletedDate, 
            @IsAdmin, 
            @PasswordResetGUID, 
            @PasswordResetExpiry
        
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
    @CreatedBy int,
    @CreatedDate datetime,
    @LastModifiedBy int,
    @LastModifiedDate datetime,
    @Deleted bit,
    @DeletedBy int = NULL,
    @DeletedDate datetime = NULL,
    @IsAdmin bit,
    @PasswordResetGUID nvarchar(512) = NULL,
    @PasswordResetExpiry datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE 
            [dbo].[Users]
        SET    
            [Forename] = @Forename, 
            [Surname] = @Surname, 
            [Email] = @Email, 
            [Password] = @Password, 
            [CreatedBy] = @CreatedBy, 
            [CreatedDate] = @CreatedDate, 
            [LastModifiedBy] = @LastModifiedBy, 
            [LastModifiedDate] = @LastModifiedDate, 
            [Deleted] = @Deleted, 
            [DeletedBy] = @DeletedBy, 
            [DeletedDate] = @DeletedDate, 
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
            [dbo].[Users]
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

        DELETE
        FROM   
            [dbo].[Users]
        WHERE  
            [UserID] = @UserID

    COMMIT
GO

----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

