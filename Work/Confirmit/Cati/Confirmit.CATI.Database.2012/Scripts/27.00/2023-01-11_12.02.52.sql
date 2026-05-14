GO
PRINT N'Adding Console.EnableAppointmentTimeZoneAdjustment system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Console.EnableAppointmentTimeZoneAdjustment', 'Enable time zone adjustment in the appointment creation UI', 'Console', 'Enable time zone adjustment in the appointment creation UI', 3, 0, 'False'

  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
