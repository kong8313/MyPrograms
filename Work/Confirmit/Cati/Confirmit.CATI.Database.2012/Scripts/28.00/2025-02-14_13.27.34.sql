PRINT N'Add Dialer.RespondentVariablesToSend system setting';
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
    BEGIN
        ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
                  (
                      SELECT 'Dialer.RespondentVariablesToSend', 'Respondent variables to send to the dialer', 'Telephony', 'A comma-separated list of respondent variables that are sent to the dialer. If the survey does not contain some variables, they are ignored.', 2, 0, ''
                  )
         INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
         SELECT * FROM data
    END

GO
PRINT N'Update complete.';


GO
