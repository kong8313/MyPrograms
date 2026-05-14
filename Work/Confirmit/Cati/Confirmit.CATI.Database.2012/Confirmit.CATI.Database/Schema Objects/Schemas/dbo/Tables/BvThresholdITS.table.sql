CREATE TABLE [dbo].[BvThresholdITS] (
    [SurveySID] INT NOT NULL,
    [ITS]       INT NOT NULL,
    [Amber]     INT NOT NULL CONSTRAINT DF_BvThresholdITS_Amber DEFAULT (2147483647),
    [Red]       INT NOT NULL CONSTRAINT DF_BvThresholdITS_Red DEFAULT (2147483647)
);

