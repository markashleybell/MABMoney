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
            [cb].[GUID],
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

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Read_Including_Deleted]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Read_Including_Deleted] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Read_Including_Deleted] 
    @UserID int,
    @Budget_BudgetID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [cb].[Budget_BudgetID], 
            [cb].[Category_CategoryID], 
            [cb].[GUID],
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
            [dbo].[vBudgets] [b] ON [b].[BudgetID] = [cb].[Budget_BudgetID]
        INNER JOIN
            [dbo].[vAccounts] [a] ON [a].[AccountID] = [c].[Account_AccountID] AND [a].[AccountID] = [b].[Account_AccountID]
        WHERE 
            [a].[User_UserID] = @UserID
        AND
            [cb].[Budget_BudgetID] = @Budget_BudgetID
        AND (
            [cb].[Deleted] = 0 OR [cb].[DeletedDate] > [b].[End]
        )
        AND (
            [c].[Deleted] = 0 OR [c].[DeletedDate] > [b].[End]
        )   

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Create] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Create] 
    @UserID int,
    @GUID uniqueidentifier,
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
                    [GUID],
                    [Amount], 
                    [CreatedBy], 
                    [CreatedDate], 
                    [LastModifiedBy], 
                    [LastModifiedDate]
                )
            SELECT 
                @Budget_BudgetID, 
                @Category_CategoryID, 
                @GUID,
                @Amount, 
                @UserID, 
                @Now, 
                @UserID, 
                @Now
            
            SELECT 
                [cb].[Budget_BudgetID], 
                [cb].[Category_CategoryID], 
                [cb].[GUID],
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
                [cb].[GUID],
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
                [cb].[GUID],
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

