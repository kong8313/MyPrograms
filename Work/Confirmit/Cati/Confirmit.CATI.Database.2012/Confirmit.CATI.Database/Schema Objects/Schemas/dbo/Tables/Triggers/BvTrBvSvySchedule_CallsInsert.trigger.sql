CREATE TRIGGER [BvTrBvSvySchedule_CallsInsert] ON [dbo].[BvSvySchedule]
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
		DECLARE @ITS SMALLINT
		DECLARE @DialingMode TINYINT

		SELECT @ITS = ITS, @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId, @DialingMode = DialingMode from dbo.GetContextData()

		INSERT INTO BvCallHistoryEx
		SELECT 
			dbo.GetUtcNow(), ApptId, ShiftTypeId, InterviewId, SurveySid, @ITS, @DialingMode, CAST(CallState AS SMALLINT), [Priority], TimeInShift, Expiretime, 
			ExplicitSid, CAST(ExplicitType AS tinyint), CellId, @OperationId, @OperationType, @CallCenterId, DialTypeId		
	    FROM inserted
	END
END
