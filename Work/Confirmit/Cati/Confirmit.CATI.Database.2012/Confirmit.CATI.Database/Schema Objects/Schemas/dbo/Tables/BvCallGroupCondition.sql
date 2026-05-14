CREATE TABLE [dbo].[BvCallGroupCondition]
(
	[CallGroupId] INT NOT NULL,
	[ConditionValue] INT NOT NULL,
	[ConditionPriority] INT NOT NULL,
	[RotatePriority] TIMESTAMP NOT NULL,
	CONSTRAINT PK_BvCallGroupCondition PRIMARY KEY ( CallGroupId, ConditionValue )
)
