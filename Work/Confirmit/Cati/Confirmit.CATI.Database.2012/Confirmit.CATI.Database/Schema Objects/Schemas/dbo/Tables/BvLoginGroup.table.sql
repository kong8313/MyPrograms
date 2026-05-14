CREATE TABLE [dbo].[BvLoginGroup] (
    [PersonSID]   INT NOT NULL,
    [ObjectSID]   INT NOT NULL,
    [SurveySID]   INT NOT NULL,
    [DialTypeId]  TINYINT NOT NULL CONSTRAINT [DF_BvLoginGroup_DialTypeId] DEFAULT (0)
);

