CREATE INDEX [IX_BvPersonDeferredMonitoring_IsComplete_TimeStamp] ON [dbo].[BvPersonDeferredMonitoring] 
([IsComplete], [TimeStamp]) 
INCLUDE([InterviewID],[SurveySID], [PersonSID], [HasAudio],[ExtendedStatus],[CallCenterId],[RespondentName],[TelephoneNumber],[InterviewDuration],[IsOldInterface],[IsRetained],[Comment])
