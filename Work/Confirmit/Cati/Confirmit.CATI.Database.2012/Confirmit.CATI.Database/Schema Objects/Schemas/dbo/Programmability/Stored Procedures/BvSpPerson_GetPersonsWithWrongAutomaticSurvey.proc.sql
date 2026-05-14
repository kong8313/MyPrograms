CREATE PROCEDURE [dbo].[BvSpPerson_GetPersonsWithWrongAutomaticSurvey]
	@SurveySID INT, 
	@PersonSID INT, 
	@CallCenterID INT
AS

IF @SurveySID IS NULL AND @PersonSID IS NULL AND @CallCenterID IS NULL
BEGIN
	SELECT 
		0 as [PersonSID],
		'' as [PersonName],
		0 as [AutomaticSurveyID]
	RETURN(0)
END

-- get Persons with wrong Automatic Survey
IF EXISTS ( SELECT SID FROM BvPerson WHERE SID = @PersonSID ) -- it's a person
	SELECT BvPerson.SID as "PersonSID", BvPerson.Name as "PersonName", BvPerson.AutomaticSurveyID as "AutomaticSurveyID"
	FROM BvPerson
	WHERE SID = @PersonSID AND AutomaticSurveyID = @SurveySID AND BvPerson.CallCenterID = @CallCenterID AND 
		NOT EXISTS (SELECT 1 -- no person or person's group survey assignments
					FROM BvPersonRel pr
					WHERE pr.PersonSID = @PersonSID AND pr.ObjectSID = @SurveySID AND pr.RoleID = 2 AND pr.Type = 2) AND 
		NOT EXISTS (SELECT 1 -- no person or person's group call assignments 
					FROM BvPersonRel pr
					INNER JOIN BvSvyScheduleRuntimeStatistics sc ON pr.ObjectSID = sc.ExplicitSID
					WHERE pr.PersonSID = @PersonSID AND pr.Type = 1 AND sc.SurveyId = @SurveySID);
ELSE -- it's a group
	SELECT BvPerson.SID as "PersonSID", BvPerson.Name as "PersonName", BvPerson.AutomaticSurveyID as "AutomaticSurveyID"
	FROM BvPerson
	WHERE AutomaticSurveyID = @SurveySID AND BvPerson.CallCenterID = @CallCenterID AND
		BvPerson.SID IN (SELECT PersonSID --look at all persons inside current group
							FROM BvPersonRel pr
							WHERE pr.ObjectSID = @PersonSID AND pr.RoleID = 2 AND pr.Type = 1) AND
		NOT EXISTS (SELECT 1 -- no person or person's group survey assignments
					FROM BvPersonRel pr
					WHERE pr.PersonSID = BvPerson.SID AND pr.ObjectSID = @SurveySID AND pr.RoleID = 2 AND pr.Type = 2) AND 
		NOT EXISTS (SELECT 1 -- no person or person's group call assignments 
					FROM BvPersonRel pr
					INNER JOIN BvSvyScheduleRuntimeStatistics sc ON pr.ObjectSID = sc.ExplicitSID
					WHERE pr.PersonSID = BvPerson.SID AND pr.Type = 1 AND sc.SurveyId = @SurveySID);

RETURN(0)
