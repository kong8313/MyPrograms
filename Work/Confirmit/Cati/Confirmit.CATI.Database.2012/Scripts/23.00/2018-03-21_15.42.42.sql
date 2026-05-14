PRINT N'Dropping [dbo].[BvSpGetExpiredCalls]...';


GO
DROP PROCEDURE [dbo].[BvSpGetExpiredCalls];


GO
PRINT N'Dropping [dbo].[BvSpRemoveExpiredCalls]...';


GO
DROP PROCEDURE [dbo].[BvSpRemoveExpiredCalls];


GO
PRINT N'Dropping [dbo].[BvCallExpired]...';


GO
DROP TABLE [dbo].[BvCallExpired];


GO
PRINT N'Creating [dbo].[BvSpAssignment_IsLoggedIn]...';


GO
CREATE PROCEDURE [dbo].[BvSpAssignment_IsLoggedIn]
	@resourceId int,
	@surveySID int
AS
	SELECT COUNT(*) FROM [BvLoginGroup] [lg] WHERE [lg].[ObjectSID] = @resourceId AND ( [lg].[SurveySID] = 0 or [lg].[SurveySID] = @surveySID )


GO
PRINT N'Creating [dbo].[BvSpCall_GetExpiredAndLock]...';


GO
CREATE PROCEDURE [dbo].[BvSpCall_GetExpiredAndLock]
	@LastId INT,
	@Now DATETIME
AS
	DECLARE @SurveyId INT
	DECLARE @InterviewId INT
	DECLARE @OldCallState INT

	;WITH data as (
	SELECT TOP(1) * FROM dbo.[BvSvySchedule]
	WHERE CallState > 0 AND ExpireTime < @Now AND ID > @LastId
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
PRINT N'Update complete.';


GO