USE [ServerAgent]
GO


IF OBJECT_ID('[dbo].[GetDeadlockTime]') IS NOT NULL
	DROP PROC [dbo].[GetDeadlockTime]
GO

CREATE PROC [dbo].[GetDeadlockTime]
	@hostName varchar(30)
AS
SET NOCOUNT ON
SET LOCK_TIMEOUT 2000

SELECT [Value] FROM [dbo].[Config] WHERE [HostName] = @hostName AND [Key] = 'DeadlockTime'
GO


IF OBJECT_ID('[dbo].[GetStoppedTime]') IS NOT NULL
	DROP PROC [dbo].[GetStoppedTime]
GO

CREATE PROC [dbo].[GetStoppedTime]
	@hostName varchar(30)
AS
SET NOCOUNT ON
SET LOCK_TIMEOUT 2000

SELECT [Value] FROM [dbo].[Config] WHERE [HostName] = @hostName AND [Key] = 'StoppedTime'
GO


IF OBJECT_ID('[dbo].[GetServerProcess]') IS NOT NULL
	DROP PROC [dbo].[GetServerProcess]
GO

CREATE PROC [dbo].[GetServerProcess]
	@hostName varchar(30)
AS
SET NOCOUNT ON
SET LOCK_TIMEOUT 2000

SELECT [ServerName], [ProcessPath]
FROM [dbo].[ServerBinary]
WHERE [HostName] = @hostName
GO
