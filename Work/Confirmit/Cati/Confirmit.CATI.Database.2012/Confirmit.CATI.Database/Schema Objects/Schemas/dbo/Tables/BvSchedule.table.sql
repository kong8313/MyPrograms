CREATE TABLE [dbo].[BvSchedule] (
    [ScheduleID]			INT                           NOT NULL,
    [XmlInUse]				NVARCHAR (MAX)                NOT NULL CONSTRAINT DF_BvSchedule_XmlInUse DEFAULT(N''),
    [XmlUnderDev]			NVARCHAR (MAX)                NOT NULL CONSTRAINT DF_BvSchedule_XmlUnderDev DEFAULT(N''),
    [ScriptSource]			NVARCHAR (MAX)                NULL,
    [Name]					NVARCHAR (255)                NOT NULL,
    [CreateDate]			DATETIME                      NOT NULL,
    [ModifyDate]			DATETIME                      NOT NULL,
    [RegenerateIsRequired]	     BIT				      NOT NULL CONSTRAINT DF_BvSchedule_RegenerateIsRequired DEFAULT(0),
    [DesignStateGroupID]	INT                           NULL CONSTRAINT [FK_BvSchedule_BvStateGroup] FOREIGN KEY ([DesignStateGroupID]) REFERENCES [BvStateGroup] ([ID]) ON DELETE SET NULL,
	[IsSampleUpdateRuleSet] BIT                          NOT NULL CONSTRAINT DF_BvSchedule_IsSampleUpdateRuleSet DEFAULT(0)
);

