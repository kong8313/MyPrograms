CREATE TABLE [dbo].[BvSamples] (
    [BatchID]          INT            NOT NULL,
    [SurveySID]        INT            NOT NULL,
    [State]            INT            NOT NULL,
    [StateDescription] NVARCHAR(MAX)  NOT NULL,
    [StartedTime]      DATETIME       NOT NULL,
    [FinishedTime]     DATETIME       NULL,
    [CountInterviews]  INT            NOT NULL,
    [SampleType]       INT            NOT NULL CONSTRAINT DF_BvSamples_SampleType DEFAULT(0)
);
