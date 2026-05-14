GO
PRINT N'Altering [dbo].[BvPersonDeferredMonitoring]...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD [IsRetained] BIT             CONSTRAINT [DF_BvPersonDeferredMonitoring_IsRetained] DEFAULT 0 NOT NULL,
        [Comment]    NVARCHAR (1024) NULL;


GO
PRINT N'Altering [dbo].[BvSpCleanDeferredMonitoring]...';


GO
ALTER PROCEDURE [BvSpCleanDeferredMonitoring]
	@ExpirationPeriodInDays INT,
	@DeleteTopRows INT
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @EpirationTime DATETIME = DateAdd(day, -@ExpirationPeriodInDays, GETUTCDATE())
	DECLARE @DeletedRowCount INT
	DELETE TOP(@DeleteTopRows) FROM [BvPersonDeferredMonitoring]
	WHERE [TimeStamp] < @EpirationTime AND [IsRetained] <> 1
	SET @DeletedRowCount = @@ROWCOUNT;
	RETURN @DeletedRowCount	
END
GO
PRINT N'Refreshing [dbo].[BvSpGetDeferredMonitoringStartFile]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetDeferredMonitoringStartFile]';


GO
PRINT N'Update complete.';


GO
