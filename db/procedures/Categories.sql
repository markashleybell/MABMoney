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
            [c].[GUID],
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
    @GUID uniqueidentifier,
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
            
            -- If a category with the same name and type already exists 
            -- for this account, but is deleted, just 'undelete' it
            IF EXISTS(
                SELECT 
                    [CategoryID] 
                FROM 
                    [dbo].[Categories] [c] 
                WHERE
                    [c].[Account_AccountID] = @Account_AccountID
                AND 
                    [c].[Type] = @Type
                AND
                    [c].[Name] = @Name
            )
            BEGIN
                    
                DECLARE @CategoryID int = (
                    SELECT TOP 1
                        [CategoryID] 
                    FROM 
                        [dbo].[Categories] [c] 
                    WHERE
                        [c].[Account_AccountID] = @Account_AccountID
                    AND 
                        [c].[Type] = @Type
                    AND
                        [c].[Name] = @Name
                )
                    
                UPDATE 
                    [dbo].[Categories]
                SET    
                    [Name] = @Name, 
                    [Account_AccountID] = @Account_AccountID, 
                    [Type] = @Type, 
                    [LastModifiedBy] = @UserID, 
                    [LastModifiedDate] = @Now,
                    [Deleted] = 0,
                    [DeletedBy] = NULL,
                    [DeletedDate] = NULL
                WHERE  
                    [CategoryID] = @CategoryID
                
                SELECT 
                    [c].[CategoryID], 
                    [c].[GUID],
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
            ELSE
            BEGIN
            
                INSERT INTO 
                    [dbo].[Categories] (
                        [GUID],
                        [Name], 
                        [Account_AccountID], 
                        [Type], 
                        [CreatedBy], 
                        [CreatedDate], 
                        [LastModifiedBy], 
                        [LastModifiedDate]
                    )
                SELECT 
                    @GUID,
                    @Name, 
                    @Account_AccountID, 
                    @Type, 
                    @UserID, 
                    @Now, 
                    @UserID, 
                    @Now
                
                SELECT 
                    [c].[CategoryID],
                    [c].[GUID], 
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
                [c].[GUID], 
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
                [c].[GUID], 
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

