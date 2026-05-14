DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
      SELECT 'Toggle.EnableExternalTransfer', 'Enable external transfer', 'Toggle', 'Enable external call transfer functionality', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END


GO

;WITH data( StateId, Name, Priority, StateGroupID, DA, FcdAction )
AS
(
    SELECT s.StateId, s.Name, s.Priority, sg.ID, s.DA, s.FcdAction FROM BvStateGroup sg CROSS JOIN 
    (
        SELECT 1011 as StateId, 'External Transfer' as Name, 1 as Priority, 0 as DA, 0 as FcdAction
    ) as s
)
INSERT INTO [dbo].[BvState] (StateID, Name, Priority, StateGroupID, DA, FcdAction) SELECT StateId, Name, Priority, StateGroupID, DA, FcdAction FROM data

PRINT N'Inserting into [dbo].[BvThresholdITS]...';

INSERT INTO BvThresholdITS ( SurveySID, ITS ) VALUES(0, 1011)

PRINT N'Inserting into [dbo].[BvConfirmitStatus]...';


GO
IF NOT EXISTS(SELECT 1 FROM [BvConfirmitStatus] WHERE StatusCode_BvFEE = 1011)
begin
	INSERT INTO [BvConfirmitStatus] ([StatusCode_Cnf],[StatusName_Cnf],[StatusCode_BvFEE]) VALUES( '1011', 'External Transfer', 1011 )
end;


GO
PRINT N'Creating [dbo].[BvExternalTransferTelephoneNumber]...';


GO
CREATE TABLE [dbo].[BvExternalTransferTelephoneNumber] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [TelephoneNumber] NVARCHAR (256) NULL,
    [Description]     NVARCHAR (256) NULL,
    CONSTRAINT [PK_BvExternalTransferTelephoneNumber] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvExternalTransferTelephoneNumberAssignment]...';


GO
CREATE TABLE [dbo].[BvExternalTransferTelephoneNumberAssignment] (
    [ExternalTransferTelephoneNumberId] INT NOT NULL,
    [SurveyId]                          INT NOT NULL
);


GO
PRINT N'Creating [dbo].[FK_BvExternalTransferTelephoneNumberAssignment_SurveyId]...';


GO
ALTER TABLE [dbo].[BvExternalTransferTelephoneNumberAssignment] WITH NOCHECK
    ADD CONSTRAINT [FK_BvExternalTransferTelephoneNumberAssignment_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_BvExternalTransferTelephoneNumberAssignment_ExternalTransferTelephoneNumberId]...';


GO
ALTER TABLE [dbo].[BvExternalTransferTelephoneNumberAssignment] WITH NOCHECK
    ADD CONSTRAINT [FK_BvExternalTransferTelephoneNumberAssignment_ExternalTransferTelephoneNumberId] FOREIGN KEY ([ExternalTransferTelephoneNumberId]) REFERENCES [dbo].[BvExternalTransferTelephoneNumber] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Checking existing data against newly created constraints';


GO



GO
ALTER TABLE [dbo].[BvExternalTransferTelephoneNumberAssignment] WITH CHECK CHECK CONSTRAINT [FK_BvExternalTransferTelephoneNumberAssignment_SurveyId];

ALTER TABLE [dbo].[BvExternalTransferTelephoneNumberAssignment] WITH CHECK CHECK CONSTRAINT [FK_BvExternalTransferTelephoneNumberAssignment_ExternalTransferTelephoneNumberId];


GO
PRINT N'Update complete.';


GO
