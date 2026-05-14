CREATE TABLE [dbo].[BvActiveShiftTypeZone] (
    [Id] INT NOT NULL, --actually here stored shiftZoneId or min int value for None shift type
                       --or negative time zone id for any valid shift type.
    [SurveyId] INT NOT NULL, 
    [ShiftPriority] TINYINT CONSTRAINT [DF_BvActiveShiftTypeZone_ShiftPriority] DEFAULT (0) NOT NULL
);

