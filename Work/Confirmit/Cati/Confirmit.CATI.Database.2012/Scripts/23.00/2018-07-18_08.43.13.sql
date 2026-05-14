DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
    SELECT 'Ivr.TermChar', 'TerminatingCharacter', 'Ivr', 'The terminating DTMF character for DTMF input recognition', 2, 0, '#'
    UNION ALL
    SELECT 'Ivr.RecordType', 'RecordType', 'Ivr', 'The media format of the resulting recording', 2, 0, 'audio/x-wav'
    UNION ALL
    SELECT 'Ivr.Beep', 'UseBeep', 'Ivr', 'If true, a tone is emitted just prior to recording', 3, 0, 'True'
    UNION ALL
    SELECT 'Ivr.MaxTime', 'MaxTime', 'Ivr', 'The maximum duration to record', 1, 0, '20'
    UNION ALL
    SELECT 'Ivr.FinalSilence', 'FinalSilence', 'Ivr', 'The interval of silence that indicates end of speech', 1, 0, '10'
    UNION ALL
    SELECT 'Ivr.DtmfTerm', 'UseDtmfTermination', 'Ivr', 'If true, any DTMF keypress not matched by an active grammar will be treated as a match of an active (anonymous) local DTMF grammar', 3, 0, 'True'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END


GO
PRINT N'Creating [dbo].[BvIvrSettings]...';


GO
CREATE TABLE [dbo].[BvIvrSettings] (
    [LanguageId]             INT            NOT NULL,
    [LanguageDescription]    NVARCHAR (255) NOT NULL,
    [WrongInputAudioUrl]     NVARCHAR (255) NOT NULL,
    [WrongInputText]         NVARCHAR (255) NOT NULL,
    [WrongInputExitAudioUrl] NVARCHAR (255) NOT NULL,
    [WrongInputExitText]     NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_BvIvrSettings_LanguageId] PRIMARY KEY CLUSTERED ([LanguageId] ASC)
);


PRINT N'Update complete.';
GO