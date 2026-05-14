CREATE TABLE [dbo].[BvAppointment] (
    [SurveySID]      INT              NOT NULL,
    [InterviewSID]   INT              NOT NULL,
    [Time]           DATETIME         NOT NULL,
    [ExpTime]        DATETIME         NULL,
    [RespondentName] NVARCHAR (255)   NULL,
    [ID]             INT              IDENTITY (1, 1) NOT NULL,
    [State]          INT              NOT NULL,
    [ContactName]    NVARCHAR (255)   NOT NULL,
    [BatchID]        INT              NOT NULL CONSTRAINT DF_BvAppointment_BatchID DEFAULT (0),
    [TempID]         INT              NOT NULL CONSTRAINT DF_BvAppointment_TempID DEFAULT (0),
    [TZID]           INT              NULL
);

