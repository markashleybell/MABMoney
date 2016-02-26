USE [master]

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'MABMoney')
BEGIN
	--DECLARE @kill varchar(8000) = '';
	--SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), spid) + ';'
	--FROM master..sysprocesses 
	--WHERE dbid = db_id('MABMoney')

	--EXEC(@kill)
	ALTER DATABASE [MABMoney] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [MABMoney]
END
GO

CREATE DATABASE [MABMoney]
GO

USE [MABMoney]
GO

