CREATE TABLE [dbo].[ServerAgentLog]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Date] datetime NOT NULL,
    [Thread] varchar(255) NOT NULL,
    [Level] varchar(50) NOT NULL,
    [Logger] varchar(255) NOT NULL,
    [Message] nvarchar(4000) NOT NULL,
    [Exception] nvarchar(2000) NULL
)
