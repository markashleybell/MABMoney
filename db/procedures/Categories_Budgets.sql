IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Read]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Read] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Read] 
    @Budget_BudgetID int,
    @Category_CategoryID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  

    BEGIN TRAN

        SELECT 
            [Budget_BudgetID], 
            [Category_CategoryID], 
            [Amount], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate] 
        FROM   
            [dbo].[Categories_Budgets] 
        WHERE  
            ([Budget_BudgetID] = @Budget_BudgetID OR @Budget_BudgetID IS NULL) 
	    AND 
	        ([Category_CategoryID] = @Category_CategoryID OR @Category_CategoryID IS NULL) 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Create]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Create] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Create] 
    @Budget_BudgetID int,
    @Category_CategoryID int,
    @Amount decimal(18, 2),
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
            [dbo].[Categories_Budgets] (
                [Budget_BudgetID], 
                [Category_CategoryID], 
                [Amount], 
                [CreatedBy], 
                [CreatedDate], 
                [LastModifiedBy], 
                [LastModifiedDate], 
                [Deleted], 
                [DeletedBy], 
                [DeletedDate]
            )
        SELECT 
            @Budget_BudgetID, 
            @Category_CategoryID, 
            @Amount, 
            @CreatedBy, 
            @CreatedDate, 
            @LastModifiedBy, 
            @LastModifiedDate, 
            @Deleted, 
            @DeletedBy, 
            @DeletedDate
        
        SELECT 
            [Budget_BudgetID], 
            [Category_CategoryID], 
            [Amount], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Categories_Budgets]
        WHERE  
            [Budget_BudgetID] = @Budget_BudgetID
	    AND 
	        [Category_CategoryID] = @Category_CategoryID
           
    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Update]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Update] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Update] 
    @Budget_BudgetID int,
    @Category_CategoryID int,
    @Amount decimal(18, 2),
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
            [dbo].[Categories_Budgets]
        SET    
            [Budget_BudgetID] = @Budget_BudgetID, 
            [Category_CategoryID] = @Category_CategoryID, 
            [Amount] = @Amount, 
            [CreatedBy] = @CreatedBy, 
            [CreatedDate] = @CreatedDate, 
            [LastModifiedBy] = @LastModifiedBy, 
            [LastModifiedDate] = @LastModifiedDate, 
            [Deleted] = @Deleted, 
            [DeletedBy] = @DeletedBy, 
            [DeletedDate] = @DeletedDate
        WHERE  
            [Budget_BudgetID] = @Budget_BudgetID
	    AND 
	        [Category_CategoryID] = @Category_CategoryID
        
        SELECT 
            [Budget_BudgetID], 
            [Category_CategoryID], 
            [Amount], 
            [CreatedBy], 
            [CreatedDate], 
            [LastModifiedBy], 
            [LastModifiedDate], 
            [Deleted], 
            [DeletedBy], 
            [DeletedDate]
        FROM   
            [dbo].[Categories_Budgets]
        WHERE  
            [Budget_BudgetID] = @Budget_BudgetID
	    AND 
	        [Category_CategoryID] = @Category_CategoryID 

    COMMIT
GO

IF OBJECT_ID('[dbo].[mm_Categories_Budgets_Delete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[mm_Categories_Budgets_Delete] 
END 
GO

CREATE PROC [dbo].[mm_Categories_Budgets_Delete] 
    @Budget_BudgetID int,
    @Category_CategoryID int
AS 
    SET NOCOUNT ON 
    SET XACT_ABORT ON  
    
    BEGIN TRAN

        DELETE
        FROM   
            [dbo].[Categories_Budgets]
        WHERE  
            [Budget_BudgetID] = @Budget_BudgetID
	    AND 
	        [Category_CategoryID] = @Category_CategoryID

    COMMIT
GO

----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

