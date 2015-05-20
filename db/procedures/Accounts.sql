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

