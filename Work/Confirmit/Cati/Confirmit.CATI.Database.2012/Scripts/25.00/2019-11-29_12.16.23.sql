UPDATE [dbo].[BvInterviewerProductivityReportTemplate] 
SET ShowDialerAttempts = 1
Where Name = 'System template'

GO
PRINT N'Update complete.';


GO
