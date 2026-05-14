GO
PRINT N'Add Toggle.EnableInterviewerMetricsConfiguration system settings';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
            (
			SELECT 'Toggle.EnableInterviewerMetricsConfiguration', 'Enable configuration of interviewer performance metrics', 'Toggle', 'When enabled, Interviewer statistic settings will be available under the Admin menu', 3, 0, 'False'
			)
   INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
SELECT * FROM Data

END