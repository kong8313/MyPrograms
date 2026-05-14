CREATE TABLE [dbo].[BvFilterFields] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [FilterSID] INT            NOT NULL,
    [Table]     INT            NOT NULL,
    [Column]    NVARCHAR (255) NOT NULL,
    [Type]      INT            NOT NULL,
    [Sign]      INT            NOT NULL,
    [Value]     NVARCHAR (255) NOT NULL,
    [IsNeedCast] BIT           NOT NULL CONSTRAINT DF_BvFilterFields_IsNeedCast DEFAULT(0)
);

