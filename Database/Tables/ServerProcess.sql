CREATE TABLE [dbo].[ServerProcess] (
	[HostName] varchar(30) NOT NULL,
	[ServerName] varchar(255) NOT NULL,
	[BinaryPath] varchar(255) NOT NULL,

	PRIMARY KEY([HostName], [ServerName])
)