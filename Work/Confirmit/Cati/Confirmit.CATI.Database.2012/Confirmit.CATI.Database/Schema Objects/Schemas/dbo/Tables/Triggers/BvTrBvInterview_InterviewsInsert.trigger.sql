CREATE TRIGGER [BvTrBvInterview_InterviewsInsert] ON [dbo].[BvInterview] 
AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON
    
	INSERT INTO [BvAggregateSurveyDelta]
		SELECT 
		    /*[SID]*/ SurveySID, 
			/*[ScheduledCallsCount]*/ 0, 
			/*[SuspendedCallsCount]*/ COUNT(*) SuspendedCallsCount, 
			/*[MinutesSpentWorkingOnSurvey]*/ 0
        FROM inserted
        GROUP BY SurveySID

    INSERT INTO [BvSampleStatusSummaryDelta]
		SELECT 
    	    /*[SurveySID]*/ SurveySID,
	        /*[ITS]*/ TransientState,
	        /*[Cnt]*/ COUNT(ID),
			/*[IsCati]*/ CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
	    FROM INSERTED
	    GROUP BY SurveySID, TransientState, CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
END