GO
PRINT N'Dropping Index [dbo].[BvSvySchedule].[IX_BvSvySchedule_SurveySid_CallState_i_InterviewID_Priority_TimeInShift_ExplicitSID]...';


GO
DROP INDEX [IX_BvSvySchedule_SurveySid_CallState_i_InterviewID_Priority_TimeInShift_ExplicitSID]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Creating Index [dbo].[BvSvySchedule].[IX_BvSvySchedule_SurveySid_CallState_DialTypeId_ShiftTypeID_ExplicitSID_TimeInShift_Priority]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSvySchedule_SurveySid_CallState_DialTypeId_ShiftTypeID_ExplicitSID_TimeInShift_Priority]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [CallState] ASC, [DialTypeId] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC, [TimeInShift] ASC, [Priority] DESC)
    INCLUDE([ID], [InterviewID]);


GO
PRINT N'Update complete.';


GO
