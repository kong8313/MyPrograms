CREATE PROCEDURE BvSpGetDeferredMonitoringStartFile
	@RecordID INT
AS
BEGIN
	SELECT [StartingFile] FROM [BvPersonDeferredMonitoring] WHERE [ID] = @RecordID
END