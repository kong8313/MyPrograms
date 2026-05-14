CREATE TABLE [dbo].[BvPersonOrGroupAssignmentOnSurvey] (
    [Id]                INT IDENTITY (1, 1) NOT NULL,
	[CallCenterID]		INT NOT NULL,
    [PersonOrGroupId]   INT NOT NULL,
    [SurveyId] INT NOT NULL
);

