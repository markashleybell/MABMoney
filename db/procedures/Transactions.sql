IF OBJECT_ID('[dbo].[mm_Transactions_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Transactions_Read] 
END 
GO

CREATE PROC [dbo].[mm_Transactions_Read] 
    @UserID int,
    @TransactionID int = NULL,
    @AccountID int = NULL,
    @From datetime = NULL,
    @To datetime = NULL
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

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
            [a].[AccountID] = CASE WHEN @AccountID IS NULL THEN [a].[AccountID] ELSE @AccountID END
        AND
            [t].[Date] >= CASE WHEN @From IS NULL THEN [t].[Date] ELSE @From END
        AND
            [t].[Date] <= CASE WHEN @To IS NULL THEN [t].[Date] ELSE @To END
        AND
            [t].[TransactionID] = CASE WHEN @TransactionID IS NULL THEN [t].[TransactionID] ELSE @TransactionID END
            
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
                [dbo].[Accounts] [a] 
            INNER JOIN
                [dbo].[Transactions] [t] ON [t].[Account_AccountID] = [a].[AccountID] AND [t].[TransactionID] = @TransactionID
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
                [dbo].[Transactions] AS [t]
            INNER JOIN
                [dbo].[Accounts] [a] ON [a].[AccountID] = [t].[Account_AccountID]
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

