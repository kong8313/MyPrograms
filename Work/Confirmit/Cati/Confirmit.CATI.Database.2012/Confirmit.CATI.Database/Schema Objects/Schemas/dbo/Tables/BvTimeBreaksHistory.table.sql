CREATE TABLE [dbo].[BvTimeBreaksHistory] 
(
    ID INT IDENTITY(1, 1) NOT NULL,
    StartTime DATETIME NOT NULL,
    InterviewerId INT NOT NULL,
    Duration INT NULL,
	CallCenterId INT NOT NULL, --seconds  
	SurveyId INT NOT NULL CONSTRAINT DF_BvTimeBreaksHistory_SurveyId DEFAULT(0),
	BreakTypeId INT NOT NULL,
	[DialTypeId] TINYINT NULL,
    [SessionId] INT NULL,
    CONSTRAINT PK_BvTimeBreaksHistory_Id PRIMARY KEY NONCLUSTERED (Id)
);


GO
EXEC sp_addextendedproperty @name='MS_Description',
                            @value ='Duration is measured in seconds',
                            @level0type = N'Schema', @level0name = 'dbo',
							@level1type = N'Table', @level1name = 'BvTimeBreaksHistory', 
							@level2type = N'Column',@level2name = 'Duration';
