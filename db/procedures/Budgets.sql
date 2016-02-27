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
            [b].[GUID],
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
                [b].[GUID],
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
    @GUID uniqueidentifier,
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
                    [GUID],
                    [Start], 
                    [End], 
                    [Account_AccountID], 
                    [CreatedBy], 
                    [CreatedDate], 
                    [LastModifiedBy], 
                    [LastModifiedDate]
                )
            SELECT 
                @GUID,
                @Start, 
                @End, 
                @Account_AccountID, 
                @UserID, 
                @Now,
                @UserID,
                @Now
            
            SELECT 
                [BudgetID], 
                [GUID],
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
                [GUID],
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
                [b].[GUID],
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

