PRINT N'Dropping [dbo].[BvSvySchedule].[IX_BvSvySchedule_CallState]...';


GO
DROP INDEX [IX_BvSvySchedule_CallState]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvySchedule_SurveySid_CallState_i_InterviewID_Priority_TimeInShift_ExplisitSID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSvySchedule_SurveySid_CallState_i_InterviewID_Priority_TimeInShift_ExplicitSID]
    ON [dbo].[BvSvySchedule]([SurveySID], [CallState] ASC)
    INCLUDE([InterviewID], [Priority], [TimeInShift], [ExplicitSID]);


GO
PRINT N'Update complete.';


GO
