CREATE INDEX [IX_BvPersonDeferredMonitoring_IsComplete_SurveySID_PersonSID] ON [dbo].[BvPersonDeferredMonitoring] 
([IsComplete], [SurveySID], [PersonSID]) 
INCLUDE([InterviewID],[TimeStamp],[HasAudio],[ExtendedStatus],[CallCenterId],[RespondentName],[TelephoneNumber],[InterviewDuration],[IsOldInterface],[IsRetained],[Comment])
