USE [ServerAgent]
GO


IF OBJECT_ID('[dbo].[Config]') IS NOT NULL
	DROP TABLE [dbo].[Config]
GO

CREATE TABLE [dbo].[Config] (
	[HostName] varchar(255) NOT NULL,
	[Key] varchar(255) NOT NULL,
	[Value] varchar(255) NOT NULL,

	PRIMARY KEY([HostName], [Key])
)
GO


IF OBJECT_ID('[dbo].[ServerBinary]') IS NOT NULL
	DROP TABLE [dbo].[ServerBinary]
GO

CREATE TABLE [dbo].[ServerBinary] (
	[HostName] varchar(255) NOT NULL,
	[ProcessPath] varchar(255) NOT NULL,
	[ServerName] varchar(255) NOT NULL,

	PRIMARY KEY([HostName], [ProcessPath], [ServerName])
)
GO
