IF OBJECT_ID('tempdb..#CheckItem' ) IS NOT NULL
BEGIN
	DROP PROCEDURE #CheckItem
END

GO

CREATE PROCEDURE #CheckItem
	@Description NVARCHAR(MAX),
	@ErrorMessage NVARCHAR(MAX),
	@CheckQuery NVARCHAR(MAX),
	@CorrectRowsCount INT
AS
	SET NOCOUNT ON
	PRINT @Description
	DECLARE @Query NVARCHAR(MAX) = 
	'SELECT * INTO #_internal_temp_data FROM ( ' + @CheckQuery + ' ) _internal_temp_table_name_
	IF @@ROWCOUNT <> ' + CAST( @CorrectRowsCount AS NVARCHAR(10)) + '
	BEGIN
		RAISERROR (''' + REPLACE( @ErrorMessage, '''', '''''' ) + ''', 16, 1 )
		SELECT * FROM #_internal_temp_data
	END'
	EXEC( @Query )
GO

EXEC #CheckItem
		@Description		= 'Check BvCallCenter.IsDefault',
		@ErrorMessage		= 'BvCallCenter table contains wrong count of default callcenters.',
		@CheckQuery			= 'SELECT * FROM BvCallCenter 
									WHERE IsDefault = 1',
		@CorrectRowsCount	= 1
		
EXEC #CheckItem
		@Description		= 'Check BvCallCenter.LocalTimeZoneId',
		@ErrorMessage		= 'BvCallCenter table contains wrong LocalTimeZoneId.',
		@CheckQuery			= 'SELECT c.* FROM BvCallCenter c 
									LEFT JOIN BvTimezone t ON c.LocalTimeZoneId = t.ID 
									WHERE t.ID IS NULL',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvPerson.CallCenterID for CATI persons',
		@ErrorMessage		= 'BvPerson table contains persons from CATI group with wrong CallCenterID.',
		@CheckQuery			= 'SELECT * FROM BvPerson 
									WHERE CallCenterID NOT IN ( SELECT ID FROM BvCallCenter ) AND 
									SID IN ( SELECT ObjectSID FROM BvMembership ms 
											INNER JOIN BvPersonGroup pg ON ms.ContainerSID = pg.SID WHERE pg.RoleID = 2 )',
		@CorrectRowsCount	= 0
		
EXEC #CheckItem
		@Description		= 'Check BvPerson.CallCenterID for CAPI persons',
		@ErrorMessage		= 'BvPerson table contains persons from CAPI group with wrong CallCenterID',
		@CheckQuery			= 'SELECT * FROM BvPerson 
									WHERE CallCenterID <> 0 AND 
									SID IN ( SELECT ObjectSID FROM BvMembership ms 
									INNER JOIN BvPersonGroup pg ON ms.ContainerSID = pg.SID WHERE pg.RoleID = 64 )',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvPersonDeferredMonitoring.CallCenterID',
		@ErrorMessage		= 'BvPersonDeferredMonitoring table contains records with wrong CallCenterID',
		@CheckQuery			= 'SELECT * FROM BvPersonDeferredMonitoring 
									WHERE CallCenterID NOT IN ( SELECT ID FROM BvCallCenter )',
		@CorrectRowsCount	= 0
		
EXEC #CheckItem
		@Description		= 'Check BvPersonOrGroupAssignmentOnSurvey.CallCenterID',
		@ErrorMessage		= 'BvPersonOrGroupAssignmentOnSurvey table contains records with wrong CallCenterID',
		@CheckQuery			= 'SELECT a.*, vp.Name, vp.IsGroup FROM BvPersonOrGroupAssignmentOnSurvey a
									LEFT JOIN BvViewPersonAndGroup vp ON a.PersonOrGroupId = vp.SID 
									WHERE a.CallCenterID NOT IN ( SELECT ID FROM BvCallCenter )',
		@CorrectRowsCount	= 0
		
EXEC #CheckItem
		@Description		= 'Check BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId',
		@ErrorMessage		= 'BvPersonOrGroupAssignmentOnSurvey table contains assigment on CAPI person',
		@CheckQuery			= 'SELECT * FROM BvPersonOrGroupAssignmentOnSurvey 
									WHERE PersonOrGroupId IN ( SELECT SID FROM BvPerson WHERE CallCenterID = 0 )',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvSupervisorAssignment.CallCenterID',
		@ErrorMessage		= 'BvSupervisorAssignment table contains records with wrong CallCenterID',
		@CheckQuery			= 'SELECT * FROM BvSupervisorAssignment 
									WHERE CallCenterID NOT IN ( SELECT ID FROM BvCallCenter )',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvSurveyAssignmentOnCallCenter.CallCenterID.',
		@ErrorMessage		= 'BvSurveyAssignmentOnCallCenter table contains records with wrong CallCenterID.',
		@CheckQuery			= 'SELECT s.Name, a.* FROM BvSurveyAssignmentOnCallCenter a 
									LEFT JOIN BvSurvey s ON a.SurveyId = s.SID 
									WHERE CallCenterID NOT IN ( SELECT ID FROM BvCallCenter )',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvSurveyAssignmentOnCallCenter.SurveyId.',
		@ErrorMessage		= 'BvSurveyAssignmentOnCallCenter table contains records with wrong SurveyId.',
		@CheckQuery			= 'SELECT a.*, cc.Name as CallCenterName FROM BvSurveyAssignmentOnCallCenter a 
									LEFT JOIN BvCallCenter cc ON a.CallCenterId = cc.Id 
									WHERE SurveyId NOT IN ( SELECT SID FROM BvSurvey )',
		@CorrectRowsCount	= 0
	
EXEC #CheckItem
		@Description		= 'Check BvTasks.CallCenterID',
		@ErrorMessage		= 'BvTasks table contains records with wrong CallCenterID',
		@CheckQuery			= 'SELECT t.*, p.Name as PersonName FROM BvTasks t 
									LEFT JOIN BvPerson p ON t.PersonSID = p.SID 
									WHERE t.CallCenterID NOT IN ( SELECT ID FROM BvCallCenter )',
		@CorrectRowsCount	= 0
		
EXEC #CheckItem
		@Description		= 'Check BvTimeBreaksHistory.CallCenterID',
		@ErrorMessage		= 'BvTimeBreaksHistory table contains records with wrong CallCenterID',
		@CheckQuery			= 'SELECT * FROM BvTimeBreaksHistory 
									WHERE CallCenterID NOT IN ( SELECT ID FROM BvCallCenter )',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvAsyncOperationQueue.CallCenterID',
		@ErrorMessage		= 'BvAsyncOperationQueue table contains records with wrong CallCenterID',
		@CheckQuery			= 'SELECT * FROM BvAsyncOperationQueue 
									WHERE CallCenterID NOT IN ( SELECT ID FROM BvCallCenter UNION SELECT 0 )',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvAsyncOperationQueue.SurveyId',
		@ErrorMessage		= 'BvAsyncOperationQueue table contains records with wrong SurveyId',
		@CheckQuery			= 'SELECT Id, Title, SurveySID, State FROM BvAsyncOperationQueue 
									WHERE SurveySId NOT IN ( SELECT SID FROM BvSurvey UNION SELECT 0 )',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvSurvey.SID',
		@ErrorMessage		= 'BvSurvey table contains records without assignment on any callcenter',
		@CheckQuery			= 'SELECT * FROM BvSurvey 
									WHERE SID NOT IN ( SELECT SurveyId FROM BvSurveyAssignmentOnCallCenter)',
		@CorrectRowsCount	= 0

IF @CheckLevel >= 1
BEGIN
	IF OBJECT_ID('tempdb..#Wrongpaths') IS NOT NULL
		DROP TABLE #Wrongpaths
		
	CREATE TABLE #Wrongpaths( CfDbSchemaPath NVARCHAR(MAX) )
	
	DECLARE @SchemaPath NVARCHAR(MAX) 
	DECLARE crSchemaPaths CURSOR FOR 
		SELECT CfDbSchemaPath FROM BvSurvey WHERE ReplicationStatus = 1
	OPEN crSchemaPaths;

	FETCH NEXT FROM crSchemaPaths INTO @SchemaPath
	WHILE @@FETCH_STATUS = 0
	BEGIN 
		BEGIN TRY 
		   DECLARE @Query NVARCHAR(MAX) = 'DECLARE @Cnt INT; SELECT @Cnt = COUNT(*) FROM ' + @SchemaPath + '.quotas'
		   EXEC(@Query)
		END TRY
		BEGIN CATCH
			INSERT INTO #Wrongpaths SELECT @SchemaPath
		END CATCH
	
		FETCH NEXT FROM crSchemaPaths INTO @SchemaPath
	END

	CLOSE crSchemaPaths
	DEALLOCATE crSchemaPaths
	
	EXEC #CheckItem
		@Description		= 'Check BvSurvey.CfDbSchemaPath',
		@ErrorMessage		= 'BvSurvey table contains problem confirmit schema path',
		@CheckQuery			= 'SELECT * FROM #Wrongpaths',
		@CorrectRowsCount	= 0
END

EXEC #CheckItem
		@Description		= 'Check BvPersonRel.PersonSID',
		@ErrorMessage		= 'BvPersonRel table contains records with wrong PersonSID',
		@CheckQuery			= 'SELECT * FROM BvPersonRel WHERE PersonSID NOT IN ( SELECT SID FROM dbo.BvViewPersonAndGroup)',
		@CorrectRowsCount	= 0
		
EXEC #CheckItem
		@Description		= 'Check BvPersonRel.ObjectSID',
		@ErrorMessage		= 'BvPersonRel table contains records with wrong ObjectSID',
		@CheckQuery			= 'SELECT * FROM BvPersonRel 
									WHERE ObjectSID NOT IN ( SELECT SID FROM dbo.BvViewPersonAndGroup ) 
										AND Type = 1 OR ObjectSID NOT IN ( SELECT SID FROM BvSurvey) AND Type = 2',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvPersonRel.PersonSID+ObjectSID(persons can be assigned only on surveys which have assignments on person''s call center )',
		@ErrorMessage		= 'BvPersonRel table contains wrong record',
		@CheckQuery			= 'SELECT r.*, p.Name as PersonName, s.Name as SurveyName  FROM BvPersonRel r INNER JOIN BvPerson p ON p.SID = r.PersonSID 
									   INNER JOIN BvSurvey s ON r.ObjectSID = s.SID 
									   WHERE Type = 2 AND p.CallCenterID NOT IN 
									   ( 
											SELECT CallCenterID FROM BvSurveyAssignmentOnCallCenter sa WHERE sa.SurveyId = s.SID
										)',
		@CorrectRowsCount	= 0

EXEC #CheckItem
		@Description		= 'Check BvSvySchedule.ExplicitSID',
		@ErrorMessage		= 'BvSvySchedule table contains calls which is assigned to CAPI persons',
		@CheckQuery			= 'SELECT p.Name as PersonName, s.* FROM BvSvySchedule s 
									INNER JOIN BvPerson p ON s.ExplicitSID = p.SID 
									WHERE p.CallCenterID = 0',
		@CorrectRowsCount	= 0
