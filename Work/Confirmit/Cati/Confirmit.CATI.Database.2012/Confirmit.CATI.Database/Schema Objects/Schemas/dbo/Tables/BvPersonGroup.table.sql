CREATE TABLE [dbo].[BvPersonGroup] (
    [SID]                  INT            NOT NULL,
    [Name]                 NVARCHAR (255) NOT NULL,
    [Description]          NVARCHAR (255) NOT NULL,
    [InboundCallBehavior]  TINYINT        NOT NULL,
    [CallTransferBehavior] TINYINT        NOT NULL, 
    [IsAdministrative]     BIT            NOT NULL CONSTRAINT DF_BvPersonGroup_IsAdministrative DEFAULT 0
);

