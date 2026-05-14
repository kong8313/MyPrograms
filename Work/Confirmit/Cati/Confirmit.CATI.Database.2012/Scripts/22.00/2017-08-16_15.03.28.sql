IF NOT EXISTS(SELECT 1 FROM [BvState] WHERE StateID = 1051)
begin
	WITH data( StateId, Name, Priority, StateGroupID, DA, FcdAction )
	AS
	(
		SELECT s.StateId, s.Name, s.Priority, sg.ID, s.DA, s.FcdAction FROM BvStateGroup sg CROSS JOIN 
		(
			SELECT 1051 as StateId, 'Survey script error' as Name, 1 as Priority, 0 as DA, 0 as FcdAction
		) as s
	)
	INSERT INTO [dbo].[BvState] (StateID, Name, Priority, StateGroupID, DA, FcdAction) SELECT StateId, Name, Priority, StateGroupID, DA, FcdAction FROM data
end;

GO
IF NOT EXISTS(SELECT 1 FROM [BvConfirmitStatus] WHERE StatusCode_BvFEE = 1051)
begin
	INSERT INTO [BvConfirmitStatus] ([StatusCode_Cnf],[StatusName_Cnf],[StatusCode_BvFEE]) VALUES( '1051', 'SurveyScriptError', 1051 )
end;

GO
IF NOT EXISTS(SELECT 1 FROM [BvThresholdITS] WHERE ITS = 1051)
begin
	INSERT INTO BvThresholdITS ( SurveySID, ITS ) VALUES (0, 1051)
end;

GO
PRINT N'Update complete.';


GO
