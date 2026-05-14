CREATE PROCEDURE [dbo].[BvSpScheduleParam_Launch]
	@ScheduleID INT,
	@ParamBatchID INT
AS
	;MERGE BvScheduleParam as target
	USING(	SELECT @ScheduleId as ScheduleId, s.SID as SurveyId, p.ParamId, p.Name, p.Description, p.Type, p.Value  FROM BvScheduleParam p
				CROSS JOIN BvSurvey s
				WHERE p.ScheduleID = -@ParamBatchID AND s.ScheduleID = @ScheduleID AND s.State <> 2 ) as source
			ON target.ScheduleId = @ScheduleID AND target.SurveySID = source.SurveyId AND target.Name = source.Name AND target.Type = source.Type
			/*Insert new not exists parameters*/
			WHEN NOT MATCHED BY TARGET 
				THEN INSERT (ScheduleId, SurveySID, ParamId, Name, Description, Type, Value ) VALUES( source.ScheduleId, source.SurveyId, source.ParamId, source.Name, source.Description, source.Type, source.Value )
			/*Update parameter id and description for matched parameters. Note: we don't change/reset value to new default value*/
			WHEN MATCHED 
				THEN UPDATE 
					SET ParamID = source.ParamID,
					Description = source.Description
                        /*We delete all not matched records for specific schedule id. The condition also should delete all old default records(SurveySID=0) with default parameters*/
			WHEN NOT MATCHED BY SOURCE AND target.ScheduleId = @ScheduleID
				THEN DELETE
                        /*We delete all default parameters in previous condition, so we should move all prepared records(ScheduleId = -@ParamBatchID) with default parameters to default records*/
			WHEN NOT MATCHED BY SOURCE AND target.ScheduleId = -@ParamBatchID 
				THEN UPDATE SET ScheduleID = @ScheduleID;
                
RETURN (0)