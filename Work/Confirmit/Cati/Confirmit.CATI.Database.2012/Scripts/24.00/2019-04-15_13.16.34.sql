PRINT N'Insert new ITS to tables';

;WITH data( StateId, Name, Priority, StateGroupID, DA, FcdAction )
AS
(
    SELECT s.StateId, s.Name, s.Priority, sg.ID, s.DA, s.FcdAction FROM BvStateGroup sg CROSS JOIN 
    (
        SELECT 1021 as StateId, 'Externally validated number' as Name, 1 as Priority, 0 as DA, 0 as FcdAction
    ) as s
)
INSERT INTO [dbo].[BvState] (StateID, Name, Priority, StateGroupID, DA, FcdAction) SELECT StateId, Name, Priority, StateGroupID, DA, FcdAction FROM data

INSERT INTO BvThresholdITS ( SurveySID, ITS ) VALUES(0, 1021)
INSERT INTO [BvConfirmitStatus] VALUES( '1021', 'Externally validated number', 1021 )

GO
PRINT N'Update complete.';


GO
