ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
   ADD CONSTRAINT [DF_BvPersonDeferredMonitoring_HasAudio] 
   DEFAULT ((0))
   FOR [HasAudio]


