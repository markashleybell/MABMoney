USE [MABMoney]
GO

DROP TRIGGER [dbo].[Transactions_UpdateAccountBalance]
GO

CREATE TRIGGER [dbo].[Transactions_UpdateAccountBalance]
ON [dbo].[Transactions]
FOR INSERT, UPDATE, DELETE
AS
BEGIN
	
	DECLARE @DeletedFromAccountID int, @InsertedIntoAccountID int, @AccountID int

	SELECT @DeletedFromAccountID = [Account_AccountID] FROM DELETED
	SELECT @InsertedIntoAccountID = [Account_AccountID] FROM INSERTED
	
	IF @DeletedFromAccountID IS NOT NULL AND @InsertedIntoAccountID IS NULL
	BEGIN
		-- PRINT 'DELETED transaction from account ' + CAST(@DeletedFromAccountID AS nvarchar(10))
		SET @AccountID = @DeletedFromAccountID
	END
	
	IF @DeletedFromAccountID IS NOT NULL AND @InsertedIntoAccountID IS NOT NULL
	BEGIN
		-- PRINT 'UPDATED transaction in account ' + CAST(@DeletedFromAccountID AS nvarchar(10))
		SET @AccountID = @InsertedIntoAccountID
	END
	
	IF @DeletedFromAccountID IS NULL AND @InsertedIntoAccountID IS NOT NULL
	BEGIN
		-- PRINT 'INSERTED transaction into account ' + CAST(@InsertedIntoAccountID AS nvarchar(10))
		SET @AccountID = @InsertedIntoAccountID
	END
		
	UPDATE Accounts SET CurrentBalance = StartingBalance + ISNULL((SELECT SUM(Amount) FROM Transactions WHERE Account_AccountID = @AccountID), 0) WHERE AccountID = @AccountID
	
END
GO

DROP TRIGGER [dbo].[Accounts_UpdateAccountBalance]
GO

CREATE TRIGGER [dbo].[Accounts_UpdateAccountBalance]
ON [dbo].[Accounts]
FOR INSERT, UPDATE, DELETE
AS
BEGIN
	
	DECLARE @DeletedFromAccountID int, @InsertedIntoAccountID int, @AccountID int

	SELECT @DeletedFromAccountID = [AccountID] FROM DELETED
	SELECT @InsertedIntoAccountID = [AccountID] FROM INSERTED
	
	IF @DeletedFromAccountID IS NOT NULL AND @InsertedIntoAccountID IS NULL
	BEGIN
		-- PRINT 'DELETED transaction from account ' + CAST(@DeletedFromAccountID AS nvarchar(10))
		SET @AccountID = @DeletedFromAccountID
	END
	
	IF @DeletedFromAccountID IS NOT NULL AND @InsertedIntoAccountID IS NOT NULL
	BEGIN
		-- PRINT 'UPDATED transaction in account ' + CAST(@DeletedFromAccountID AS nvarchar(10))
		SET @AccountID = @InsertedIntoAccountID
	END
	
	IF @DeletedFromAccountID IS NULL AND @InsertedIntoAccountID IS NOT NULL
	BEGIN
		-- PRINT 'INSERTED transaction into account ' + CAST(@InsertedIntoAccountID AS nvarchar(10))
		SET @AccountID = @InsertedIntoAccountID
	END
		
	UPDATE Accounts SET CurrentBalance = StartingBalance + ISNULL((SELECT SUM(Amount) FROM Transactions WHERE Account_AccountID = @AccountID), 0) WHERE AccountID = @AccountID
	
END
GO
