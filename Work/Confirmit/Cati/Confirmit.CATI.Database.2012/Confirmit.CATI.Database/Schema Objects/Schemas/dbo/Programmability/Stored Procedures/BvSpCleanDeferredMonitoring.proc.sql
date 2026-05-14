CREATE PROCEDURE [BvSpCleanDeferredMonitoring]
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