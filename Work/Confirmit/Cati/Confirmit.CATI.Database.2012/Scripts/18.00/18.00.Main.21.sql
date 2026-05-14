GO
PRINT N'Altering [dbo].[BvDialerState]...';


GO
ALTER TABLE [dbo].[BvDialerState]
    ADD [DialerNotificationExpirationTime] DATETIME NOT NULL CONSTRAINT DF_BvDialerState_DialerNotificationExpirationTime DEFAULT ('01/01/1900')
GO
PRINT N'Update complete.';


GO
