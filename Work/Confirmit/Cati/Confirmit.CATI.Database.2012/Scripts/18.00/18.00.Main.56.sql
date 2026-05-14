
GO

DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name FROM SYS.DEFAULT_CONSTRAINTS
WHERE PARENT_OBJECT_ID = OBJECT_ID('BvPerson')
AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns
                        WHERE NAME = N'PwdSetDate'
                        AND object_id = OBJECT_ID(N'BvPerson'))

IF @ConstraintName IS NOT NULL
EXEC('ALTER TABLE BvPerson DROP CONSTRAINT ' + @ConstraintName)

GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_PwdSetDate] DEFAULT GETUTCDATE() FOR [PwdSetDate];

GO

