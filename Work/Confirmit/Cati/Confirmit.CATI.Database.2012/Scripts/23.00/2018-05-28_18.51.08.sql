PRINT N'Altering [dbo].[BvSpCall_GetExpiredAndLock]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_GetExpiredAndLock]
	@LastId INT,
	@Now DATETIME
AS
	DECLARE @SurveyId INT
	DECLARE @InterviewId INT
	DECLARE @OldCallState INT

	;WITH data as (
	SELECT TOP(1) * FROM dbo.[BvSvySchedule] with(readpast, INDEX([IX_BvTime]))
	WHERE CallState > 0 AND ExpireTime < @Now AND ID > @LastId
	ORDER BY ID
	)
	UPDATE data SET @OldCallState = CallState, @SurveyId = SurveySID, @InterviewId = InterviewId, CallState = -1

	UPDATE BvAppointment
	SET STATE = 2
	WHERE SurveySID = @SurveyID AND InterviewSID = @InterviewID AND STATE = 1

	SELECT
		BvSvySchedule.[ID] callid,
		BvSvySchedule.ApptID,
		BvSvySchedule.SurveySID,
		BvSvySchedule.InterviewID iid,
		ISNULL( @OldCallState, BvSvySchedule.CallState ) as CallState,
		ISNULL( BvShiftZones.[ShiftTypeID], BvSvySchedule.[ShiftTypeID] ) ShiftID,
		BvSvySchedule.Priority,
		BvSvySchedule.TimeInShift,
		BvSvySchedule.ExpireTime TimeToExpire,
		CASE WHEN BvSvySchedule.ExplicitType = 2 THEN BvSvySchedule.ExplicitSID ELSE 0 END AS Resource,
		BvSvySchedule.ExplicitType Resource_Type,
		OldPriority,
		RuleNumber,
		ConditionValue,
		BvSvySchedule.CellId,
		BvSvySchedule.DialTypeId,
		BvSvySchedule.Type
	FROM BvSvySchedule
	LEFT JOIN BvShiftZones ON BvSvySchedule.ShiftTypeID = BvShiftZones.[ID]
	WHERE BvSvySchedule.SurveySID = @SurveyID AND BvSvySchedule.InterviewID = @InterviewID
GO


UPDATE BvTimezone SET  
			Name = '(GMT+00:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-04 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-06-03 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+00:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-04 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-06-03 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT+00:00) Casablanca', 0, 2, 'Morocco Standard Time', '2000-10-04 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-06-03 02:00:00.000', 0, -60
END

GO
PRINT N'Update complete.';


GO
