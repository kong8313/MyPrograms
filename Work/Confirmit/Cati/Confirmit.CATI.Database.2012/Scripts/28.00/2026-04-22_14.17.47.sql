GO
PRINT N'Add Toggle.BvSvyScheduleDeadlockReduction system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Toggle.BvSvyScheduleDeadlockReduction', 'When eneabled, BvSvySchedule table usage is reduced.', 'Toggle', 'When eneabled, ActiveDialId and DialerId columns of BvSvySchedule table are not used.', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';
