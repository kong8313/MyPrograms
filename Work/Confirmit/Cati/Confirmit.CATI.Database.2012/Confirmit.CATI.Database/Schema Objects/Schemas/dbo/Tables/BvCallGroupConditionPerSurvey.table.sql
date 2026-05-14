	CREATE TABLE [dbo].[BvCallGroupConditionPerSurvey]
	(
		[SurveyId] INT NOT NULL,
		[CallGroupId] INT NOT NULL,
		[ConditionValue] INT NOT NULL,
		[ConditionPriority] INT NOT NULL,
		[RotatePriority] TIMESTAMP NOT NULL,
		CONSTRAINT PK_BvCallGroupConditionPerSurvey PRIMARY KEY ( SurveyId, CallGroupId, ConditionValue )
	)

