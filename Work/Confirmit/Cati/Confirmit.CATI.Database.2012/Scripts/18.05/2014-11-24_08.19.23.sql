PRINT 'Add new system setigs:'
GO

WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
	SELECT 'Console.ShowRedialButtonSetting', 'Show redial button setting in Supervisor', 'Supervisor', 'Setting appearance', 3, 0, 'False'
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data

GO
PRINT N'Update complete.';


GO
