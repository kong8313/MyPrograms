CREATE TABLE [dbo].[BvAppointmentCounters] (
    [SurveySID]             INT            NOT NULL,
    [ProjectID]             NVARCHAR (255) NOT NULL,
    [SurveyName]            NVARCHAR (255) NOT NULL,
    [CountForShortInterval] INT            NOT NULL,
    [CountForLongInterval]  INT            NOT NULL
);

