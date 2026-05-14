PRINT N'Creating [dbo].[BvViewInnerShiftType]...';


GO
create view dbo.[BvViewInnerShiftType] 
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
