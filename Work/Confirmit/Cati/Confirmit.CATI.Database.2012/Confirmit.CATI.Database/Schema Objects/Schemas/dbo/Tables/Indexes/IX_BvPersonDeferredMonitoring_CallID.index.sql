CREATE UNIQUE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_CallID] ON [BvPersonDeferredMonitoring]
([CallID]) 
INCLUDE ([PersonSID], [InterviewID], [SurveySID], [TimeStamp], [IsRecording], [IsComplete], [ClientTimeUtc], [ServerTimeUtc], [ExtendedStatus], [InterviewDuration], [RecordCreationTime])
WHERE [CallID] IS NOT NULL