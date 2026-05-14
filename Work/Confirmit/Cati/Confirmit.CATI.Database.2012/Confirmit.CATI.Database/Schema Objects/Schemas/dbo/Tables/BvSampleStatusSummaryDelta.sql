CREATE TABLE [dbo].[BvSampleStatusSummaryDelta] (
    [ID]          BIGINT   NOT NULL IDENTITY,
    [SurveySID]   INT NOT NULL,
    [ITS]         INT NOT NULL CONSTRAINT DF_BvSampleStatusSummaryDelta_Its DEFAULT(0),
    [Cnt]         INT NOT NULL CONSTRAINT DF_BvSampleStatusSummaryDelta_Cnt DEFAULT(0),
    [IsCati]      BIT NOT NULL CONSTRAINT DF_BvSampleStatusSummaryDelta_IsCati DEFAULT (0),
	CONSTRAINT [BvSampleStatusSummaryDelta_PK_ID] PRIMARY KEY CLUSTERED ([ID])
);
GO

ALTER TABLE [dbo].[BvSampleStatusSummaryDelta] SET (LOCK_ESCALATION = DISABLE)
GO