GO
PRINT N'Dropping Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_CallID]...';


GO
DROP INDEX [IX_BvPersonDeferredMonitoring_CallID]
    ON [dbo].[BvPersonDeferredMonitoring];


GO
PRINT N'Dropping Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_TimeStamp]...';


GO
DROP INDEX [IX_BvPersonDeferredMonitoring_TimeStamp]
    ON [dbo].[BvPersonDeferredMonitoring];

GO
PRINT N'Dropping Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_SurveySID]...';


GO
DROP INDEX [IX_BvPersonDeferredMonitoring_SurveySID]
    ON [dbo].[BvPersonDeferredMonitoring];


GO
PRINT N'Dropping Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_TelephoneNumber]...';


GO
DROP INDEX [IX_BvPersonDeferredMonitoring_TelephoneNumber]
    ON [dbo].[BvPersonDeferredMonitoring];


GO
PRINT N'Creating Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_CallID]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_CallID]
    ON [dbo].[BvPersonDeferredMonitoring]([CallID] ASC)
    INCLUDE([PersonSID], [InterviewID], [SurveySID], [TimeStamp], [IsRecording], [IsComplete], [ClientTimeUtc], [ServerTimeUtc], [ExtendedStatus], [InterviewDuration], [RecordCreationTime]) WHERE [CallID] IS NOT NULL;


GO
PRINT N'Creating Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_TimeStamp]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_TimeStamp]
    ON [dbo].[BvPersonDeferredMonitoring]([TimeStamp] ASC, [IsRetained] ASC)
    ON [PRIMARY];


GO
PRINT N'Creating Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_IsComplete]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_IsComplete]
    ON [dbo].[BvPersonDeferredMonitoring]([IsComplete] ASC)
    INCLUDE([PersonSID], [InterviewID], [SurveySID], [TimeStamp], [HasAudio], [ExtendedStatus], [CallCenterId], [RespondentName], [TelephoneNumber], [InterviewDuration], [IsOldInterface], [IsRetained], [Comment]);


GO
PRINT N'Creating Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_IsRetained_SurveySID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_IsRetained_SurveySID]
    ON [dbo].[BvPersonDeferredMonitoring]([IsRetained] ASC, [SurveySID] ASC);


GO
PRINT N'Creating Index [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_SurveySID_InterviewId_PersonSID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_SurveySID_InterviewId_PersonSID]
    ON [dbo].[BvPersonDeferredMonitoring]([SurveySID] ASC, [InterviewID] ASC, [PersonSID] ASC);


GO
PRINT N'Creating Index [dbo].[BvPerson].[IX_BvPerson_CallCenterId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPerson_CallCenterId]
    ON [dbo].[BvPerson]([CallCenterID] ASC);


GO
PRINT N'Creating Index [dbo].[BvUserSurveyPermission].[IX_BvUserSurveyPermission_SurveySID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvUserSurveyPermission_SurveySID]
    ON [dbo].[BvUserSurveyPermission]([SurveySID] ASC)
    INCLUDE([UserName]);


GO
PRINT N'Update complete.';


GO
