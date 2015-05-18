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

