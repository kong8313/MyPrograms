CREATE TABLE BvScheduleParam
(
    [ScheduleID] INT NOT NULL,
    [SurveySID] INT NOT NULL,
    [ParamID] INT NOT NULL,
    [Name] NVARCHAR(256) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Type] INT NOT NULL,
    [Value] INT NOT NULL
)