using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using ConfirmitDialerInterface;
using Tabs = Confirmit.CATI.Core.AuthoringService.Tabs;
using System.Linq;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.Paging;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Timezone;

namespace Confirmit.CATI.Supervisor.Surveys.Controls
{
    public partial class SrvPropertiesGeneral : SrvInfoChild
    {
        private readonly IQuotaClusteringConfigurationService _quotaClusteringConfigurationService = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();
        private readonly IQuotaBalancingService _quotaBalancingService = ServiceLocator.Resolve<IQuotaBalancingService>();
        private readonly ICallQueueService _callQueueService = ServiceLocator.Resolve<ICallQueueService>();
        private readonly ISystemSettings _systemSettings = ServiceLocator.Resolve<ISystemSettings>();
        protected readonly ICachedLocalTimezoneManager LocalTimezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();

        /// <summary>
        /// Gets/sets flag indicated was this page rendered or not
        /// By default returns true.
        /// </summary>
        /// <remarks>
        /// UltraWebTab control with enabled AutoPostBack doesn't call Render method for invisible tab 
        /// </remarks>
        public bool IsFirstTime
        {
            get
            {
                return (bool)(ViewState["IsFirstTime"] ?? true);
            }
            set
            {
                ViewState["IsFirstTime"] = value;
            }
        }

        public override void Save()
        {
            if (string.IsNullOrWhiteSpace(txtTarget.Text))
            {
                Survey.Target = null;
            }
            else
            {
                int target;

                if (int.TryParse(txtTarget.Text, out target) && target > 0)
                {
                    Survey.Target = target;
                }
                else
                {
                    Page.AddUserMessage(Strings.IncorrectTargetValue);
                    return;
                }
            }

            int scheduleId = Convert.ToInt32(ddlSchedulingScript.SelectedValue);
            int stateGroupId = Convert.ToInt32(lbITSDefGroup.SelectedValue);
            var callDeliveryMode = (CallDeliveryMode)Convert.ToInt32(ddlCallDeliveryMode.SelectedValue);
            var schedulingMode = (SurveySchedulingMode)Convert.ToInt32(ddlCallGroupsMode.SelectedValue);

            var evt = new UpdateSurveyEvent(Survey.SID, Survey.Name, stateGroupId, scheduleId);

            if (scheduleId != Survey.ScheduleID)
            {
                ReassignShiftTypesIfNeeded(scheduleId);
                OnUpdateAssignedScriptEvent();
            }

            Survey.ScheduleID = scheduleId;
            Survey.StateGroupID = stateGroupId;
            Survey.SurveySchedulingMode = (short)schedulingMode;

            if (_systemSettings.Toggle.EnableExternalTransfer)
            {
                Survey.ExternalTransferType = Convert.ToByte(ddlExternalTransfer.SelectedValue);
            }

            if (_systemSettings.Toggle.EnableInternalTransfer)
            {
                Survey.InternalTransferType = Convert.ToByte(ddlInternalTransfer.SelectedValue);
            }

            if (trInboundBehavior.Visible)
            {
                Survey.InboundCallBehavior = (byte)ddlInboundBehaviorType.SelectedIndex;
            }

            using (var transactionScope = new DatabaseTransactionScope("UpdateSurvey", DeadlockPriority.Supervisor))
            {
                SurveyRepository.Update(Survey);

                if (isBalancedQuotaCleared.Value == "true")
                {
                    _quotaBalancingService.ResetQuotaBalancingConfiguration(Survey.SID);
                }

                if (isClusteredQuotaCleared.Value == "true")
                {
                    _quotaClusteringConfigurationService.Reset(Survey.SID);
                }

                transactionScope.Commit();
            }

            SurveyService.SetCallDeliveryMode(Survey.SID, callDeliveryMode);

            evt.Finish();

            stateChecker.MarkAsUnchanged();
        }

        private void ReassignShiftTypesIfNeeded(int scheduleId)
        {
            try
            {
                var shiftTypesToChange =
                    _callQueueService.GetShiftTypesThatNeedChange(scheduleId, Survey.SID);

                if (shiftTypesToChange.Count > 0)
                {
                    foreach (var shiftToChange in shiftTypesToChange)
                    {
                        CallManager.ChangeShiftTypeOfCalls(Survey.SID, shiftToChange.ShiftTypeAnalogId,
                            new FilteredBatchParameters(
                                Survey.SID,
                                0, 
                                LocalTimezoneProvider.GetLocalTimezoneId(),
                                CallStates.Scheduled,
                                new SearchParameterCollection(
                                    new List<SearchParameter>()
                                    {
                                        new SearchParameter()
                                        {
                                            ColumnName = "ShiftType", 
                                            ColumnType = SearchColumnType.Text, 
                                            Operator = SearchOperator.Equal, 
                                            Value = shiftToChange.ShiftTypeCurrent
                                        }
                                    })), 
                            true);
                    }
                }
            }
            catch (Exception)
            {
                Page.RegisterScriptBlock($"alert('{Strings.ItIsNecessaryToReassignCalls}');");
            }
        }

        private void ShowScriptNotLaunchedNotification(string scheduleName, string defaultScheduleName)
        {
            Page.RegisterScriptBlock($"alert('{string.Format(Strings.SurveyScheduleNotLaunched, scheduleName, defaultScheduleName)}');");
        }

        protected void SaveHandler(object sender, EventArgs e)
        {
            Save();

            OnUpdateAssignedScriptEvent();
        }

        private void OnUpdateAssignedScriptEvent()
        {
            Page.RegisterStartupScript("Common.fireGlobalEvent('SurveyViewAssignedScriptChanged');");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            int surveyId = Survey.SID;

            stateChecker.AddSaveButton(btnSave);

            if (IsFirstTime)
            {
                SrvName.Text = Survey.Name;
                SrvDescription.Text = Survey.Description;
                txtSrvSize.InnerText = SurveyService.GetSampleSize(surveyId).ToString();
                txtSrvState.InnerText = GetResString("SrvState_" + Survey.State);
                txtTarget.Text = Survey.Target.HasValue ? Survey.Target.Value.ToString() : String.Empty;

                var dialingMode = Survey.DialingMode;

                lbDiallingMode.Text = StringHelper.GetStringFromEnum(dialingMode);
                lbCallGroupsWarning.Visible = _systemSettings.CallGroup.Enabled && (dialingMode == DialingMode.Predictive);

                txtOpenendReview.InnerText = Survey.ForceOpnRev == 1
                                                 ? Strings.Enabled
                                                 : Strings.Disabled;
                txtInterviewVoiceRecording.InnerText = Survey.RecWholeInt == 1
                                                      ? Strings.Enabled
                                                      : Strings.Disabled;
                txtInterviewScreenRecording.InnerText = Survey.InterviewScreenRecording
                                                      ? Strings.Enabled
                                                      : Strings.Disabled;
                txtSupportTelBlacklist.InnerText = Survey.IsTelephoneBlacklistSupported
                                                      ? Strings.Enabled
                                                      : Strings.Disabled;

                lbPageInfo.Text = string.Format(Strings.GeneralSettingsForSurvey, Survey.Description, Survey.Name);

                lbtnFilterByProjectId.OnClientClick = $"filterSurveys('{Survey.Name}'); return false;";

                FillSchedudingScriptDropdown();

                FillStateGroupDropdown();

                FillCallDeliveryModeDropdown();

                if (_systemSettings.Toggle.EnableInternalTransfer)
                {
                    FillInternalTransferTypeDropdown();
                }
                else
                {
                    trInternalTransfer.Visible = false;
                }

                if (_systemSettings.Toggle.EnableExternalTransfer)
                {
                    FillExternalTransferTypeDropdown();
                }
                else
                {
                    trExternalTransfer.Visible = false;
                }

                IsFirstTime = false;

                ddlCallGroupsMode.SelectedValue = Survey.SurveySchedulingMode.ToString();

                if (_systemSettings.Toggle.EnableInbound)
                {
                    ddlInboundBehaviorType.SelectedIndex = Survey.InboundCallBehavior;
                }
                else
                {
                    trInboundBehavior.Visible = false;
                }
            }

            if (!Page.User.AllowedTabs.HasFlag(Tabs.Scheduling))
                lbtnJumpToScheduling.Style["visibility"] = "hidden";
            else
            {
                int scheduleId = Convert.ToInt32(ddlSchedulingScript.SelectedValue);
                lbtnJumpToScheduling.OnClientClick = String.Format("jumpToShedule({0}); return false;", scheduleId);
            }

            btnFilterTasks.OnClientClick = $"top.catiGoTo.jumpToTaskListByProjectId('{Survey.ProjectId}'); return false;";

            if (!Page.User.AllowedTabs.HasFlag(Tabs.Resources))
                lbtnJumpToExtendedStatuse.Style["visibility"] = "hidden";
            else
            {
                int stateGroupId = Convert.ToInt32(lbITSDefGroup.SelectedValue);
                lbtnJumpToExtendedStatuse.OnClientClick = String.Format("jumpToExtendedStatus({0}); return false;", stateGroupId);
            }

            BindBalancedQuota();
            BindClusteredQuota();

            lbtnSetupBalancingParameters.OnClientClick =
                        String.Format("showQuotaBalancingParametersDialog('{0}','{1}'); return false;",
                            surveyId,
                            Strings.QuotaBalancingParameters
                        );

            lbtnSetupClusteringParameters.OnClientClick =
                        String.Format("showQuotaClusteringParametersDialog('{0}','{1}'); return false;",
                            surveyId,
                            Strings.QuotaClusteringParameters
                        );

            trCallGroups.Visible = _systemSettings.CallGroup.Enabled;
        }

        private void BindBalancedQuota()
        {
            if (DoesSurveyHasCatiQuotas())
            {
                trQuotaForBalancing.Visible = true;
                string balancedQuotaName = string.Join(", ", QuotaManager.GetBalancedQuotaNames(Survey.SID));

                if (String.IsNullOrEmpty(balancedQuotaName))
                {
                    btnClearBalancedQuota.Style["visibility"] = "hidden";
                }
                else
                {
                    txtQuotaForBalancing.Text = balancedQuotaName;
                    btnClearBalancedQuota.Style["visibility"] = "visible";
                }
            }
            else
            {
                trQuotaForBalancing.Visible = false;
            }
        }

        private void BindClusteredQuota()
        {
            if (DoesSurveyHasCatiQuotas() &&
                (_systemSettings.QuotaClustering.Enabled || _quotaClusteringConfigurationService.IsEnabled(Survey.SID)))
            {
                trQuotaForClustering.Visible = true;

                string clusteredQuotaName = _quotaClusteringConfigurationService.GetConfiguration(Survey.SID).QuotaName;


                if (String.IsNullOrEmpty(clusteredQuotaName))
                {
                    btnClearClusteredQuota.Style["visibility"] = "hidden";
                }
                else
                {
                    txtQuotaForClustering.Text = clusteredQuotaName;
                    btnClearClusteredQuota.Style["visibility"] = "visible";
                }
            }
            else
            {
                trQuotaForClustering.Visible = false;
            }
        }

        private bool DoesSurveyHasCatiQuotas()
        {
            return QuotaManager.GetQuotaNames(this.Survey.SID).Length != 0;
        }

        private void FillStateGroupDropdown()
        {
            lbITSDefGroup.Items.Clear();
            var stateGroups = StateGroupRepository.GetAll();
            stateGroups.Sort(new CommonComparer<BvStateGroupEntity>("Name", true));
            foreach (BvStateGroupEntity stateGroup in stateGroups)
            {
                ListItem item = new ListItem(stateGroup.Name, stateGroup.ID.ToString());
                item.Selected = stateGroup.ID == Survey.StateGroupID;
                lbITSDefGroup.Items.Add(item);
            }
        }

        private void FillSchedudingScriptDropdown()
        {
            ddlSchedulingScript.Items.Clear();
            var schedules = ScheduleRepository.GetAll();
            var launchedScripts = schedules.Where(s => s.ScriptSource != null).OrderBy(s => s.Name);

            foreach (var schedule in launchedScripts)
            {
                ListItem item = new ListItem(schedule.Name, schedule.ScheduleID.ToString());
                item.Selected = Survey.ScheduleID == schedule.ScheduleID;
                ddlSchedulingScript.Items.Add(item);
            }

            var selectedSchedule = schedules.First(s => s.ScheduleID == Survey.ScheduleID);
            if (selectedSchedule.ScriptSource == null)
            {
                ddlSchedulingScript.Items[0].Selected = true;
                stateChecker.MarkAsChanged();
                ShowScriptNotLaunchedNotification(selectedSchedule.Name, ddlSchedulingScript.Items[0].Text);
                OnUpdateAssignedScriptEvent();
            }
        }

        private void FillCallDeliveryModeDropdown()
        {
            ListItem orderedByInterviewIdItem = new ListItem(Strings.OrderedByInterviewIdCallDeliveryMode, ((int)CallDeliveryMode.InOrder).ToString());
            ListItem randomItem = new ListItem(Strings.RandomCallDeliveryMode, ((int)CallDeliveryMode.Random).ToString());

            if (Survey.IsRandomCallDeliveryEnabled)
                randomItem.Selected = true;
            else
                orderedByInterviewIdItem.Selected = true;

            ddlCallDeliveryMode.Items.AddRange(new[] { orderedByInterviewIdItem, randomItem });
        }

        private void FillInternalTransferTypeDropdown()
        {
            ListItem internalWarm = new ListItem(Strings.WarmTransfer, ((int)InternalTransferType.Warm).ToString());
            ListItem internalCold = new ListItem(Strings.ColdTransfer, ((int)InternalTransferType.Cold).ToString());
            ListItem internalOff = new ListItem(Strings.InternalTransferOff, ((int)InternalTransferType.Off).ToString());

            ddlInternalTransfer.Items.AddRange(new[] { internalWarm, internalCold, internalOff });
            internalWarm.Selected = (Survey.InternalTransferType == (int)InternalTransferType.Warm);
            internalCold.Selected = (Survey.InternalTransferType == (int)InternalTransferType.Cold);
            internalOff.Selected = (Survey.InternalTransferType == (int)InternalTransferType.Off);
        }

        private void FillExternalTransferTypeDropdown()
        {
            ListItem externalWarm = new ListItem(Strings.WarmTransfer, ((int)ExternalTransferType.Warm).ToString());
            ListItem externalCold = new ListItem(Strings.ColdTransfer, ((int)ExternalTransferType.Cold).ToString());

            ddlExternalTransfer.Items.AddRange(new[] { externalWarm, externalCold });
            externalWarm.Selected = (Survey.ExternalTransferType == (int)ExternalTransferType.Warm);
            externalCold.Selected = (Survey.ExternalTransferType == (int)ExternalTransferType.Cold);
        }
    }
}
