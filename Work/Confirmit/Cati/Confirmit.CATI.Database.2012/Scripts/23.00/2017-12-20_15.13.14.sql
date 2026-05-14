GO
PRINT N'Creating [dbo].[BvSpPerson_GetSurveys]...';


GO
CREATE procedure [dbo].[BvSpPerson_GetSurveys]
@PersonSID  int,
@ProjectIds BvStringArrayType READONLY
as
	DECLARE @CallCenterId INT = ( SELECT CallCenterId FROM BvPerson WHERE SID = @PersonSID )
	IF( ( SELECT COUNT(*) FROM @ProjectIds ) <> 0 )
	BEGIN
		SELECT s.* FROM BvFnSurvey_GetByCallCenterId(@CallCenterId) as s 
			INNER JOIN @ProjectIds p ON s.Name = p.Value
			INNER join BvPersonRel r on r.ObjectSID = s.SID and r.Type = 2 
										and r.RoleID = 2 and r.PersonSID = @PersonSID
			WHERE s.State = 1
	END
	ELSE
	BEGIN
		SELECT s.* FROM BvFnSurvey_GetByCallCenterId(@CallCenterId) as s 
			INNER join BvPersonRel r on r.ObjectSID = s.SID and r.Type = 2 
										and r.RoleID = 2 and r.PersonSID = @PersonSID
			WHERE s.State = 1
	END
return (0)
GO
PRINT N'Update complete.';


GO
