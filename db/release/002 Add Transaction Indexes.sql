CREATE NONCLUSTERED INDEX [IX_Transactions.Date_Account_Deleted] ON [dbo].[Transactions] 
(
	[Date] ASC,
	[Account_AccountID] ASC,
	[Deleted] ASC
)
INCLUDE ( [TransactionID],
[Description],
[Amount],
[Category_CategoryID],
[CreatedBy],
[CreatedDate],
[LastModifiedBy],
[LastModifiedDate],
[DeletedBy],
[DeletedDate],
[TransferGUID]) 
GO