CREATE TABLE [dbo].[Config] (
	[HostName] VARCHAR(30) NOT NULL,
	[Key] VARCHAR(255) NOT NULL,
	[Value] VARCHAR(255) NOT NULL,

	PRIMARY KEY([HostName], [Key])
)