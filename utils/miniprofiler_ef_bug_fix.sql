ALTER TABLE dbo.__MigrationHistory ADD CreatedOn DateTime Default GETDATE()
GO
UPDATE dbo.__MigrationHistory SET CreatedOn = GETDATE()
GO