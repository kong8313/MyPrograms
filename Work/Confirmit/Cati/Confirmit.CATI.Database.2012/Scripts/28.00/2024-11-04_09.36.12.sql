GO
PRINT N'Dropping Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_IsComplete]...';


GO
DROP INDEX [IX_BvPersonDeferredMonitoring_IsComplete]
    ON [dbo].[BvPersonDeferredMonitoring];


GO
PRINT N'Creating Column Store Index [dbo].[BvPersonDeferredMonitoring].[CSIX_BvPersonDeferredMonitoring_IsComplete_SurveySID]...';


GO
CREATE COLUMNSTORE INDEX [CSIX_BvPersonDeferredMonitoring_IsComplete_SurveySID]
    ON [dbo].[BvPersonDeferredMonitoring]([IsComplete], [SurveySID]);


GO
PRINT N'Creating Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_IsComplete_SurveySID_PersonSID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_IsComplete_SurveySID_PersonSID]
    ON [dbo].[BvPersonDeferredMonitoring]([IsComplete] ASC, [SurveySID] ASC, [PersonSID] ASC)
    INCLUDE([InterviewID], [TimeStamp], [HasAudio], [ExtendedStatus], [CallCenterId], [RespondentName], [TelephoneNumber], [InterviewDuration], [IsOldInterface], [IsRetained], [Comment]);


GO
PRINT N'Creating Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_IsComplete_TimeStamp]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_IsComplete_TimeStamp]
    ON [dbo].[BvPersonDeferredMonitoring]([IsComplete] ASC, [TimeStamp] ASC)
    INCLUDE([InterviewID], [SurveySID], [PersonSID], [HasAudio], [ExtendedStatus], [CallCenterId], [RespondentName], [TelephoneNumber], [InterviewDuration], [IsOldInterface], [IsRetained], [Comment]);


GO
PRINT N'Update complete.';


GO
