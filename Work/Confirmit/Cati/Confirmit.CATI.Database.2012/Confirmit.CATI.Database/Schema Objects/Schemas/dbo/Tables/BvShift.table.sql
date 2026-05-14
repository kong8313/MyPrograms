CREATE TABLE [dbo].[BvShift] (
    [OwnerSID]        INT      NOT NULL,
    [ID]              INT      NOT NULL,
    [CycleType]       INT      NOT NULL,
    [StartDayOfWeek]  INT      NULL,
    [StartTime]       DATETIME NOT NULL,
    [FinishDayOfWeek] INT      NULL,
    [FinishTime]      DATETIME NOT NULL,
    [ShiftTypeID]     INT      NOT NULL
);

