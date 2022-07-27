CREATE TABLE [dbo].[MonitoringConfig]
(
	[HostName] VARCHAR(30) NOT NULL PRIMARY KEY, 
    [Checker] VARCHAR(10) NOT NULL, 
    [DeadlockMin] INT NOT NULL, 
    [StoppedMin] INT NOT NULL
)
