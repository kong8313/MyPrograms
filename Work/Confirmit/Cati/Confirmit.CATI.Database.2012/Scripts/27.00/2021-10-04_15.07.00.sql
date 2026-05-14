PRINT N'Adding Synchronized Sample Status';

GO
BEGIN
    WITH data( StateId, Name, Priority, StateGroupID, DA, FcdAction )
    AS
    (
        SELECT s.StateId, s.Name, s.Priority, sg.ID, s.DA, s.FcdAction FROM BvStateGroup sg CROSS JOIN 
       	(
        	SELECT 1052 as StateId, 'Synchronized Sample' as Name, 1 as Priority, 0 as DA, 0 as FcdAction
        ) as s
    )
	INSERT INTO [dbo].[BvState] (StateID, Name, Priority, StateGroupID, DA, FcdAction) SELECT StateId, Name, Priority, StateGroupID, DA, FcdAction FROM data
END;

GO
INSERT INTO [BvConfirmitStatus] ([StatusCode_Cnf],[StatusName_Cnf],[StatusCode_BvFEE]) VALUES('1052', 'Synchronized Sample', 1052 )

GO
INSERT INTO BvThresholdITS ( SurveySID, ITS ) VALUES (0, 1052)

GO
PRINT N'Update complete.';

GO