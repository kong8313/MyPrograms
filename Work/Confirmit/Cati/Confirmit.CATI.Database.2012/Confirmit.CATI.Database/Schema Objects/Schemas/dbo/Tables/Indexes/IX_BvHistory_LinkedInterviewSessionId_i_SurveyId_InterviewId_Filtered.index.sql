CREATE NONCLUSTERED INDEX IX_LinkedInterviewSessionId_i_SurveyId_InterviewId_Filtered
    ON [dbo].[BvHistory] (LinkedInterviewSessionId) INCLUDE (SurveyId, InterviewId)  
    WHERE LinkedInterviewSessionId IS NOT NULL 