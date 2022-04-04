USE [ServerAgent]
GO

TRUNCATE TABLE [dbo].[Config]
GO

INSERT INTO [dbo].[Config] ([HostName], [Key], [Value])
    VALUES ('LAPTOP-PR4G61PI', 'DeadlockTime', '3'),
           ('LAPTOP-PR4G61PI', 'StoppedTime', '30'),
           ('LAPTOP-PR4G61PI', 'Checker', 'datetime'),
           ('DESKTOP-UNOS40A', 'DeadlockTime', '3'),
           ('DESKTOP-UNOS40A', 'StoppedTime', '30'),
           ('DESKTOP-UNOS40A', 'Checker', 'datetime')
GO


TRUNCATE TABLE [dbo].[ServerProcess]
GO

INSERT INTO [dbo].[ServerProcess] ([HostName], [ServerName], [ProcessPath])
    VALUES ('LAPTOP-PR4G61PI', 'TestServer1', '.\TestServer.exe'),
           ('LAPTOP-PR4G61PI', 'TestServer2', '.\TestServer.exe'),
           ('DESKTOP-UNOS40A', 'TestServer1', '.\TestServer.exe'),
           ('DESKTOP-UNOS40A', 'TestServer2', '.\TestServer.exe')
GO
