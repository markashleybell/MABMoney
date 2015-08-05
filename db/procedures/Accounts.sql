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
            [DisplayOrder],
            [IncludeInNetWorth]
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
    @DisplayOrder int,
    @IncludeInNetWorth bit
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
				[DisplayOrder],
				[IncludeInNetWorth]
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
            @DisplayOrder,
            @IncludeInNetWorth
        
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
            [DisplayOrder],
            [IncludeInNetWorth]
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
    @DisplayOrder int,
    @IncludeInNetWorth bit
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
            [DisplayOrder] = @DisplayOrder,
            [IncludeInNetWorth] = @IncludeInNetWorth
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
            [DisplayOrder],
            [IncludeInNetWorth]
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
            [DisplayOrder],
            [IncludeInNetWorth]
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

