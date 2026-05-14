CREATE TABLE [dbo].[BvTimezoneShift] (
    [OwnerSID]        INT      NOT NULL,
    [ShiftID]         INT      NOT NULL,
    [TimezoneID]      INT      NOT NULL,
    [StartDayOfWeek]  INT      NULL,
    [StartTime]       DATETIME NOT NULL,
    [FinishDayOfWeek] INT      NULL,
    [FinishTime]      DATETIME NOT NULL,
    CONSTRAINT FK_BvTimezoneShift_TimezoneID FOREIGN KEY ([TimezoneID]) REFERENCES [dbo].[BvTimezone] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

