;WITH data( StateId, Name, Priority, StateGroupID, DA, FcdAction )
AS
(
    SELECT s.StateId, s.Name, s.Priority, sg.ID, s.DA, s.FcdAction FROM BvStateGroup sg CROSS JOIN 
    (
        SELECT 1010 as StateId, 'Internal Transfer' as Name, 2000 as Priority, 0 as DA, 0 as FcdAction
    ) as s
)
INSERT INTO [dbo].[BvState] (StateID, Name, Priority, StateGroupID, DA, FcdAction) SELECT StateId, Name, Priority, StateGroupID, DA, FcdAction FROM data


GO
INSERT INTO BvThresholdITS ( SurveySID, ITS ) 
    SELECT 0, StateId FROM
    (
        SELECT 1010 as StateId
    ) as s


GO
IF NOT EXISTS(SELECT 1 FROM [BvConfirmitStatus] WHERE StatusCode_BvFEE = 1010)
begin
	INSERT INTO [BvConfirmitStatus] ([StatusCode_Cnf],[StatusName_Cnf],[StatusCode_BvFEE]) VALUES( '1010', 'Internal Transfer', 1010 )
end;


GO
PRINT N'Update complete.';


GO
