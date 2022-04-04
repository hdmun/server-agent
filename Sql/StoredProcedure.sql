USE [ServerAgent]
GO


IF OBJECT_ID('[dbo].[GetConfigValue]') IS NOT NULL
	DROP PROC [dbo].[GetConfigValue]
GO

CREATE PROC [dbo].[GetConfigValue]
	@hostName varchar(30),
	@key varchar(255)
AS
SET NOCOUNT ON
SET LOCK_TIMEOUT 2000

SELECT [Value] FROM [dbo].[Config] WHERE [HostName] = @hostName AND [Key] = @key
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
