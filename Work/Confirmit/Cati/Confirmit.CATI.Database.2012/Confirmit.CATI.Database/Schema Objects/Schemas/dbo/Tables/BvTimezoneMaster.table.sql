CREATE TABLE [dbo].[BvTimezoneMaster] (
    [ID]                INT            NOT NULL,
    [Name]              NVARCHAR (255) NOT NULL,
    [Bias]              INT            NOT NULL,
    [DaylightType]      INT            NOT NULL,
    [StandardName]      NVARCHAR (255) NOT NULL,
    [StandardStart]     DATETIME       NULL,
    [StandardDayOfWeek] INT            NULL,
    [StandardBias]      INT            NOT NULL,
    [DaylightName]      NVARCHAR (255) NOT NULL,
    [DaylightStart]     DATETIME       NULL,
    [DaylightDayOfWeek] INT            NULL,
    [DaylightBias]      INT            NOT NULL,
    CONSTRAINT PK_BvTimezoneMaster_Id PRIMARY KEY CLUSTERED ([ID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);

