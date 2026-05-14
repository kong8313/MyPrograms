CREATE TRIGGER [BvTrBvSvySchedule_CallsDelete] ON [dbo].[BvSvySchedule]
FOR DELETE
AS 
BEGIN
	SET NOCOUNT ON
                                      
    INSERT INTO BvSvyScheduleRuntimeStatisticsDelta(SurveyId, ShiftTypeID, ExplicitSID, CallState, CountDelta )
        SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, -COUNT(*) as CountDelta
                FROM deleted
                WHERE CallState IN ( -2, 2 )
                GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState

END
