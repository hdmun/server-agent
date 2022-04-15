CREATE TABLE [dbo].[ServerProcess] (
	[HostName] varchar(30) NOT NULL,
	[ServerName] varchar(255) NOT NULL,
	[ProcessPath] varchar(255) NOT NULL,

	PRIMARY KEY([HostName], [ServerName])
)