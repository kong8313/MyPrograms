CREATE TRIGGER TrCallGroupCondition_Delete ON BvCallGroupCondition FOR DELETE
AS
    DELETE FROM BvCallGroupConditionPerSurvey 
        FROM BvCallGroupConditionPerSurvey cgc
        INNER JOIN deleted d 
        ON cgc.CallGroupId = d.CallGroupId AND cgc.ConditionValue = d.ConditionValue
