CREATE TRIGGER [BvTrBvInterview_InterviewsDelete] ON [dbo].[BvInterview] 
AFTER DELETE
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO [BvAggregateSurveyDelta]
		SELECT 
		    /*[SID]*/ SurveySID, 
			/*[ScheduledCallsCount]*/ 0, 
			/*[SuspendedCallsCount]*/ -COUNT(*) SuspendedCallsCount, 
			/*[MinutesSpentWorkingOnSurvey]*/ 0
        FROM deleted
        GROUP BY SurveySID

    INSERT INTO [BvSampleStatusSummaryDelta]
		SELECT 
    	    /*[SurveySID]*/ SurveySID,
	        /*[ITS]*/ TransientState,
	        /*[Cnt]*/ -COUNT(ID),
			/*[IsCati]*/ CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
	    FROM DELETED
	    GROUP BY SurveySID, TransientState, CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
END