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
    @Password nvarchar(MAX)
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
                [LastModifiedDate]
            )
        SELECT 
            @Forename, 
            @Surname, 
            @Email, 
            @Password, 
            -1, 
            @Now, 
            -1, 
            @Now
        
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
    @PasswordResetExpiry datetime = NULL
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

