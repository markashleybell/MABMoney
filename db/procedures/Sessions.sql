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
            [GUID],
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
    @GUID uniqueidentifier,
    @Key nvarchar(MAX),
    @Expiry datetime
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN
    
        DECLARE @Now datetime = GETDATE()
        
        INSERT INTO 
            [dbo].[Sessions] (
                [GUID],
                [Key], 
                [Expiry], 
                [User_UserID], 
                [CreatedBy], 
                [CreatedDate], 
                [LastModifiedBy], 
                [LastModifiedDate]
            )
        SELECT 
            @GUID,
            @Key, 
            @Expiry, 
            @UserID, 
            @UserID, 
            @Now, 
            @UserID, 
            @Now
        
        SELECT 
            [SessionID], 
            [GUID],
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
            [GUID],
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
            [GUID],
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

