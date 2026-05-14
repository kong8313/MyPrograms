DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	IF (NOT EXISTS(SELECT 1 FROM BvSystemSettings WHERE SystemName = 'Setup.InterviewerConsoleVersion'))
	BEGIN
		WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
		(
			SELECT 'Setup.InterviewerConsoleVersion', 'InterviewerConsoleVersion', 'Setup', 'Version of Interviewer Console', 2, 0, ''
		)
		INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT * FROM Data
	END
	
	IF (NOT EXISTS(SELECT 1 FROM BvSystemSettings WHERE SystemName = 'Setup.MonitoringConsoleVersion'))
	BEGIN
		WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
		(
			SELECT 'Setup.MonitoringConsoleVersion', 'MonitoringConsoleVersion', 'Setup', 'Version of Monitoring Console', 2, 0, ''
		)
		INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT * FROM Data
	END
END


GO
PRINT N'Update complete.';


GO
