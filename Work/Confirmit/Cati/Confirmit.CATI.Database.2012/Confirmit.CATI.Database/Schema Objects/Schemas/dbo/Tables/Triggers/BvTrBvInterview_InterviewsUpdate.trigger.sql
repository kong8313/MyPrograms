CREATE TRIGGER [BvTrBvInterview_InterviewsUpdate] ON [dbo].[BvInterview] 
AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON

    IF UPDATE( TransientState )
    BEGIN
		INSERT INTO [BvSampleStatusSummaryDelta]
			SELECT 
    			/*[SurveySID]*/ SurveySID,
				/*[ITS]*/ TransientState,
				/*[Cnt]*/ -COUNT(ID),
				/*[IsCati]*/ CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
			FROM DELETED
			GROUP BY SurveySID, TransientState,CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END

		INSERT INTO [BvSampleStatusSummaryDelta]
			SELECT 
    			/*[SurveySID]*/ SurveySID,
				/*[ITS]*/ TransientState,
				/*[Cnt]*/ COUNT(ID),
				/*[IsCati]*/ CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
			FROM INSERTED
			GROUP BY SurveySID, TransientState, CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
    END
END