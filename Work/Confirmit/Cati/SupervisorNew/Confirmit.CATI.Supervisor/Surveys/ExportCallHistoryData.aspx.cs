using System;
using System.Collections.Generic;
using System.Text;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.Export;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Classes;
using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "IDS", SeparatorCharacter = ",")]
    public partial class ExportCallHistoryData : BaseForm
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly ICallHistoryDataProvider _dataProvider = ServiceLocator.Resolve<ICallHistoryDataProvider>();
        private readonly IInputParameterValidator _inputParameterValidator = ServiceLocator.Resolve<IInputParameterValidator>();
        private readonly ISurveyRepository _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
        private readonly IUserSurveyListRepository _userSurveyListRepository = ServiceLocator.Resolve<IUserSurveyListRepository>();

        private string m_SurveyIDs;
        private const string ExportFileName = "CallHistoryData.txt";
        private const string PackageFileName = "CallHistoryData.zip";
        
        /// <summary>
        /// Selected surveys ids
        /// </summary>
        protected string SurveyIDs
        {
            get
            {
                if (m_SurveyIDs == null)
                {
                    m_SurveyIDs = (String)ViewState["IDS"];
                }

                return m_SurveyIDs;
            }
        }

        private List<int> m_IDs;

        protected List<int> IDs
        {
            //TODO: should always return some List<int>
            get
            {
                if (m_IDs == null)
                {
                    string[] ids = SurveyIDs.Split(',');
                    m_IDs = ids.Select(x => Int32.Parse(x)).ToList();
                }
                return m_IDs;
            }
        }

        /// <summary>
        /// Start time for selection data to export.
        /// </summary>
        private DateTime? StartTime
        {
            get
            {
                return cbxSelectAll.Checked 
                    ? (DateTime?) null
                    : _timezoneProvider.ConvertToUtc(dteStartTime.DateTimeValue);
            }
        }

        /// <summary>
        /// End time for selection data to export.
        /// </summary>
        private DateTime? EndTime
        {
            get
            {
                return cbxSelectAll.Checked
                    ? (DateTime?)null
                    : _timezoneProvider.ConvertToUtc(dteEndTime.DateTimeValue);
            }
        }

        private bool IncludeBreakTimes
        {
            get
            {
                return cbxIncludeBreaks.Checked;
            }
        }

        private bool IncludeLoginLogoutInfo
        {
            get
            {
                return cbxIncludeLoginLogout.Checked;
            }
        }

        private bool IncludeColumnHeadings
        {
            get
            {
                return cbxIncludeColumnHeadings.Checked;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ShowPostbackProcessingAnimation = false;

            if (User.IsCatiAdministratorOrPros || User.IsCatiProjectAdministrator) 
                cbxIncludeDataFromAllSurveys.Visible = true;

            if (!IsPostBack)
            {
                var ids = Request.Params["IDS"];

                if(ids == null)
                    throw new InternalErrorException("At least one survey should be selected");

                ViewState["IDS"] = ids;
                string callHistoryHelp = Strings.ExportCallHistoryDataHelp;
                if (BackendInstance.Current.HasCallCentersAddon)
                {
                    callHistoryHelp += Strings.ExportCallHistoryCallCenterHelp;
                }

                divExportCallHistoryDataHelp.InnerHtml = callHistoryHelp;

                InitStartEndTimes();

                foreach (var surveyId in IDs)
                {
                    _userSurveyListRepository.Insert(UserSurveyListType.Recent, surveyId);
                }
            }

            dteStartTime.Enabled = !cbxSelectAll.Checked;
            dteEndTime.Enabled = !cbxSelectAll.Checked;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterClientScripts();
            var settings = ServiceLocator.Resolve<ISystemSettings>().Reports;
            limitsHint.Text = string.Format(Strings.ExportCallHistoryDataLimitsHintText,
                                            settings.CallHistoryReportCallHistoryRowsLimit,
                                            settings.CallHistoryReportInterviewerBreaksRowsLimit);
        }

        protected void ExportClick(object sender, EventArgs e)
        {
            try
            {
                var evt = new CallHistoryExportEvent();

                var cleaner = new DelimitedStringCleaner();
                var surveyIds = SurveyIDs;

                var includeVariables = cbxIncludeReplicatedVariables.Checked;
                var replicatedVariablesFilter = includeVariables ? ReplicatedVariablesTextBox.Text : string.Empty;

                if (includeVariables)
                {
                    if (ValidateVariablesDoNotAllowEmptyVariables(replicatedVariablesFilter) == false)
                    {
                        AddUserMessage(Strings.ReplicatedVariablesInvalidFormatMessage);
                        return;
                    }

                    _dataProvider.IncludeReplicatedVariables = true;
                    ReplicatedVariablesTextBox.Text = cleaner.CleanString(replicatedVariablesFilter);
                }
                    
                if ((User.IsCatiAdministratorOrPros || User.IsCatiProjectAdministrator) && cbxIncludeDataFromAllSurveys.Checked)
                    surveyIds = null;

                var variables = cleaner.ParseString(replicatedVariablesFilter).ToArray();

                var surveyIdsDelimited = new DelimitedStringCleaner().ParseString(surveyIds);

                var projectIds = surveyIdsDelimited.Select(
                    sid => _surveyRepository.GetById(int.Parse(sid)).ProjectId).JoinInString(",");

                evt.Details.ProjectIds = projectIds;
                evt.Details.StartTime = StartTime;
                evt.Details.EndTime = EndTime;
                evt.Details.ReplicatedVariables = string.Join(",", variables);

                evt.Details.IncludeVariables = includeVariables;
                evt.Details.IncludeBreakTime = IncludeBreakTimes;
                evt.Details.IncludeLoginLogoutInfo = IncludeLoginLogoutInfo;
                evt.Details.IncludeColumnHeadings = IncludeColumnHeadings;

                var callHistoryList = _dataProvider.GetCallHistoryData(surveyIds, StartTime, EndTime, variables, IncludeBreakTimes, IncludeLoginLogoutInfo);
                
                // we use tab separated values format to export call history data
                var tsvString = DsvManager.ExportToDsv(callHistoryList, "\t", _dataProvider.PrepareForExport);

                if (includeVariables || IncludeColumnHeadings)
                {
                    tsvString = _dataProvider.GetHeader(replicatedVariablesFilter) + tsvString;
                }

                var packageFilePath = String.Empty;

                try
                {
                    // we pack TSV data file before sending it to client
                    packageFilePath = new Packaging().CreatePackage(ExportFileName, tsvString);
                }
                catch (Exception ex)
                {
                    ExceptionTraceHelper.TraceException(ex);
                    throw new Exception("Error on creating export file, contact the administrator");
                }

                FileToClientSender.SendFileContent(packageFilePath, PackageFileName, true);

                evt.Save();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void InitStartEndTimes()
        {
            var lt = _timezoneProvider.GetCurrentLocalTime();
            dteStartTime.DateTimeValue = new DateTime(lt.Year, lt.Month, lt.Day, 0, 0, 0);
            dteEndTime.DateTimeValue = new DateTime(lt.Year, lt.Month, lt.Day, 23, 59, 59);
        }

        private void RegisterClientScripts()
        {
            var script = new StringBuilder();

            script.Append("     function cbxSelectAll_checked(isChecked)");
            script.Append("     {");
            script.AppendFormat("{0}.setEnabled(isChecked);", dteStartTime.ClientControllerName);
            script.AppendFormat("{0}.setEnabled(isChecked);", dteEndTime.ClientControllerName);
            script.Append("     }");

            ClientScript.RegisterClientScriptBlock(
                Page.GetType(),
                "ExportCallHistoryData",
                script.ToString(),
                true);

            cbxSelectAll.Attributes.Add("onclick", "cbxSelectAll_checked(!this.checked)");
        }

        protected bool ValidateVariablesDoNotAllowEmptyVariables(string source)
        {
            var isValid = (string.IsNullOrEmpty(source) == false);

            try
            {
                var variables = new DelimitedStringCleaner().ParseString(source);

                if (variables.Any(email => _inputParameterValidator.IsValidQuestionId(email) == false))
                {
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                Context.AddError(ex);
            }

            return isValid;
        }
    }
}
