CREATE TABLE [dbo].[BvAppointmentsAlertStatus] (
    [ID]              INT            NOT NULL,
    [SurveySID]       INT            NOT NULL,
    [SurveyName]      NVARCHAR (255) NOT NULL,
    [ProjectID]       NVARCHAR (255) NOT NULL,
    [InterviewID]     INT            NOT NULL,
    [AppointmentTime] DATETIME       NOT NULL,
    [TZID]            INT            NULL,
    [Resource]        NVARCHAR (255) NULL,
    [Contact]         NVARCHAR (255) NOT NULL,
    [AlertStatus]     INT            NOT NULL CONSTRAINT DF_BvAppointmentsAlertStatus_AlertStatus DEFAULT(0),
    [CallID]          INT            NOT NULL CONSTRAINT DF_BvAppointmentsAlertStatus_CallID DEFAULT(0),
    [ExtendedStatus]  INT            NOT NULL
);

