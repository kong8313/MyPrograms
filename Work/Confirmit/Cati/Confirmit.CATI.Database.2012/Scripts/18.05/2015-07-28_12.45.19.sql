PRINT 'Add new system setigs:'
GO

WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
	SELECT 'Console.EnablePreviousPageToolbarButton', 'ConsoleEnablePreviousPageToolbarButton', 'Interviewing', 'Is Interviewer Console Previous Page toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableNextPageToolbarButton', 'ConsoleEnableNextPageToolbarButton', 'Interviewing', 'Is Interviewer Console Next Page toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableAppointmentToolbarButton', 'ConsoleEnableAppointmentToolbarButton', 'Interviewing', 'Is Interviewer Console Appointment toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableRedoToolbarButton', 'ConsoleEnableRedoToolbarButton', 'Interviewing', 'Is Interviewer Console Redo toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableFastForwardToolbarButton', 'ConsoleEnableFastForwardToolbarButton', 'Interviewing', 'Is Interviewer Console Fast Forward toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableCheckSpellingToolbarButton', 'ConsoleEnableCheckSpellingToolbarButton', 'Interviewing', 'Is Interviewer Console Check Spelling toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableRedialToolbarButton', 'ConsoleEnableRedialToolbarButton', 'Interviewing', 'Is Interviewer Console Redial toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableHangUpToolbarButton', 'ConsoleEnableHangUpToolbarButton', 'Interviewing', 'Is Interviewer Console Hang Up toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableLogoutAfterFinishToolbarButton', 'ConsoleEnableLogoutAfterFinishToolbarButton', 'Interviewing', 'Is Interviewer Console Logout After Finish toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableTerminateToolbarButton', 'ConsoleEnableTerminateToolbarButton', 'Interviewing', 'Is Interviewer Console Terminate toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableTakeBreakToolbarButton', 'ConsoleEnableTakeBreakToolbarButton', 'Interviewing', 'Is Interviewer Console Take Break toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableChangeTaskChoiceToolbarButton', 'ConsoleEnableChangeTaskChoiceToolbarButton', 'Interviewing', 'Is Interviewer Console Change Task Choicee toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableMessageFormToolbarButton', 'ConsoleEnableMessageFormToolbarButton', 'Interviewing', 'Is Interviewer Console Message Form toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableAppointmensListToolbarButton', 'ConsoleEnableAppointmensListToolbarButton', 'Interviewing', 'Is Interviewer Console Appointmense toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableRefreshToolbarButton', 'ConsoleEnableRefreshToolbarButton', 'Interviewing', 'Is Interviewer Console Refresh toolbar button enabled', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableLogoutToolbarButton', 'ConsoleEnableLogoutToolbarButton', 'Interviewing', 'Is Interviewer Console Logout toolbar button enabled', 3, 0, 'True'
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data

GO
PRINT N'Update complete.';

GO
