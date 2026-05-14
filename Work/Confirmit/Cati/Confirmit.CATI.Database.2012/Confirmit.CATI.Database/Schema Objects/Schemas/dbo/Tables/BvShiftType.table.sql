CREATE TABLE [dbo].[BvShiftType] (
    [OwnerSID] INT           NOT NULL,
    [ID]       INT           NOT NULL,
    [Name]     VARCHAR (255) NOT NULL,
    [Color]    INT           NOT NULL,
    [ObjectID] INT           IDENTITY (1, 1) NOT NULL
);

