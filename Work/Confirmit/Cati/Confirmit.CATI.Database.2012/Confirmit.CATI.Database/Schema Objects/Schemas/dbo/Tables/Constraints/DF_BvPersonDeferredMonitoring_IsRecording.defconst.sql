ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
   ADD CONSTRAINT [DF_BvPersonDeferredMonitoring_IsRecording] 
   DEFAULT ((1))
   FOR [IsRecording]


