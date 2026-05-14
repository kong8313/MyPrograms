PRINT N'Dropping [dbo].[BvAsyncTriggerMessages]...';


GO
DROP TABLE [dbo].[BvAsyncTriggerMessages];


GO
PRINT N'Creating [dbo].[BvAsyncTriggerNotifications]...';


GO
CREATE TABLE [dbo].[BvAsyncTriggerNotifications] (
    [Name] VARCHAR (256)  NOT NULL,
    [Body] NVARCHAR (MAX) NOT NULL
);


GO
PRINT N'Update complete.';


GO
