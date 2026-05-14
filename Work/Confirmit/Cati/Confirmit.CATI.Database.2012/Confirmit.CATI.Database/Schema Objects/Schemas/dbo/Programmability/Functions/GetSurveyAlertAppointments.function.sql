CREATE FUNCTION GetSurveyAlertAppointments
	(
		@SurveySID INT,
		@Top INT,
		@Amber INT,
		@Red INT,
		@Now DATETIME
	)
	RETURNS TABLE AS RETURN
	(
		WITH a as
		(
			SELECT *, 2 as AlertStatus FROM BvAppointment a
			WHERE a.SurveySID = @SurveySID AND  a.State = 1 /*with call*/ AND a.Time < DATEADD( second, -@Red, @NOW )
			UNION ALL 
			SELECT *, 1 as AlertStatus FROM BvAppointment a
			WHERE a.SurveySID = @SurveySID AND  a.State = 1 /*with call*/ AND a.Time BETWEEN  @Now AND DATEADD( second, -@Amber, @NOW )
		)
		SELECT TOP(100) * FROM a ORDER BY a.Time
		

	) 