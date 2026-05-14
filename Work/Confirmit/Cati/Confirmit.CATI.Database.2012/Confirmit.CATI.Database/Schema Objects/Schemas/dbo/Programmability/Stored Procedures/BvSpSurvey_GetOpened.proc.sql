CREATE PROCEDURE [dbo].[BvSpSurvey_GetOpened]
AS
 
 SELECT
     BvSurvey.SID
 FROM
     BvSurvey
    WHERE
        BvSurvey.State = 1

RETURN 0