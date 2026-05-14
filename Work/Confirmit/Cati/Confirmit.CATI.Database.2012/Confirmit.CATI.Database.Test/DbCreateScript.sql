CREATE TABLE [dbo].[BvTest] (
    [Id]        INT IDENTITY (1, 1) NOT NULL,
    [TestField] INT NOT NULL,
    CONSTRAINT [PK_BvTest_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE SEQUENCE [dbo].[BvTestSequence]
    AS INT
    START WITH 1
    INCREMENT BY 1;



GO
CREATE VIEW [dbo].[vTest]
WITH SCHEMABINDING
as
    SELECT BvTest.Id FROM [dbo].[BvTest]


GO
CREATE UNIQUE CLUSTERED INDEX [pk_vTest]
    ON [dbo].[vTest]([Id] ASC);


GO
CREATE PROCEDURE [dbo].[BvSpTest]
	@Id int,
	@testField int
AS
	UPDATE BvTest set TestField = @testField
		WHERE id = @id
RETURN 0
