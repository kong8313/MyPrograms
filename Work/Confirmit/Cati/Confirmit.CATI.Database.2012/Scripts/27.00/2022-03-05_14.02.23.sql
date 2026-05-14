PRINT N'Add dialer reconection timespan and ExpectedIsActiveState column to BvDialers.';
GO

ALTER TABLE BvDialers
ADD ReconnectionDuration INT CONSTRAINT [DF_BvDialers_ReconnectionDuration] DEFAULT (7200000) WITH VALUES,
	ExpectedState INT NOT NULL CONSTRAINT [DF_BvDialers_ExpectedState] DEFAULT (2/*DisconnectedAndDiactivated*/) WITH VALUES;

GO

PRINT N'Normalize dialers expected state';
GO

UPDATE [dbo].[BvDialers]
SET [ExpectedState] = 2/*DisconnectenAndDiactivated*/ - CAST(IsActive AS INT) - CAST(DialerOperationalStateNotification AS INT)

GO

PRINT N'Update complete.';

GO