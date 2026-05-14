CREATE TABLE [dbo].[BvReport] (
    [Rpt_ID]             INT            IDENTITY (1, 1) NOT NULL,
    [Rpt_TargetClassID]  INT            NOT NULL,
    [Rpt_Name]           NVARCHAR (255) NOT NULL,
    [Rpt_FileName]       NVARCHAR (255) NOT NULL,
    [Rpt_DialogFileName] NVARCHAR (255) NOT NULL
);

