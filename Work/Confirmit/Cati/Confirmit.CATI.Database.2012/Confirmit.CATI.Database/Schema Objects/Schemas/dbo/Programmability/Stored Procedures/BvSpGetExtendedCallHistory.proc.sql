CREATE PROCEDURE [dbo].[BvSpGetExtendedCallHistory]
@InterviewID     INTEGER,
@SurveyID        INTEGER,
@CallCenterID	 INTEGER
AS
SET NOCOUNT OFF


	SELECT 
		h.[Id],
		[FiredTime],
		[ApptID],
		ITS,
		ISNULL(BvState.[Name],'')  AS TransientState,
		h.ShiftTypeId,
		h.DialingMode,
		ISNULL(BvShiftType.[Name], '' ) AS ShiftType, 
		[CallState] ,
		h.[Priority],
		[TimeInShift],
		CASE h.[ExpireTime] WHEN '9999-01-01 00:00:00.000' THEN NULL ELSE h.[ExpireTime] END AS [ExpireTime],
		[ExplicitSID],
		[ExplicitType],
		ISNULL(pg.[Name], '') AS Resource,
		[CellId],
		[OperationId],
		[OperationType],
        ISNULL(cc.Name, '') AS CallCenterName,
		ISNULL(dt.Name, '') AS DialType

	FROM BvViewBothCallHistories h
	INNER JOIN BvSurvey s ON s.SID = h.SurveyId 
	LEFT JOIN BvCallCenter cc ON cc.ID = h.CallCenterID
	LEFT JOIN BvShiftZones ON BvShiftZones.[ID] = h.ShiftTypeID  
	LEFT JOIN BvShiftType ON  BvShiftType.ObjectID = BvShiftZones.ShiftTypeID  
	LEFT JOIN BvState ON BvState.StateID = h.ITS AND BvState.StateGroupID = s.StateGroupID
	LEFT JOIN BvViewPersonAndGroup pg ON pg.SID = h.ExplicitSID
	LEFT JOIN BvDialType dt ON h.DialTypeId = dt.Id

	WHERE 
		SurveyId = @SurveyID AND InterviewID = @InterviewID
	ORDER BY h.[Id]

RETURN 0
