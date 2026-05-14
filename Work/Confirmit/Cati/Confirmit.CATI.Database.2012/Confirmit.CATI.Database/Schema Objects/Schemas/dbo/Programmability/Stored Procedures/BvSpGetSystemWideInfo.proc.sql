CREATE PROCEDURE BvSpGetSystemWideInfo
   @BatchID INT,
   @CallCenterID INT
AS  
        --1. InterviewersLoggedCount thresholds
        DECLARE @AmberOfInterviewersLoggedCountSWI INT
        DECLARE @RedOfInterviewersLoggedCountSWI INT
        SELECT @AmberOfInterviewersLoggedCountSWI = Amber, @RedOfInterviewersLoggedCountSWI = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 12/*SystemWideInfo.LoggedInterviewersCount alert*/

        --2. OpenSurveysCount thresholds
        DECLARE @AmberOfOpenSurveysCount INT
        DECLARE @RedOfOpenSurveysCount INT
        SELECT @AmberOfOpenSurveysCount = Amber, @RedOfOpenSurveysCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 13/*SystemWideInfo.OpenSurveysCount alert*/

        --3. CallsCount thresholds
        DECLARE @AmberOfCallsCount INT
        DECLARE @RedOfCallsCount INT
        SELECT @AmberOfCallsCount = Amber, @RedOfCallsCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 14/*SystemWideInfo.CallsCount alert*/


        DECLARE @count INT;
		DECLARE @countOpenSurveys INT
        DECLARE @totalInterviewers INT
        DECLARE @loggedinterviewers INT   
        DECLARE @loggedIvrAgents INT
		DECLARE @totalInterviewersWorkedToday INT

        SELECT @count = ISNULL(SUM(StrikeRate),0)
        FROM BvAggregateSurveyAlertStatus asas
        INNER JOIN BvSurvey s ON (s.SID = asas.SID)
        INNER JOIN BvTransferArrays ta ON (ta.BatchID = @BatchID AND
                                           ta.ItemID = s.SID)
                                                  
        SELECT @totalInterviewers = COUNT(DISTINCT Person.SID) FROM BvFnPerson_Get(@CallCenterID)  Person INNER JOIN 
					 BvMembership ON Person.SID = ObjectSID INNER JOIN 
					 BvPersonGroup ON BvMembership.ContainerSID = BvPersonGroup.SID
        
		SELECT @totalInterviewersWorkedToday = COUNT(DISTINCT BvInterviewerPerformance.InterviewerId) FROM BvInterviewerPerformance;

		SELECT @loggedinterviewers = ISNULL( SUM( CASE WHEN [Type] = 0 THEN 1 ELSE 0 END ), 0 ),
		@loggedIvrAgents = ISNULL( SUM( CASE WHEN [Type] = 1 THEN 1 ELSE 0 END ), 0 ) 
		FROM BvTasks t INNER JOIN BvPerson p ON t.PersonSID = p.SID

        SELECT @countOpenSurveys = COUNT(*)
        FROM BvSurvey s
        INNER JOIN BvTransferArrays ta ON (ta.BatchID = @BatchID AND
                                           ta.ItemID = s.SID)
        WHERE s.State = 1 /*open*/
               
        SELECT         
			@totalInterviewers as TotalInterviewersCount,
			@loggedinterviewers as LoggedInterviewersCount,
			@loggedIvrAgents as LoggedIvrAgentsCount,
            @countOpenSurveys as OpenSurveysCount,
			@totalInterviewersWorkedToday as TotalInterviewersWorkedTodayCount,
            @count as CallsCount,
            dbo.udf_AlertStatus_INT(@loggedinterviewers, @AmberOfInterviewersLoggedCountSWI, @RedOfInterviewersLoggedCountSWI) as AlertStatusOfLoggedInterviewersCount,
            dbo.udf_AlertStatus_INT(@countOpenSurveys, @AmberOfOpenSurveysCount, @RedOfOpenSurveysCount) as AlertStatusOfOpenSurveysCount,
            dbo.udf_AlertStatus_INT(@count, @AmberOfCallsCount, @RedOfCallsCount) as AlertStatusOfCallsCount
