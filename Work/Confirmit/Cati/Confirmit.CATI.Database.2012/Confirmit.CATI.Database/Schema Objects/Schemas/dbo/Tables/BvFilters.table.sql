CREATE TABLE [dbo].[BvFilters] (
    [SID]          INT            NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [Description]  NVARCHAR (255) NOT NULL,
    [AndOrOperator]TINYINT        NOT NULL,
    [SurveySID]    INT            NOT NULL,
    [Hidden]       TINYINT        NOT NULL
);

