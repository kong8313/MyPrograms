﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4206
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UpdateDatabase {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class Configuration : global::System.Configuration.ApplicationSettingsBase {
        
        private static Configuration defaultInstance = ((Configuration)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Configuration())));
        
        public static Configuration Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ConfirmitCATIUpgrade")]
        public string SourceDatabaseName {
            get {
                return ((string)(this["SourceDatabaseName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ConfirmitCATIUpgrade.bak")]
        public string SourceDatabaseBackupFilePath {
            get {
                return ((string)(this["SourceDatabaseBackupFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("sa")]
        public string SqlUserName {
            get {
                return ((string)(this["SqlUserName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("firm")]
        public string SqlPassword {
            get {
                return ((string)(this["SqlPassword"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>bvcellflag;cellflags</string>
  <string>bvcell;cells</string>
  <string>bvinterview;interviews</string>
  <string>bvhistoryraw;historyraw</string>
  <string>bvkey;key</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection SplitTablesList {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["SplitTablesList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>%PROJPATH%\\DLL\\BvSqlCallQueue.dll</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection BackupContent_ClrAssembyFiles {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["BackupContent_ClrAssembyFiles"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ProductionDatabaseBackupFilePath {
            get {
                return ((string)(this["ProductionDatabaseBackupFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsTestModeEnabled {
            get {
                return ((bool)(this["IsTestModeEnabled"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("confirmit template")]
        public string TemplateSurveyName {
            get {
                return ((string)(this["TemplateSurveyName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("(PtBvDb\\d+$)|(ConfirmitCATI$)")]
        public string DatabaseNamePattern {
            get {
                return ((string)(this["DatabaseNamePattern"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ExecuteUpdateScript {
            get {
                return ((bool)(this["ExecuteUpdateScript"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Updates\\DataUpdate.sql")]
        public string ExternalUpdateScriptPath {
            get {
                return ((string)(this["ExternalUpdateScriptPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".")]
        public string SqlServerName {
            get {
                return ((string)(this["SqlServerName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UpdateDefaultDatabaseBackup {
            get {
                return ((bool)(this["UpdateDefaultDatabaseBackup"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ConfirmitCATI")]
        public string DefaultDatabaseName {
            get {
                return ((string)(this["DefaultDatabaseName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\backupConfirmitCATI.bak ")]
        public string DefaultDatabaseBackupFilePath {
            get {
                return ((string)(this["DefaultDatabaseBackupFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DatabaseBackupsDirectory {
            get {
                return ((string)(this["DatabaseBackupsDirectory"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>BvSpCall_Activate</string>\r\n  <string>BvSpCall_ChangeShiftType</string>\r\n " +
            " <string>BvSpCall_MoveToITS</string>\r\n  <string>BvSpCallHistory_List</string>\r\n " +
            " <string>BvSpCell_Insert</string>\r\n  <string>BvSpChildQuestionnaires</string>\r\n " +
            " <string>BvSpChildQuestionnaires_Delete</string>\r\n  <string>BvSpChildQuestionnai" +
            "res_DeleteData</string>\r\n  <string>BvSpDeleteBatches</string>\r\n  <string>BvSpGet" +
            "StatesFromBatch</string>\r\n  <string>BvSpGetSurveyTasks</string>\r\n  <string>BvSpH" +
            "istory_Insert</string>\r\n  <string>BvSpInterview_CheckSerialField</string>\r\n  <st" +
            "ring>BvSpInterview_Get</string>\r\n  <string>BvSpInterview_Insert</string>\r\n  <str" +
            "ing>BvSpInterview_IsExists</string>\r\n  <string>BvSpInterview_List</string>\r\n  <s" +
            "tring>BvSpInterview_Start</string>\r\n  <string>BvSpInterview_Update</string>\r\n  <" +
            "string>BvSpKey_Delete</string>\r\n  <string>BvSpKey_GetByRID</string>\r\n  <string>B" +
            "vSpKey_InsertUpdate</string>\r\n  <string>BvSpReportSSS</string>\r\n  <string>BvSpRe" +
            "portSSSChart</string>\r\n  <string>BvSpRptQtPgrsByPers</string>\r\n  <string>BvSpRpt" +
            "QuotaDynamics</string>\r\n  <string>BvSpRptSampleDisposition</string>\r\n  <string>B" +
            "vSpRptSSSPrgssChrtByPers</string>\r\n  <string>BvSpSample_Abandon</string>\r\n  <str" +
            "ing>BvSpSample_Finalize</string>\r\n  <string>BvSpSummaryReport_InterviewStatistic" +
            "s</string>\r\n  <string>BvSpSurvey_CancelInterview</string>\r\n  <string>BvSpSurvey_" +
            "Delete</string>\r\n  <string>BvSpSurvey_DeleteInterview</string>\r\n  <string>BvSpSu" +
            "rvey_GetTotalMinutes</string>\r\n  <string>BvSpSurvey_QreDistribution</string>\r\n  " +
            "<string>BvSpSurvey_Update</string>\r\n  <string>BvSpSurveyModifyStateGroup</string" +
            ">\r\n  <string>BvSpSvySch_Update</string>\r\n  <string>BvSpSvySch_Insert</string>\r\n " +
            " <string>BvSpSurveyState_Update</string>\r\n  <string>BvSpGetCachedCallsForPredict" +
            "iveSurvey</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection SplitProceduresList {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["SplitProceduresList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>%PROJPATH%\\UNITS\\bv7\\SQL\\CreateDB.sql</string>\r\n  <string>%PROJPATH%\\UNITS" +
            "\\bv7\\SQL\\TABLES\\bvcellflag.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\TABL" +
            "ES\\bvcell.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\TABLES\\bvinterview.sq" +
            "l</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\TABLES\\bvhistoryraw.sql</string>\r" +
            "\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\TABLES\\bvkey.sql</string>\r\n  <string>%PROJPA" +
            "TH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpCall_Activate.sql</string>\r\n  <string>%PROJPATH" +
            "%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpCall_ChangeShiftType.sql</string>\r\n  <string>%PRO" +
            "JPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpCall_MoveToITS.sql</string>\r\n  <string>%PROJ" +
            "PATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpCallHistory_List.sql</string>\r\n  <string>%PRO" +
            "JPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpCell_Insert.sql</string>\r\n  <string>%PROJPAT" +
            "H%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpChildQuestionnaires.sql</string>\r\n  <string>%PRO" +
            "JPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpChildQuestionnaires_Delete.sql</string>\r\n  <" +
            "string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpChildQuestionnaires_DeleteData.sq" +
            "l</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpDeleteBatches.sql<" +
            "/string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpGetStatesFromBatch.s" +
            "ql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpGetSurveyTasks.sq" +
            "l</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpHistory_Insert.sql" +
            "</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpInterview_CheckSeri" +
            "alField.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpIntervie" +
            "w_Get.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpInterview_" +
            "Insert.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpInterview" +
            "_IsExists.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpInterv" +
            "iew_List.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpIntervi" +
            "ew_Start.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpIntervi" +
            "ew_Update.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpKey_De" +
            "lete.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpKey_GetByRI" +
            "D.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpKey_InsertUpda" +
            "te.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpReportSSS.sql" +
            "</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpReportSSSChart.sql<" +
            "/string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpRptQtPgrsByPers.sql<" +
            "/string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpRptQuotaDynamics.sql" +
            "</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpRptSampleDispositio" +
            "n.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpRptSSSPrgssChr" +
            "tByPers.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpSample_A" +
            "bandon.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpSample_Fi" +
            "nalize.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpSummaryRe" +
            "port_InterviewStatistics.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCED" +
            "URES\\BvSpSurvey_CancelInterview.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL" +
            "\\PROCEDURES\\BvSpSurvey_Delete.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\P" +
            "ROCEDURES\\BvSpSurvey_DeleteInterview.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv" +
            "7\\SQL\\PROCEDURES\\BvSpSurvey_GetTotalMinutes.sql</string>\r\n  <string>%PROJPATH%\\U" +
            "NITS\\bv7\\SQL\\PROCEDURES\\BvSpSurvey_QreDistribution.sql</string>\r\n  <string>%PROJ" +
            "PATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpSurvey_Update.sql</string>\r\n  <string>%PROJPA" +
            "TH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpSurveyModifyStateGroup.sql</string>\r\n  <string>" +
            "%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvTrInterview_InterviewsDelete.sql</string>\r" +
            "\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvTrInterview_InterviewsInsert.sq" +
            "l</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvTrInterview_Intervie" +
            "wsUpdate.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpSvySch_" +
            "Update.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpSvysch_In" +
            "sert.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpSurveyState" +
            "_Update.sql</string>\r\n  <string>%PROJPATH%\\UNITS\\bv7\\SQL\\PROCEDURES\\BvSpGetCache" +
            "dCallsForPredictiveSurvey.sql</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection BackupContent_SqlScriptFiles {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["BackupContent_SqlScriptFiles"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ExternalPreUpdateScriptPath {
            get {
                return ((string)(this["ExternalPreUpdateScriptPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ExternalPostUpdateScriptPath {
            get {
                return ((string)(this["ExternalPostUpdateScriptPath"]));
            }
        }
    }
}
