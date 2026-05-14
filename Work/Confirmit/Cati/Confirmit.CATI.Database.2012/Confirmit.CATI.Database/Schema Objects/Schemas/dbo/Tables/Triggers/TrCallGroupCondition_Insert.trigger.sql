CREATE TRIGGER [dbo].[TrCallGroupCondition_Insert] ON [dbo].[BvCallGroupCondition] FOR INSERT
AS
    ;WITH surveys AS
    (
        SELECT SID FROM BvSurvey WHERE State = 1 AND SurveySchedulingMode = 1
    )
    INSERT INTO BvCallGroupConditionPerSurvey(SurveyId, CallGroupId, ConditionValue, ConditionPriority )
            SELECT SID, CallGroupId, ConditionValue, ConditionPriority FROM inserted, surveys
            WHERE inserted.ConditionPriority <> 0

			