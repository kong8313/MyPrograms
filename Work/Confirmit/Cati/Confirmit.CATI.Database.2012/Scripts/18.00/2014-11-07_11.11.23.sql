PRINT 'Add new system setigs:'
GO

WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
	SELECT 'Console.ShowRedialButton', 'Show redial button in Interviewer Console', 'Interviewing', 'Setting for dialer button appearing in console.', 3, 0, 'False'
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data

GO
PRINT N'Update complete.';


GO
