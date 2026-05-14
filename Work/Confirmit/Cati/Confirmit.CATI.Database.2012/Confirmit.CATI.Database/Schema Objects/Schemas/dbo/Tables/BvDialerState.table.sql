CREATE TABLE [dbo].[BvDialerState]
(
    [ServerName] VARCHAR(50) NOT NULL , 
    [DialerId] INT NOT NULL, 
    [LatestGetStateRequestDateTime]    DATETIME     NOT NULL CONSTRAINT DF_BvDialerState_LatestGetStateRequestDateTime DEFAULT ('01/01/1900'), 
    [LatestSuccessfulGetStateDateTime] DATETIME     NOT NULL CONSTRAINT DF_BvDialerState_LatestSuccessfulGetStateDateTime DEFAULT ('01/01/1900'),
    [LatestDialerNotificationDateTime] DATETIME     NOT NULL CONSTRAINT DF_BvDialerState_LatestDialerNotificationDateTime DEFAULT ('01/01/1900'),
    [DialerNotificationExpirationTime] DATETIME     NOT NULL CONSTRAINT DF_BvDialerState_DialerNotificationExpirationTime DEFAULT ('01/01/1900'),
    CONSTRAINT PK_BvDialerState_ServerName PRIMARY KEY ([ServerName], [DialerId])
)
