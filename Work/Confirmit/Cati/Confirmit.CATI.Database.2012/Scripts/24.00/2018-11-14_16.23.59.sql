PRINT N'Add Console.EnableInternalCallTransferToolbarButton and Console.EnableExternalCallTransferToolbarButton settings';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
    SELECT 'Console.EnableInternalCallTransferToolbarButton', 'Console enable ability to do internal call transfer', 'Interviewing', 'Is Interviewer Console able to do internal call transfer', 3, 0, 'True'
    UNION ALL
    SELECT 'Console.EnableExternalCallTransferToolbarButton', 'Console enable ability to do external call transfer', 'Interviewing', 'Is Interviewer Console able to do external call transfer', 3, 0, 'True'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END

GO
PRINT N'Update complete.';