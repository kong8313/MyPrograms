ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
   ADD CONSTRAINT [DF_BvPersonDeferredMonitoring_IsComplete] 
   DEFAULT ((0))
   FOR [IsComplete]


