USE [ServerAgent]
GO


IF OBJECT_ID('[dbo].[Config]') IS NOT NULL
	DROP TABLE [dbo].[Config]
GO

CREATE TABLE [dbo].[Config] (
	[HostName] varchar(30) NOT NULL,
	[Key] varchar(255) NOT NULL,
	[Value] varchar(255) NOT NULL,

	PRIMARY KEY([HostName], [Key])
)
GO


IF OBJECT_ID('[dbo].[ServerProcess]') IS NOT NULL
	DROP TABLE [dbo].[ServerProcess]
GO

CREATE TABLE [dbo].[ServerProcess] (
	[HostName] varchar(30) NOT NULL,
	[ServerName] varchar(255) NOT NULL,
	[ProcessPath] varchar(255) NOT NULL,

	PRIMARY KEY([HostName], [ServerName])
)
GO
