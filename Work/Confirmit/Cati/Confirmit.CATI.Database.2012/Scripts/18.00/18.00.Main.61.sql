PRINT N'Altering [dbo].[TrCallGroupCondition_Insert]...';


GO
ALTER TRIGGER [dbo].[TrCallGroupCondition_Insert] ON [dbo].[BvCallGroupCondition] FOR INSERT
AS
    ;WITH surveys AS
    (
        SELECT SID FROM BvSurvey WHERE State = 1 AND SurveySchedulingMode = 1
    )
    INSERT INTO BvCallGroupConditionPerSurvey(SurveyId, CallGroupId, ConditionValue, ConditionPriority )
            SELECT SID, CallGroupId, ConditionValue, ConditionPriority FROM inserted, surveys
            WHERE inserted.ConditionPriority <> 0
GO
PRINT N'Altering [dbo].[TrCallGroupCondition_Update]...';


GO
ALTER TRIGGER [dbo].[TrCallGroupCondition_Update] ON [dbo].[BvCallGroupCondition] FOR UPDATE
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
GO
PRINT N'Altering [dbo].[TrSurvey_Changed]...';


GO
ALTER TRIGGER [dbo].[TrSurvey_Changed] ON [dbo].[BvSurvey] FOR INSERT, UPDATE, DELETE
AS
    --insert
    WITH activated AS
    (
        SELECT i.SID FROM inserted i 
        LEFT JOIN deleted d 
        ON i.SID = d.SID 
        WHERE ( i.State = 1 AND i.SurveySchedulingMode = 1 ) AND 
                (d.State <> 1 OR d.SurveySchedulingMode <> 1 OR d.SID IS NULL ) 
    )
    INSERT INTO BvCallGroupConditionPerSurvey(SurveyId, CallGroupId, ConditionValue, ConditionPriority )
        SELECT SID, CallGroupId, ConditionValue, ConditionPriority FROM activated, BvCallGroupCondition
			WHERE ConditionPriority <> 0
                
    --delete
    ;WITH deactivated AS
    (
        SELECT d.SID FROM deleted d
        LEFT JOIN inserted i
        ON i.SID = d.SID 
        WHERE (d.State = 1 AND d.SurveySchedulingMode = 1 ) AND
                ( i.State <> 1 OR i.SurveySchedulingMode <> 1 OR i.SID IS NULL) 
    )
    DELETE FROM BvCallGroupConditionPerSurvey WHERE SurveyId IN ( SELECT SID FROM deactivated )
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Update complete.';


GO
