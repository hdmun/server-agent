USE [ServerAgent]
GO

TRUNCATE TABLE [dbo].[Config]
GO

INSERT INTO [dbo].[Config] ([HostName], [Key], [Value])
    VALUES ('LAPTOP-PR4G61PI', 'DeadlockTime', '3'),
           ('LAPTOP-PR4G61PI', 'StoppedTime', '30'),
           ('DESKTOP-UNOS40A', 'DeadlockTime', '3'),
           ('DESKTOP-UNOS40A', 'StoppedTime', '30')
GO


TRUNCATE TABLE [dbo].[ServerBinary]
GO

INSERT INTO [dbo].[ServerBinary] ([HostName], [ProcessPath], [ServerName])
    VALUES ('LAPTOP-PR4G61PI', '.\TestServer.exe', 'TestServer1'),
           ('LAPTOP-PR4G61PI', '.\TestServer.exe', 'TestServer2'),
           ('DESKTOP-UNOS40A', '.\TestServer.exe', 'TestServer1'),
           ('DESKTOP-UNOS40A', '.\TestServer.exe', 'TestServer2')
GO
