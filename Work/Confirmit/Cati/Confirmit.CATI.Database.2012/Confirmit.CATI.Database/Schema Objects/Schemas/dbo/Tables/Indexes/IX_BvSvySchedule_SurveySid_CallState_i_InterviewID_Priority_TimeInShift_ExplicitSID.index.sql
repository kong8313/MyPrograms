CREATE NONCLUSTERED INDEX [IX_BvSvySchedule_SurveySid_CallState_DialTypeId_ShiftTypeID_ExplicitSID_TimeInShift_Priority]       
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [CallState] ASC, [DialTypeId] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC, [TimeInShift] ASC, [Priority] DESC)
    INCLUDE([ID], [InterviewID])
