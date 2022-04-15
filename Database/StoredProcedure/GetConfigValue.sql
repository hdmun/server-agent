
CREATE PROC [dbo].[GetConfigValue]
	@hostName varchar(30),
	@key varchar(255)
AS
SET NOCOUNT ON
SET LOCK_TIMEOUT 2000

	SELECT [Value] FROM [dbo].[Config] WHERE [HostName] = @hostName AND [Key] = @key
