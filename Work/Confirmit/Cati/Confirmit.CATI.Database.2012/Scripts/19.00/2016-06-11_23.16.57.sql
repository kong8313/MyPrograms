PRINT N'Altering [dbo].[BvCallHistory]...';


GO
ALTER TABLE [dbo].[BvCallHistory]
    ADD [DialTypeId] TINYINT NULL;


GO
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsInsert]...';


GO
ALTER TRIGGER [BvTrBvSvySchedule_CallsInsert] ON [dbo].[BvSvySchedule]
AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON
	
	INSERT INTO BvSvyScheduleRuntimeStatisticsDelta(SurveyId, ShiftTypeID, ExplicitSID, CallState, CountDelta )
        SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, COUNT(*) as CountDelta
                FROM inserted
                WHERE CallState IN ( -2, 2 )
                GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState

	IF (CONTEXT_INFO() IS NOT NULL)
	BEGIN

		DECLARE @OperationType TINYINT
		DECLARE @CallCenterId INT
		DECLARE @OperationId INT
		DECLARE @ITS TINYINT
		DECLARE @DialingMode TINYINT

		SELECT @ITS = ITS, @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId, @DialingMode = DialingMode from dbo.GetContextData()

		INSERT INTO BvCallHistory
		SELECT 
			GETUTCDATE(), ApptId, ShiftTypeId, InterviewId, SurveySid, @ITS, @DialingMode, CAST(CallState AS SMALLINT), [Priority], TimeInShift, Expiretime, 
			ExplicitSid, CAST(ExplicitType AS tinyint), CellId, @OperationId, @OperationType, @CallCenterId, DialTypeId		
	    FROM inserted
	END
END
GO
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsUpdate]...';


GO
ALTER TRIGGER [dbo].[BvTrBvSvySchedule_CallsUpdate] ON [dbo].[BvSvySchedule]
FOR UPDATE
AS
BEGIN
    SET NOCOUNT ON
     
    IF UPDATE( SurveySid ) OR UPDATE( ShiftTypeId ) OR UPDATE( ExplicitSID ) OR UPDATE( CallState )
    BEGIN
        ;WITH stat AS
        (
            SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, COUNT(*) as CountDelta
                    FROM inserted
                    WHERE CallState IN ( -2, 2 )
                    GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState
            UNION ALL
            SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, -COUNT(*)
                    FROM deleted
                    WHERE CallState IN ( -2, 2 )
                    GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState
        )
        INSERT INTO BvSvyScheduleRuntimeStatisticsDelta(SurveyId, ShiftTypeID, ExplicitSID, CallState, CountDelta )
            SELECT SurveySid, ShiftTypeId, ExplicitSID, CallState, SUM(CountDelta) as Delta
                FROM stat
                GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState
                HAVING SUM(CountDelta) <> 0
                 
    END

	IF (CONTEXT_INFO() IS NOT NULL)
	BEGIN

		DECLARE @OperationType TINYINT
		DECLARE @CallCenterId INT
		DECLARE @OperationId INT
		DECLARE @ITS TINYINT
		DECLARE @DialingMode TINYINT

		SELECT @ITS = ITS, @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId, @DialingMode = DialingMode from dbo.GetContextData()

		INSERT INTO BvCallHistory
		SELECT 
			GETUTCDATE(), ApptId, ShiftTypeId, InterviewId, SurveySid, @ITS, @DialingMode, CAST(CallState AS SMALLINT), [Priority], TimeInShift, Expiretime, 
			ExplicitSid, CAST(ExplicitType AS tinyint), CellId, @OperationId, @OperationType, @CallCenterId, DialTypeId		
		FROM inserted
	END

END
GO
PRINT N'Refreshing [dbo].[GetCountsForSample]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCountsForSample]';


GO
PRINT N'Altering [dbo].[BvSpGetExtendedCallHistory]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetExtendedCallHistory]
@InterviewID     INTEGER,
@SurveyID        INTEGER,
@CallCenterID	 INTEGER
AS
SET NOCOUNT OFF


	SELECT 
		h.[Id],
		[FiredTime],
		[ApptID],
		ITS,
		ISNULL(BvState.[Name],'')  AS TransientState,
		h.ShiftTypeId,
		h.DialingMode,
		ISNULL(BvShiftType.[Name], '' ) AS ShiftType, 
		[CallState] ,
		h.[Priority],
		[TimeInShift],
		CASE h.[ExpireTime] WHEN '9999-01-01 00:00:00.000' THEN NULL ELSE h.[ExpireTime] END AS [ExpireTime],
		[ExplicitSID],
		[ExplicitType],
		ISNULL(pg.[Name], '') AS Resource,
		[CellId],
		[OperationId],
		[OperationType],
        ISNULL(cc.Name, '') AS CallCenterName,
		ISNULL(dt.Name, '') AS DialType

	FROM BvCallHistory h
	INNER JOIN BvSurvey s ON s.SID = h.SurveyId 
	LEFT JOIN BvCallCenter cc ON cc.ID = h.CallCenterID
	LEFT JOIN BvShiftZones ON BvShiftZones.[ID] = h.ShiftTypeID  
	LEFT JOIN BvShiftType ON  BvShiftType.ObjectID = BvShiftZones.ShiftTypeID  
	LEFT JOIN BvState ON BvState.StateID = h.ITS AND BvState.StateGroupID = s.StateGroupID
	LEFT JOIN BvViewPersonAndGroup pg ON pg.SID = h.ExplicitSID
	LEFT JOIN BvDialType dt ON h.DialTypeId = dt.Id

	WHERE 
		SurveyId = @SurveyID AND InterviewID = @InterviewID
	ORDER BY h.[Id]

RETURN 0
GO
PRINT N'Refreshing [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_MoveToITS]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Clean]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Clean]';


GO
PRINT N'Refreshing [dbo].[BvSpSampleUtilisationReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleUtilisationReport]';


GO
PRINT N'Update complete.';


GO
