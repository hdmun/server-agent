USE [ServerAgent]
GO

TRUNCATE TABLE [dbo].[Config]
GO

INSERT INTO [dbo].[Config] ([HostName], [Key], [Value])
    VALUES ('LAPTOP-PR4G61PI', 'DeadlockTime', '3'),
           ('LAPTOP-PR4G61PI', 'StoppedTime', '30')

TRUNCATE TABLE [dbo].[ServerBinary]
GO

INSERT INTO [dbo].[ServerBinary] ([HostName], [ProcessPath], [ServerName])
    VALUES ('LAPTOP-PR4G61PI', '.\test-server-dot-net.exe', 'TestServer1'),
           ('LAPTOP-PR4G61PI', '.\test-server-dot-net.exe', 'TestServer2')

GO
