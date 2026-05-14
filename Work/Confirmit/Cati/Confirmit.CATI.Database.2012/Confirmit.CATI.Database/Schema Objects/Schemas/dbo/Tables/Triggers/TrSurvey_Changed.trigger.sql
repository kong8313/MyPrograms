CREATE TRIGGER [dbo].[TrSurvey_Changed] ON [dbo].[BvSurvey] FOR INSERT, UPDATE, DELETE
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