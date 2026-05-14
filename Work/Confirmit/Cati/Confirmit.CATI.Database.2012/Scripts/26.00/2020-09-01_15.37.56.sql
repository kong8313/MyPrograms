CREATE TABLE #Temp(
	[ScheduleID] [int] NOT NULL,
	[XmlInUse] xml NOT NULL,
	[XmlUnderDev] xml NOT NULL,	
	)

insert into #Temp 
select [ScheduleID]
      ,CAST([XmlInUse] AS xml)
      ,CAST([XmlUnderDev] AS xml)
FROM [BvSchedule]

UPDATE [#Temp]
SET [XmlInUse].modify('insert attribute SampleUpdate {"true"} into (/Schedule/Rules/Rule[Description[text()="SampleUpdate"]])[1]'),
 [XmlUnderDev].modify('insert attribute SampleUpdate {"true"} into (/Schedule/Rules/Rule[Description[text()="SampleUpdate"]])[1]')
 
UPDATE [BvSchedule]
SET [XmlInUse] = CAST(t.[XmlInUse] as nvarchar(MAX)), 
    [XmlUnderDev] = CAST(t.[XmlUnderDev] as nvarchar(MAX))
FROM (SELECT * FROM #Temp) as t
WHERE [BvSchedule].[ScheduleID] = t.[ScheduleID]

GO
PRINT N'Update complete.';


GO
