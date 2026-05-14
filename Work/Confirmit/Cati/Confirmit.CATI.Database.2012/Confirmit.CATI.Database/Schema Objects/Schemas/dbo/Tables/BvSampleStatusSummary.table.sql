CREATE TABLE [dbo].[BvSampleStatusSummary] (
    [SurveySID]   INT NOT NULL,
    [ITS]         INT NOT NULL,
    [Cnt]         INT NOT NULL CONSTRAINT DF_BvSampleStatusSummary_Cnt DEFAULT(0),
    [AlertStatus] INT NOT NULL CONSTRAINT DF_BvSampleStatusSummary_AlertStatus DEFAULT(0), 
    [IsCati]      BIT NOT NULL CONSTRAINT DF_BvSampleStatusSummary_IsCati DEFAULT (0)
);

