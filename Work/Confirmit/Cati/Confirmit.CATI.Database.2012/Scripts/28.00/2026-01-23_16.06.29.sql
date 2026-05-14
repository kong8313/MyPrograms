PRINT N'Add Assisted dial type (Id=2) to BvDialType';
GO

IF NOT EXISTS (SELECT 1 FROM dbo.BvDialType WHERE [ID] = 2)
	BEGIN
		INSERT INTO dbo.BvDialType([ID], [Name])
			VALUES (2, N'Assisted');
		PRINT N'Inserted BvDialType: (ID=2, Name=Assisted)';
	END
ELSE
	BEGIN
        PRINT N'Skipped: BvDialType already contains ID=2';
    END

GO

PRINT N'Update complete.';
GO