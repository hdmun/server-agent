/*
배포 후 스크립트 템플릿							
--------------------------------------------------------------------------------------
 이 파일에는 빌드 스크립트에 추가될 SQL 문이 있습니다.		
 SQLCMD 구문을 사용하여 파일을 배포 후 스크립트에 포함합니다.			
 예:      :r .\myfile.sql								
 SQLCMD 구문을 사용하여 배포 후 스크립트의 변수를 참조합니다.		
 예:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

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
