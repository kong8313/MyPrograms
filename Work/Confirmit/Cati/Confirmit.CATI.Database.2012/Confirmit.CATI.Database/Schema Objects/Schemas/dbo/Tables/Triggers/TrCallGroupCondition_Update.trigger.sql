CREATE TRIGGER [dbo].[TrCallGroupCondition_Update] ON [dbo].[BvCallGroupCondition] FOR UPDATE
AS
	MERGE BvCallGroupConditionPerSurvey AS t
	USING
	( 
		SELECT SID, CallGroupId, ConditionValue, ConditionPriority FROM inserted
			INNER JOIN BvSurvey s ON s.State = 1 AND s.SurveySchedulingMode = 1 
	) AS s (SurveyId, CallGroupId, ConditionValue, ConditionPriority)
	ON t.SurveyId = s.SurveyId AND t.CallGroupId = s.CallGroupId AND t.ConditionValue = s.ConditionValue
	WHEN MATCHED AND s.ConditionPriority = 0 THEN 
		DELETE
	WHEN MATCHED AND s.ConditionPriority <> 0 THEN 
		UPDATE SET t.ConditionPriority = s.ConditionPriority
	WHEN NOT MATCHED BY TARGET AND s.ConditionPriority <> 0 THEN
		INSERT (SurveyId, CallGroupId, ConditionValue, ConditionPriority ) 
			VALUES(s.SurveyId, s.CallGroupId, s.ConditionValue, s.ConditionPriority );
