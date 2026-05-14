GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Dialer.SettingsTemplatesJson', 'DialerSettingsTemplatesJson', 'Telephony', 'Dialer settings templates that are used to create dialer instances', 2, 0,
	'{"DialerSettingTemplates": [{"Name": "Sytel (Open Dialer API)","DialerType": "Generic","DialerConnectionParameters": [{"Id": "ServiceAddress","Name": "Service Address","Type": "System.String","Value": "https://localhost/DialerService/DialerService.svc"},{"Id": "ServiceEndpoint","Name": "Service Endpoint","Type": "System.String","Value": "DialerServiceEndpointHttps"},{"Id": "AuthorizationKeyForOutgoingRequests","Name": "Authorization Key For Outgoing Requests","Type": "System.String","Value": "0275E046-7FFF-495B-ACFE-09B439DB4902"}],"DialerConfigurationParameters": [{"Id": "SupportedPersonModes","Name": "Supported Person Modes","Type": "System.String","Value": "Manual,CampaignAssignment"},{"Id": "IsReloginNeededOnCampaignChange","Name": "Is Relogin Needed On Campaign Change","Type": "System.Boolean","Value": "True"},{"Id": "IsHangUpSupported","Name": "Is HangUp Supported","Type": "System.Boolean","Value": "True"},{"Id": "IsPauseOrResumePlaybackSupported","Name": "Is Pause Or Resume Playback Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsToggleAgentListensToPlaybackOrRespondentSupported","Name": "Is Toggle Agent Listens To Playback Or Respondent Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForLocalAgents","Name": "Is Dynamic Extension Number Allowed For Local Agents","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForRemoteAgents","Name": "Is Dynamic Extension Number Allowed For Remote Agents","Type": "System.Boolean","Value": "False"}],"DialerSurveyParameters": [{"Id": "AbandonRate","Name": "Abandon Call Target Percentage Rate","Value": "1.0","Type": "System.String","Visible": "True"},{"Id": "RNAtimeout","Name": "Ring No Answer Timeout in seconds","Type": "System.Int32","Value": "20","Visible": "True"},{"Id": "AnsMachineDetect","Name": "Answer Machine Detection","Value": "True","Type": "System.Boolean","Visible": "True"},{"Id": "CallProgressToneDetection","Name": "Call Progress Tone Detection","Value": "True","Type": "System.Boolean","Visible": "True"},{"Id": "AbandonMessageName","Name": "Abandon Message Name","Value": "ABANDON","Type": "System.String","Visible": "True"},{"Id": "CTIName","Name": "CTI Name is an optional parameter to specify the default CTI name","Type": "System.String","Visible": "True"},{"Id": "CLI","Name": "Calling Line Identity value can be \"allowed\", \"blocked\", or a number to display.","Value": "allowed","Type": "System.String","Visible": "True"},{"Id": "AnsMachineAudioMessageUrl","Name": "Answer Machine Audio Message URL","Value": "","Type": "System.String","Visible": "True"}]},{"Name": "BvTCI","DialerType": "BvTCI","DialerConnectionParameters": [{"Id": "HostNameOrIp","Name": "Host Name or IP Address","Type": "System.String","Value": ""},{"Id": "TcpPort","Name": "TCP Port","Type": "System.Int32","Value": ""},{"Id": "ServiceAddress","Name": "Service Address","Type": "System.String","Value": "http://localhost/TciDialerService/BvTciDialer.svc"},{"Id": "ServiceEndpoint","Name": "Service Endpoint","Type": "System.String","Value": "BvTciDialerServiceEndpoint"}],"DialerConfigurationParameters": [],"DialerSurveyParameters": [{"Id": "MaxRings","Name": "No reply timeout (secs)","Type": "System.Int32","Value": "45"},{"Id": "TelephoneNumberPrefix","Name": "Telephone number prefix","Type": "System.String","Value": ""}]},{"Name": "PRO-T-S","DialerType": "PROTS","DialerConnectionParameters": [{"Id": "HostNameOrIp","Name": "Host Name or IP Address","Type": "System.String","Value": ""},{"Id": "OutgoingTcpPort","Name": "Outgoing TCP Port","Type": "System.Int32","Value": "1810"},{"Id": "IncomingTcpPort","Name": "Incoming TCP Port","Type": "System.Int32","Value": "1811"},{"Id": "ServiceAddress","Name": "Service Address","Type": "System.String","Value": "http://localhost/ProtsDialerService/ProtsDialerService.svc"},{"Id": "ServiceEndpoint","Name": "Service Endpoint","Type": "System.String","Value": "PROTSDialerServiceEndpoint"},{"Id": "OperationsTimeout","Name": "Operations Timeout","Type": "System.Int32","Value": "7000"}],"DialerConfigurationParameters": [{"Id": "RootDirectoryForAudioRecords","Name": "Root Directory For Audio Records","Type": "System.String","Value": "C:\\DSM"}],"DialerSurveyParameters": [{"Id": "AbandonmentRate","Name": "Nuisance call abandonment rate","Type": "System.Int32","Value": "0"},{"Id": "MaxRings","Name": "No reply timeout (no. of rings)","Type": "System.Int32","Value": "5"},{"Id": "AnsMachineDetect","Name": "Enable answer phone detection","Type": "System.Boolean","Value": "False"},{"Id": "BillingCode","Name": "Billing Code","Type": "System.Int32","Value": "0"}]}]}'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END


GO
PRINT N'Altering [dbo].[BvDialers]...';


GO
ALTER TABLE [dbo].[BvDialers]
    ADD [DialerConfigurationTypeId] INT NULL;


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetNextAvailableDialer]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetNextAvailableDialer]';


GO
PRINT N'Update complete.';


GO
