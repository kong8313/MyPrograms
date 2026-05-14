GO
PRINT N'removing system setting RecordedInterviews.IsOldStyleEnabled';

delete from BvSystemSettings where SystemName = 'RecordedInterviews.IsOldStyleEnabled'

GO
PRINT N'Update complete.';


GO
