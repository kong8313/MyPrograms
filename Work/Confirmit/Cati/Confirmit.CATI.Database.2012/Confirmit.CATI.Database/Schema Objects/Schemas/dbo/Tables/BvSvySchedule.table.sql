CREATE TABLE [dbo].[BvSvySchedule] (
    [ID]           INT              IDENTITY (1, 1) NOT NULL,
    [ApptID]       INT              NOT NULL,
    [ShiftTypeID]  INT              NOT NULL,
    [InterviewID]  INT              NULL,
    [SurveySID]    INT              NOT NULL,
    [CallState]    INT              NOT NULL,
    [Priority]     INT              NOT NULL,
    [TimeInShift]  DATETIME         NULL,
    [ExpireTime]   DATETIME         NOT NULL CONSTRAINT DF_BvSvySchedule_ExpireTime DEFAULT ('9999-01-01 00:00:00.000'),
    [ExplicitSID]  INT              NOT NULL,
    [ExplicitType] INT              NOT NULL,
    [RuleNumber]   UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_BvSvySchedule_RuleNumber DEFAULT ('00000000-0000-0000-0000-000000000000'),
    [CallOrder]    INT              NOT NULL CONSTRAINT DF_BvSvySchedule_CallOrder DEFAULT (0),
    [OldPriority]  INT              NOT NULL CONSTRAINT DF_BvSvySchedule_OldPriority DEFAULT (0),
    [ConditionValue] INT            NOT NULL CONSTRAINT DF_BvSvySchedule_ConditionValue DEFAULT (0),
    [CellId]       INT              NOT NULL CONSTRAINT DF_BvSvySchedule_CellId DEFAULT(0),
    [DialTypeId]   TINYINT          NOT NULL,
    [Type]         TINYINT          NOT NULL CONSTRAINT DF_BvSvySchedule_Type DEFAULT(0),
	[DialerId]     INT              NOT NULL CONSTRAINT DF_BvSvySchedule_DialerId DEFAULT(0),
	[ActiveDialId] BIGINT           NOT NULL CONSTRAINT DF_BvSvySchedule_ActiveDialId DEFAULT(0)
);

