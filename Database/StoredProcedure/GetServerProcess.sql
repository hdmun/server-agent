
CREATE PROC [dbo].[GetServerProcess]
	@hostName varchar(30)
AS
SET NOCOUNT ON
SET LOCK_TIMEOUT 2000

	SELECT [ServerName], [ProcessPath]
	FROM [dbo].[ServerProcess]
	WHERE [HostName] = @hostName
