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
    @UserID int,
    @BudgetID int = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [b].[BudgetID], 
            [b].[Start], 
            [b].[End], 
            [b].[Account_AccountID], 
            [b].[CreatedBy], 
            [b].[CreatedDate], 
            [b].[LastModifiedBy], 
            [b].[LastModifiedDate], 
            [b].[Deleted], 
            [b].[DeletedBy], 
            [b].[DeletedDate] 
        FROM   
            [dbo].[vBudgets] [b]
        INNER JOIN
            [dbo].[vAccounts] [a] ON [a].[AccountID] = [b].[Account_AccountID]
        WHERE  
            [a].[User_UserID] = @UserID
        AND
            [b].[BudgetID] = CASE WHEN @BudgetID IS NULL THEN [b].[BudgetID] ELSE @BudgetID END
        ORDER BY
            [b].[Start] DESC

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Budgets_Read_Latest]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Budgets_Read_Latest] 
END 
GO

CREATE PROC [dbo].[mm_Budgets_Read_Latest] 
    @UserID int,
    @AccountID int,
    @Date datetime
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN
    
		IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            WHERE 
                [a].[User_UserID] = @UserID 
            AND 
                [a].[AccountID] = @AccountID
        )
        BEGIN

			SELECT TOP 1
				[b].[BudgetID], 
				[b].[Start], 
				[b].[End], 
				[b].[Account_AccountID], 
				[b].[CreatedBy], 
				[b].[CreatedDate], 
				[b].[LastModifiedBy], 
				[b].[LastModifiedDate], 
				[b].[Deleted], 
				[b].[DeletedBy], 
				[b].[DeletedDate] 
			FROM   
				[dbo].[vBudgets] [b]
			INNER JOIN
				[dbo].[vAccounts] [a] ON [a].[AccountID] = [b].[Account_AccountID]
			WHERE  
				[a].[User_UserID] = @UserID
			AND 
                [a].[AccountID] = @AccountID
            AND
                [b].[End] >= @Date
			ORDER BY
				[b].[BudgetID] DESC
		
		END

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Budgets_Count]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Budgets_Count] 
END 
GO

CREATE PROC [dbo].[mm_Budgets_Count] 
    @UserID int,
    @AccountID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN
    
		IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            WHERE 
                [a].[User_UserID] = @UserID 
            AND 
                [a].[AccountID] = @AccountID
        )
        BEGIN

			SELECT 
				COUNT(*) 
			FROM   
				[dbo].[vBudgets] [b]
			INNER JOIN
				[dbo].[vAccounts] [a] ON [a].[AccountID] = [b].[Account_AccountID]
			WHERE  
				[a].[User_UserID] = @UserID
			AND 
                [a].[AccountID] = @AccountID
                
		END

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Budgets_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Budgets_Create] 
END 
GO

CREATE PROC [dbo].[mm_Budgets_Create] 
    @UserID int,
    @Start datetime,
    @End datetime,
    @Account_AccountID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            WHERE 
                [a].[User_UserID] = @UserID 
            AND 
                [a].[AccountID] = @Account_AccountID
        )
        BEGIN
            
            DECLARE @Now datetime = GETDATE()
    
            INSERT INTO 
                [dbo].[Budgets] (
                    [Start], 
                    [End], 
                    [Account_AccountID], 
                    [CreatedBy], 
                    [CreatedDate], 
                    [LastModifiedBy], 
                    [LastModifiedDate]
                )
            SELECT 
                @Start, 
                @End, 
                @Account_AccountID, 
                @UserID, 
                @Now,
                @UserID,
                @Now
            
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
                [dbo].[vBudgets]
            WHERE  
                [BudgetID] = SCOPE_IDENTITY()
            
        END
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Budgets_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Budgets_Update] 
END 
GO

CREATE PROC [dbo].[mm_Budgets_Update] 
    @UserID int,
    @BudgetID int,
    @Start datetime,
    @End datetime,
    @Account_AccountID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            WHERE 
                [a].[User_UserID] = @UserID 
            AND 
                [a].[AccountID] = @Account_AccountID
        )
        BEGIN

            DECLARE @Now datetime = GETDATE()

            UPDATE 
                [dbo].[Budgets]
            SET    
                [Start] = @Start, 
                [End] = @End, 
                [Account_AccountID] = @Account_AccountID, 
                [LastModifiedBy] = @UserID, 
                [LastModifiedDate] = @Now
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
                [dbo].[vBudgets]
            WHERE  
                [BudgetID] = @BudgetID 
                
        END

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Budgets_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Budgets_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Budgets_Delete] 
    @UserID int,
    @BudgetID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        IF EXISTS(
            SELECT 
                [a].[AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            INNER JOIN
                [dbo].[vBudgets] [b] ON [b].[Account_AccountID] = [a].[AccountID] AND [b].[BudgetID] = @BudgetID
            WHERE 
                [a].[User_UserID] = @UserID 
        )
        BEGIN

            UPDATE   
                [b]
            SET  
                [b].[Deleted] = 1,
                [b].[DeletedBy] = @UserID,
                [b].[DeletedDate] = GETDATE()
            FROM
                [dbo].[vBudgets] AS [b]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [b].[Account_AccountID]
            WHERE
                [a].[User_UserID] = @UserID
            AND
                [b].[BudgetID] = @BudgetID
                
            SELECT 
                [b].[BudgetID], 
                [b].[Start], 
                [b].[End], 
                [b].[Account_AccountID], 
                [b].[CreatedBy], 
                [b].[CreatedDate], 
                [b].[LastModifiedBy], 
                [b].[LastModifiedDate], 
                [b].[Deleted], 
                [b].[DeletedBy], 
                [b].[DeletedDate]
            FROM   
                [dbo].[Budgets] [b]
            INNER JOIN
                [dbo].[Accounts] [a] ON [a].[AccountID] = [b].[Account_AccountID]
            WHERE  
                [a].[User_UserID] = @UserID
            AND
                [b].[BudgetID] = @BudgetID

        END

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
    @UserID int,
    @AccountID int = NULL,
    @CategoryID int = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [c].[CategoryID], 
            [c].[Name], 
            [c].[Account_AccountID], 
            [c].[Type], 
            [c].[CreatedBy], 
            [c].[CreatedDate], 
            [c].[LastModifiedBy], 
            [c].[LastModifiedDate], 
            [c].[Deleted], 
            [c].[DeletedBy], 
            [c].[DeletedDate],
            [a].[Name] AS [AccountName]
        FROM   
            [dbo].[vCategories] [c]
        INNER JOIN
            [dbo].[vAccounts] [a] ON [a].[AccountID] = [c].[Account_AccountID]
        WHERE  
            [a].[User_UserID] = @UserID
        AND
            [c].[CategoryID] = CASE WHEN @CategoryID IS NULL THEN [c].[CategoryID] ELSE @CategoryID END
        AND
            [a].[AccountID] = CASE WHEN @AccountID IS NULL THEN [a].[AccountID] ELSE @AccountID END
        ORDER BY
            [c].[CategoryID]

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Create] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Create] 
    @UserID int,
    @Name nvarchar(MAX),
    @Account_AccountID int,
    @Type int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
        
        IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            WHERE 
                [a].[User_UserID] = @UserID 
            AND 
                [a].[AccountID] = @Account_AccountID
        )
        BEGIN
            
            DECLARE @Now datetime = GETDATE()
            
            INSERT INTO 
                [dbo].[Categories] (
                    [Name], 
                    [Account_AccountID], 
                    [Type], 
                    [CreatedBy], 
                    [CreatedDate], 
                    [LastModifiedBy], 
                    [LastModifiedDate]
                )
            SELECT 
                @Name, 
                @Account_AccountID, 
                @Type, 
                @UserID, 
                @Now, 
                @UserID, 
                @Now
            
            SELECT 
                [c].[CategoryID], 
                [c].[Name], 
                [c].[Account_AccountID], 
                [c].[Type], 
                [c].[CreatedBy], 
                [c].[CreatedDate], 
                [c].[LastModifiedBy], 
                [c].[LastModifiedDate], 
                [c].[Deleted], 
                [c].[DeletedBy], 
                [c].[DeletedDate],
                [a].[Name] AS [AccountName]
            FROM   
                [dbo].[vCategories] [c]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [c].[Account_AccountID]
            WHERE  
                [c].[CategoryID] = SCOPE_IDENTITY()
                
        END 
          
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Update] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Update] 
    @UserID int,
    @CategoryID int,
    @Name nvarchar(MAX),
    @Account_AccountID int,
    @Type int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
        
        IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            WHERE 
                [a].[User_UserID] = @UserID 
            AND 
                [a].[AccountID] = @Account_AccountID
        )
        BEGIN

            DECLARE @Now datetime = GETDATE()
            
            UPDATE 
                [dbo].[Categories]
            SET    
                [Name] = @Name, 
                [Account_AccountID] = @Account_AccountID, 
                [Type] = @Type, 
                [LastModifiedBy] = @UserID, 
                [LastModifiedDate] = @Now
            WHERE  
                [CategoryID] = @CategoryID
            
            SELECT 
                [c].[CategoryID], 
                [c].[Name], 
                [c].[Account_AccountID], 
                [c].[Type], 
                [c].[CreatedBy], 
                [c].[CreatedDate], 
                [c].[LastModifiedBy], 
                [c].[LastModifiedDate], 
                [c].[Deleted], 
                [c].[DeletedBy], 
                [c].[DeletedDate],
                [a].[Name] AS [AccountName]
            FROM   
                [dbo].[vCategories] [c]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [c].[Account_AccountID]
            WHERE  
                [c].[CategoryID] = @CategoryID 
            
        END

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Delete] 
    @UserID int,
    @CategoryID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        IF EXISTS(
            SELECT 
                [a].[AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            INNER JOIN
                [dbo].[vCategories] [c] ON [c].[Account_AccountID] = [a].[AccountID] AND [c].[CategoryID] = @CategoryID
            WHERE 
                [a].[User_UserID] = @UserID 
        )
        BEGIN

            UPDATE   
                [c]
            SET  
                [c].[Deleted] = 1,
                [c].[DeletedBy] = @UserID,
                [c].[DeletedDate] = GETDATE()
            FROM
                [dbo].[vCategories] AS [c]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [c].[Account_AccountID]
            WHERE
                [a].[User_UserID] = @UserID
            AND
                [c].[CategoryID] = @CategoryID
                
            SELECT 
                [c].[CategoryID], 
                [c].[Name], 
                [c].[Account_AccountID], 
                [c].[Type], 
                [c].[CreatedBy], 
                [c].[CreatedDate], 
                [c].[LastModifiedBy], 
                [c].[LastModifiedDate], 
                [c].[Deleted], 
                [c].[DeletedBy], 
                [c].[DeletedDate],
                [a].[Name] AS [AccountName]
            FROM   
                [dbo].[Categories] [c]
            INNER JOIN
                [dbo].[Accounts] [a] ON [a].[AccountID] = [c].[Account_AccountID]
            WHERE  
                [a].[User_UserID] = @UserID
            AND
                [c].[CategoryID] = @CategoryID

        END
        
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
    @UserID int,
    @Budget_BudgetID int,
    @Category_CategoryID int = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [cb].[Budget_BudgetID], 
            [cb].[Category_CategoryID], 
            [cb].[Amount], 
            [cb].[CreatedBy], 
            [cb].[CreatedDate], 
            [cb].[LastModifiedBy], 
            [cb].[LastModifiedDate], 
            [cb].[Deleted], 
            [cb].[DeletedBy], 
            [cb].[DeletedDate],
            [c].[Name] AS CategoryName,
            [c].[Type] AS CategoryType
        FROM   
            [dbo].[vCategories_Budgets] [cb]
        INNER JOIN
            [dbo].[vCategories] [c] ON [c].[CategoryID] = [cb].[Category_CategoryID]
        INNER JOIN
            [dbo].[vBudgets] [b] ON [b].[BudgetID] = [cb].[Budget_BudgetID]
        INNER JOIN
            [dbo].[vAccounts] [a] ON [a].[AccountID] = [c].[Account_AccountID] AND [a].[AccountID] = [b].[Account_AccountID]
        WHERE 
            [a].[User_UserID] = @UserID
        AND
            [cb].[Budget_BudgetID] = @Budget_BudgetID
	    AND 
	        [cb].[Category_CategoryID] = CASE WHEN @Category_CategoryID IS NULL THEN [cb].[Category_CategoryID] ELSE @Category_CategoryID END

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Create] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Create] 
    @UserID int,
    @Budget_BudgetID int,
    @Category_CategoryID int,
    @Amount decimal(18, 2)
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            INNER JOIN
                [dbo].[vCategories] [c] ON [c].[Account_AccountID] = [a].[AccountID]
            INNER JOIN
                [dbo].[vBudgets] [b] ON [b].[Account_AccountID] = [a].[AccountID]
            WHERE 
                [a].[User_UserID] = @UserID 
            AND
                [b].[BudgetID] = @Budget_BudgetID
	        AND 
	            [c].[CategoryID] = @Category_CategoryID
        )
        BEGIN
            
            DECLARE @Now datetime = GETDATE()
    
            INSERT INTO 
                [dbo].[Categories_Budgets] (
                    [Budget_BudgetID], 
                    [Category_CategoryID], 
                    [Amount], 
                    [CreatedBy], 
                    [CreatedDate], 
                    [LastModifiedBy], 
                    [LastModifiedDate]
                )
            SELECT 
                @Budget_BudgetID, 
                @Category_CategoryID, 
                @Amount, 
                @UserID, 
                @Now, 
                @UserID, 
                @Now
            
            SELECT 
                [cb].[Budget_BudgetID], 
                [cb].[Category_CategoryID], 
                [cb].[Amount], 
                [cb].[CreatedBy], 
                [cb].[CreatedDate], 
                [cb].[LastModifiedBy], 
                [cb].[LastModifiedDate], 
                [cb].[Deleted], 
                [cb].[DeletedBy], 
                [cb].[DeletedDate],
                [c].[Name] AS CategoryName,
                [c].[Type] AS CategoryType
            FROM   
                [dbo].[vCategories_Budgets] [cb]
            INNER JOIN
                [dbo].[vCategories] [c] ON [c].[CategoryID] = [cb].[Category_CategoryID]
            INNER JOIN
                [dbo].[vBudgets] [b] ON [b].[BudgetID] = [cb].[Budget_BudgetID]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [c].[Account_AccountID] AND [a].[AccountID] = [b].[Account_AccountID]
            WHERE  
                [cb].[Budget_BudgetID] = @Budget_BudgetID
	        AND 
	            [cb].[Category_CategoryID] = @Category_CategoryID
	            
        END
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Update] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Update] 
    @UserID int,
    @Budget_BudgetID int,
    @Category_CategoryID int,
    @Amount decimal(18, 2)
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            INNER JOIN
                [dbo].[vCategories] [c] ON [c].[Account_AccountID] = [a].[AccountID]
            INNER JOIN
                [dbo].[vBudgets] [b] ON [b].[Account_AccountID] = [a].[AccountID]
            WHERE 
                [a].[User_UserID] = @UserID 
            AND
                [b].[BudgetID] = @Budget_BudgetID
	        AND 
	            [c].[CategoryID] = @Category_CategoryID
        )
        BEGIN
            
            DECLARE @Now datetime = GETDATE()
            
            UPDATE 
                [dbo].[Categories_Budgets]
            SET    
                [Amount] = @Amount, 
                [LastModifiedBy] = @UserID, 
                [LastModifiedDate] = @Now
            WHERE  
                [Budget_BudgetID] = @Budget_BudgetID
	        AND 
	            [Category_CategoryID] = @Category_CategoryID
            
            SELECT 
                [cb].[Budget_BudgetID], 
                [cb].[Category_CategoryID], 
                [cb].[Amount], 
                [cb].[CreatedBy], 
                [cb].[CreatedDate], 
                [cb].[LastModifiedBy], 
                [cb].[LastModifiedDate], 
                [cb].[Deleted], 
                [cb].[DeletedBy], 
                [cb].[DeletedDate],
                [c].[Name] AS CategoryName,
                [c].[Type] AS CategoryType
            FROM   
                [dbo].[vCategories_Budgets] [cb]
            INNER JOIN
                [dbo].[vCategories] [c] ON [c].[CategoryID] = [cb].[Category_CategoryID]
            INNER JOIN
                [dbo].[vBudgets] [b] ON [b].[BudgetID] = [cb].[Budget_BudgetID]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [c].[Account_AccountID] AND [a].[AccountID] = [b].[Account_AccountID]
            WHERE  
                [cb].[Budget_BudgetID] = @Budget_BudgetID
	        AND 
	            [cb].[Category_CategoryID] = @Category_CategoryID
	        
        END

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Delete] 
    @UserID int,
    @Budget_BudgetID int,
    @Category_CategoryID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            INNER JOIN
                [dbo].[vCategories] [c] ON [c].[Account_AccountID] = [a].[AccountID]
            INNER JOIN
                [dbo].[vBudgets] [b] ON [b].[Account_AccountID] = [a].[AccountID]
            WHERE 
                [a].[User_UserID] = @UserID 
            AND
                [b].[BudgetID] = @Budget_BudgetID
	        AND 
	            [c].[CategoryID] = @Category_CategoryID
        )
        BEGIN
        
            UPDATE   
                [cb]
            SET  
                [cb].[Deleted] = 1,
                [cb].[DeletedBy] = @UserID,
                [cb].[DeletedDate] = GETDATE()
            FROM
                [dbo].[vCategories_Budgets] AS [cb]
            INNER JOIN
                [dbo].[vCategories] [c] ON [c].[CategoryID] = [cb].[Category_CategoryID]
            INNER JOIN
                [dbo].[vBudgets] [b] ON [b].[BudgetID] = [cb].[Budget_BudgetID]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [c].[Account_AccountID] AND [a].[AccountID] = [b].[Account_AccountID]
            WHERE  
                [a].[User_UserID] = @UserID
            AND
                [cb].[Budget_BudgetID] = @Budget_BudgetID
	        AND 
	            [cb].[Category_CategoryID] = @Category_CategoryID
	        
	        SELECT 
                [cb].[Budget_BudgetID], 
                [cb].[Category_CategoryID], 
                [cb].[Amount], 
                [cb].[CreatedBy], 
                [cb].[CreatedDate], 
                [cb].[LastModifiedBy], 
                [cb].[LastModifiedDate], 
                [cb].[Deleted], 
                [cb].[DeletedBy], 
                [cb].[DeletedDate],
                [c].[Name] AS CategoryName,
                [c].[Type] AS CategoryType
            FROM   
                [dbo].[Categories_Budgets] [cb]
            INNER JOIN
                [dbo].[Categories] [c] ON [c].[CategoryID] = [cb].[Category_CategoryID]
            INNER JOIN
                [dbo].[Budgets] [b] ON [b].[BudgetID] = [cb].[Budget_BudgetID]
            INNER JOIN
                [dbo].[Accounts] [a] ON [a].[AccountID] = [c].[Account_AccountID] AND [a].[AccountID] = [b].[Account_AccountID]
            WHERE  
                [cb].[Budget_BudgetID] = @Budget_BudgetID
	        AND 
	            [cb].[Category_CategoryID] = @Category_CategoryID
	        
        END

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
    @UserID int,
    @SessionID int = NULL,
    @Key nvarchar(MAX) = NULL
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
            [dbo].[vSessions] 
        WHERE  
            [User_UserID] = @UserID
        AND
            [SessionID] = CASE WHEN @SessionID IS NULL THEN [SessionID] ELSE @SessionID END
        AND
            [Key] = CASE WHEN @Key IS NULL THEN [Key] ELSE @Key END
    
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Sessions_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Sessions_Create] 
END 
GO

CREATE PROC [dbo].[mm_Sessions_Create] 
    @UserID int,
    @Key nvarchar(MAX),
    @Expiry datetime
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        DECLARE @Now datetime = GETDATE()
        
        INSERT INTO 
            [dbo].[Sessions] (
                [Key], 
                [Expiry], 
                [User_UserID], 
                [CreatedBy], 
                [CreatedDate], 
                [LastModifiedBy], 
                [LastModifiedDate]
            )
        SELECT 
            @Key, 
            @Expiry, 
            @UserID, 
            @UserID, 
            @Now, 
            @UserID, 
            @Now
        
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
            [dbo].[vSessions]
        WHERE  
            [User_UserID] = @UserID
        AND
            [SessionID] = SCOPE_IDENTITY()
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Sessions_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Sessions_Update] 
END 
GO

CREATE PROC [dbo].[mm_Sessions_Update] 
    @UserID int,
    @SessionID int,
    @Key nvarchar(MAX),
    @Expiry datetime
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DECLARE @Now datetime = GETDATE()

        UPDATE 
            [dbo].[Sessions]
        SET    
            [Key] = @Key, 
            [Expiry] = @Expiry, 
            [LastModifiedBy] = @UserID, 
            [LastModifiedDate] = @Now
        WHERE  
            [User_UserID] = @UserID
        AND
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
            [dbo].[vSessions]
        WHERE  
            [User_UserID] = @UserID
        AND
            [SessionID] = @SessionID

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Sessions_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Sessions_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Sessions_Delete]
    @UserID int,
    @SessionID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        UPDATE   
            [dbo].[Sessions]
        SET  
            [Deleted] = 1,
            [DeletedBy] = @UserID,
            [DeletedDate] = GETDATE()
        WHERE
            [User_UserID] = @UserID
        AND
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
            [User_UserID] = @UserID
        AND
            [SessionID] = @SessionID

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Sessions_Delete_Expired]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Sessions_Delete_Expired] 
END 
GO

CREATE PROC [dbo].[mm_Sessions_Delete_Expired]
    @UserID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DECLARE @Now datetime = GETDATE()

        UPDATE   
            [dbo].[Sessions]
        SET  
            [Deleted] = 1,
            [DeletedBy] = @UserID,
            [DeletedDate] = @Now
        WHERE
            [User_UserID] = @UserID
        AND
            [Expiry] <= @Now

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
    @UserID int,
    @TransactionID int = NULL,
    @AccountID int = NULL,
    @CategoryID int = NULL,
    @From datetime = NULL,
    @To datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        DECLARE @Accounts TABLE (AccountID int)
        
        -- Get a table variable containing all the user's account IDs, unless an account ID 
        -- has been passed in, in which case the table var will only contain that ID
        INSERT INTO 
            @Accounts
        SELECT 
            [a].[AccountID] 
        FROM 
            [dbo].[vAccounts] [a] 
        WHERE 
            [a].[User_UserID] = @UserID
        AND 
            [a].[AccountID] = CASE WHEN @AccountID IS NULL THEN [a].[AccountID] ELSE @AccountID END
            
        SELECT          
            [t].[TransactionID], 
            [t].[Date], 
            [t].[Description], 
            [t].[Amount], 
            [t].[Category_CategoryID], 
            [t].[Account_AccountID], 
            [t].[CreatedBy], 
            [t].[CreatedDate], 
            [t].[LastModifiedBy], 
            [t].[LastModifiedDate], 
            [t].[Deleted], 
            [t].[DeletedBy], 
            [t].[DeletedDate], 
            [t].[TransferGUID], 
            [t].[Note],
            [c].[Name] AS [CategoryName],
            [a].[Name] AS [AccountName]
        INTO
            #UserTransactions
        FROM   
            [dbo].[vTransactions] [t]
        INNER JOIN
            [dbo].[vAccounts] [a] ON [a].[AccountID] = [t].[Account_AccountID]
        LEFT JOIN
            [dbo].[vCategories] [c] ON [c].[CategoryID] = [t].[Category_CategoryID]
        WHERE  
            [a].[User_UserID] = @UserID
        AND
            [a].[AccountID] IN (SELECT AccountID FROM @Accounts)
        AND
            [t].[Date] >= CASE WHEN @From IS NULL THEN [t].[Date] ELSE @From END
        AND
            [t].[Date] <= CASE WHEN @To IS NULL THEN [t].[Date] ELSE @To END
        AND
            [t].[TransactionID] = CASE WHEN @TransactionID IS NULL THEN [t].[TransactionID] ELSE @TransactionID END
		ORDER BY
			[t].[Date] DESC
        
        IF @CategoryID IS NOT NULL
        BEGIN
            SELECT 
                *
            FROM 
                #UserTransactions [t]
            WHERE
                [t].[Category_CategoryID] = @CategoryID
        END
        ELSE
        BEGIN
            SELECT 
                * 
            FROM 
                #UserTransactions [t]
        END
            
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Transactions_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Transactions_Create] 
END 
GO

CREATE PROC [dbo].[mm_Transactions_Create] 
    @UserID int,
    @Date datetime,
    @Description nvarchar(MAX) = NULL,
    @Amount decimal(18, 2),
    @Category_CategoryID int = NULL,
    @Account_AccountID int,
    @TransferGUID nvarchar(MAX) = NULL,
    @Note nvarchar(MAX) = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            WHERE 
                [a].[User_UserID] = @UserID 
            AND 
                [a].[AccountID] = @Account_AccountID
        )
        BEGIN
    
            DECLARE @Now datetime = GETDATE()
        
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
                    [TransferGUID], 
                    [Note]
                )
            SELECT 
                @Date, 
                @Description, 
                @Amount, 
                @Category_CategoryID, 
                @Account_AccountID, 
                @UserID, 
                @Now, 
                @UserID, 
                @Now, 
                @TransferGUID, 
                @Note
            
            SELECT 
                [t].[TransactionID], 
                [t].[Date], 
                [t].[Description], 
                [t].[Amount], 
                [t].[Category_CategoryID], 
                [t].[Account_AccountID], 
                [t].[CreatedBy], 
                [t].[CreatedDate], 
                [t].[LastModifiedBy], 
                [t].[LastModifiedDate], 
                [t].[Deleted], 
                [t].[DeletedBy], 
                [t].[DeletedDate], 
                [t].[TransferGUID], 
                [t].[Note],
                [c].[Name] AS [CategoryName],
                [a].[Name] AS [AccountName]
            FROM   
                [dbo].[vTransactions] [t]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [t].[Account_AccountID]
            LEFT JOIN
                [dbo].[vCategories] [c] ON [c].[CategoryID] = [t].[Category_CategoryID]
            WHERE  
                [t].[TransactionID] = SCOPE_IDENTITY()
            
        END
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Transactions_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Transactions_Update] 
END 
GO

CREATE PROC [dbo].[mm_Transactions_Update] 
    @UserID int,
    @TransactionID int,
    @Date datetime,
    @Description nvarchar(MAX) = NULL,
    @Amount decimal(18, 2),
    @Category_CategoryID int = NULL,
    @Account_AccountID int,
    @TransferGUID nvarchar(MAX) = NULL,
    @Note nvarchar(MAX) = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        IF EXISTS(
            SELECT 
                [AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            WHERE 
                [a].[User_UserID] = @UserID 
            AND 
                [a].[AccountID] = @Account_AccountID
        )
        BEGIN

            DECLARE @Now datetime = GETDATE()
            
            UPDATE 
                [dbo].[Transactions]
            SET    
                [Date] = @Date, 
                [Description] = @Description, 
                [Amount] = @Amount, 
                [Category_CategoryID] = @Category_CategoryID, 
                [Account_AccountID] = @Account_AccountID, 
                [LastModifiedBy] = @UserID, 
                [LastModifiedDate] = @Now, 
                [TransferGUID] = @TransferGUID, 
                [Note] = @Note
            WHERE  
                [TransactionID] = @TransactionID
                
            SELECT 
                [t].[TransactionID], 
                [t].[Date], 
                [t].[Description], 
                [t].[Amount], 
                [t].[Category_CategoryID], 
                [t].[Account_AccountID], 
                [t].[CreatedBy], 
                [t].[CreatedDate], 
                [t].[LastModifiedBy], 
                [t].[LastModifiedDate], 
                [t].[Deleted], 
                [t].[DeletedBy], 
                [t].[DeletedDate], 
                [t].[TransferGUID], 
                [t].[Note],
                [c].[Name] AS [CategoryName],
                [a].[Name] AS [AccountName]
            FROM   
                [dbo].[vTransactions] [t]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [t].[Account_AccountID]
            LEFT JOIN
                [dbo].[vCategories] [c] ON [c].[CategoryID] = [t].[Category_CategoryID]
            WHERE  
                [t].[TransactionID] = @TransactionID

        END
        
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Transactions_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Transactions_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Transactions_Delete] 
    @UserID int,
    @TransactionID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        IF EXISTS(
            SELECT 
                [a].[AccountID] 
            FROM 
                [dbo].[vAccounts] [a] 
            INNER JOIN
                [dbo].[vTransactions] [t] ON [t].[Account_AccountID] = [a].[AccountID] AND [t].[TransactionID] = @TransactionID
            WHERE 
                [a].[User_UserID] = @UserID 
        )
        BEGIN

            UPDATE   
                [t]
            SET  
                [t].[Deleted] = 1,
                [t].[DeletedBy] = @UserID,
                [t].[DeletedDate] = GETDATE()
            FROM
                [dbo].[vTransactions] AS [t]
            INNER JOIN
                [dbo].[vAccounts] [a] ON [a].[AccountID] = [t].[Account_AccountID]
            WHERE
                [a].[User_UserID] = @UserID
            AND
                [t].[TransactionID] = @TransactionID
                
            SELECT 
                [t].[TransactionID], 
                [t].[Date], 
                [t].[Description], 
                [t].[Amount], 
                [t].[Category_CategoryID], 
                [t].[Account_AccountID], 
                [t].[CreatedBy], 
                [t].[CreatedDate], 
                [t].[LastModifiedBy], 
                [t].[LastModifiedDate], 
                [t].[Deleted], 
                [t].[DeletedBy], 
                [t].[DeletedDate], 
                [t].[TransferGUID], 
                [t].[Note] 
            FROM   
                [dbo].[Transactions] [t]
            INNER JOIN
                [dbo].[Accounts] [a] ON [a].[AccountID] = [t].[Account_AccountID]
            WHERE  
                [a].[User_UserID] = @UserID
            AND
                [t].[TransactionID] = CASE WHEN @TransactionID IS NULL THEN [t].[TransactionID] ELSE @TransactionID END

        END

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