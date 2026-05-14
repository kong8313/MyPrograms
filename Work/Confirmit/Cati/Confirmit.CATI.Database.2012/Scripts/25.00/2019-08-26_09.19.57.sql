PRINT N'Add Email.FeedbackSupportEmailAddress system setting';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	IF (NOT EXISTS(SELECT 1 FROM BvSystemSettings WHERE SystemName = 'Email.FeedbackSupportEmailAddress'))
	BEGIN
		WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
		(
			SELECT 'Email.FeedbackSupportEmailAddress', 'Support Email for Feedback', 'Logging', 'Support email that is being used in feedback functionality', 2, 0, 'support@confirmit.com'
		)
		INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT * FROM Data
	END
END


GO
PRINT N'Update complete.';


GO
