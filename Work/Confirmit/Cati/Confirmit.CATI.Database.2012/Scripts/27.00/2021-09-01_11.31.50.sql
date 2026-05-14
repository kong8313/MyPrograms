GO
PRINT N'Removing schema binding from [dbo].[BvViewInnerShiftType]...';


GO
ALTER VIEW [dbo].[BvViewInnerShiftType]
AS
SELECT [BvShiftZones].[ID] AS [ShiftTypeId],
       [BvShiftType].[Name] AS [ShiftTypeName]
FROM   dbo.[BvShiftZones]
       INNER JOIN
       dbo.[BvShiftType]
       ON [BvShiftType].[ObjectID] = [BvShiftZones].[ShiftTypeID]
UNION
SELECT -2147483648,
       '[None]'
UNION
SELECT -id,
       '[AnyValid]'
FROM   dbo.[BvTimezone]
UNION
SELECT 0,
       '[AnyValid]';


GO
PRINT N'Altering [dbo].[BvTimezone]...';


GO
ALTER TABLE [dbo].[BvTimezone]
    ADD [ParentID] INT NULL;


GO
PRINT N'Adding schema binding to [dbo].[BvViewInnerShiftType]...';


GO
ALTER view dbo.[BvViewInnerShiftType] 
with schemabinding
as
select [BvShiftZones].[ID] as [ShiftTypeId], [BvShiftType].[Name] as [ShiftTypeName]
from dbo.[BvShiftZones]
INNER JOIN dbo.[BvShiftType] ON  [BvShiftType].[ObjectID] = [BvShiftZones].[ShiftTypeID]
union 
select -2147483648, '[None]'
union 
select -id, '[AnyValid]'
from dbo.[BvTimezone]
union
select 0, '[AnyValid]'


GO
PRINT N'Altering [dbo].[BvSpTimezone_Activate]...';


GO
ALTER PROCEDURE [dbo].[BvSpTimezone_Activate]
    @TzID INT
AS
    IF NOT EXISTS( SELECT 1 FROM BvTimezoneMaster WHERE ID = @TzID )
    BEGIN
        RAISERROR( 'Timezone with ID = ''%d'' not found in master list', 16, 1, @TzID )
        RETURN -1
    END

    INSERT INTO BvTimezone 
        SELECT *, NULL as ParentID FROM BvTimezoneMaster 
            WHERE ID = @TzID AND ID NOT IN( SELECT ID FROM BvTimezone )

    RETURN @@ROWCOUNT
	
GO
PRINT N'Altering [dbo].[BvSpTimezone_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpTimezone_Delete]
        @ID     int,
        @Mode   int
AS

DECLARE @Rows int
DECLARE @res bit

SELECT @Rows = COUNT( * ) FROM BvTimezone WHERE ID = @ID

IF @Rows = 0 
  BEGIN
    RAISERROR( 'Timezone %i not exists', 16, 1, @ID )
    RETURN -1
  END

SELECT @Rows = COUNT( * ) FROM BvTimezoneShift WHERE TimezoneID = @ID

IF @Rows <> 0 
  BEGIN
    RAISERROR( 'Unable to delete timezone %i. Link exists on timezone shift', 12, 1, @ID )
    RETURN -1
  END

IF EXISTS( SELECT TOP 1 BvSvySchedule.[ID] 
             FROM BvSvySchedule, BvShiftZones
            WHERE BvShiftZones.TimeZoneID = @ID
                  AND BvSvySchedule.ShiftTypeID = BvShiftZones.[ID] )
BEGIN
    RAISERROR( 'Unable to delete timezone %i. Link exists on calls', 12, 1, @ID )
    RETURN -1
END

IF EXISTS(SELECT * FROM BvCallCenter WHERE LocalTimezoneId = @ID)
BEGIN
	RAISERROR('Unable to delete timezone %i. The timezone is used in some call center', 12, 1, @ID)
	RETURN -1
END
  
SELECT @res = COUNT(*)
FROM BvInterview
WHERE TimezoneID = @ID

IF @res <> 0
BEGIN
  RAISERROR( 'Unable to delete timezone %i. Link exist on interview', 12, 1, @ID )
  RETURN -1
END

IF EXISTS(SELECT * FROM BvTimezone WHERE ParentID = @ID)
BEGIN
	RAISERROR('Unable to delete timezone %i. The timezone has custom timezones', 12, 1, @ID)
	RETURN -1	
END



BEGIN TRANSACTION

DELETE BvTimezone WHERE ID = @ID

COMMIT TRANSACTION

RETURN 0

GO
PRINT N'Creating [dbo].[CustomTimezoneIdSequence]...';


GO
CREATE SEQUENCE [dbo].[CustomTimezoneIdSequence]
    AS INT
    START WITH 1000
    INCREMENT BY 1;

GO
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing [dbo].[BvSpGetActiveShifts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveShifts]';


GO
PRINT N'Refreshing [dbo].[BvSpGetAppointmentActivity]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAppointmentActivity]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCallsSentToDialerDistribution]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCallsSentToDialerDistribution]';


GO
PRINT N'Refreshing [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


GO
PRINT N'Refreshing [dbo].[BvSpGetLiveShifts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetLiveShifts]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpShiftType_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpShiftType_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_DeleteUnused]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_DeleteUnused]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezoneMaster_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezoneMaster_Get]';


GO
PRINT N'Update complete.';


GO
