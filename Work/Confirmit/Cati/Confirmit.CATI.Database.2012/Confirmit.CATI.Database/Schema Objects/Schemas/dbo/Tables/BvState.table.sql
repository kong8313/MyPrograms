CREATE TABLE [dbo].[BvState] (
    [StateID]      INT            NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [Priority]     INT            NOT NULL CONSTRAINT DF_BvState_Priority DEFAULT(1),
    [StateGroupID] INT            NOT NULL CONSTRAINT DF_BvState_StateGroupID DEFAULT(0),
    [DA]           INT            NOT NULL CONSTRAINT DF_BvState_DA DEFAULT(0),
	[FcdAction]    BIT            NOT NULL CONSTRAINT DF_BvState_FcdAction DEFAULT(0),
    [AaporCode]        NVARCHAR (10)
);

