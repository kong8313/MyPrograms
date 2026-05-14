using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Script.Classes;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using Infragistics.Web.UI.GridControls;
using Confirmit.CATI.Supervisor.Classes.Script;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using System.Web;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public partial class SchedulingRulesNewControl : ScheduleControlBase
    {
        private IActionCollection _actionCollection;
        private Dictionary<int, string> _cachedGroupStatuses;

        public override void AddConfirmationWhileLaunch()
        {
            // m_grid.Commands.First(x => x.Key == "Launch").Confirmation = Strings.LaunchScriptConfirmation;
        }

        protected override string ClientControllerName
        {
            get { return "schedulingRulesController"; }
        }

        protected RuleCollection Rules
        {
            get
            {
                return WorkingSchedule.Rules;
            }
        }

        protected ShiftTypeCollection ShiftTypeCollection
        {
            get
            {
                return WorkingSchedule.ShiftTypes;
            }
        }

        protected IActionCollection ActionCollection
        {
            get
            {
                var scheduleService = ServiceLocator.Resolve<IScheduleService>();
                return _actionCollection ?? (_actionCollection = scheduleService.GetActions());
            }
        }

        protected Dictionary<int, string> CachedGroupStatuses
        {
            get
            {
                if (_cachedGroupStatuses == null)
                {
                    _cachedGroupStatuses = new Dictionary<int, string>();
                    var stateGroup = StateGroupsManager.GetStateGroupForScript(Schedule);
                    StateRepository.GetAll(stateGroup.ID).ForEach(x => _cachedGroupStatuses.Add(x.StateID, x.Name));
                }

                return _cachedGroupStatuses;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            m_grid.RowIslandsPopulating += m_grid_RowIslandsPopulating;
            m_grid.LeftLabel = string.Format("Script \"{0}\" using extended status group \"{1}\"", 
                Schedule.Name, StateGroupsManager.GetStateGroupForScript(Schedule).Name);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var newSubruleCommand = ((OverlayCommand)m_grid.GetCommand("NewSubrule"));
            newSubruleCommand.AddDynamicClientParameter("ParentRuleId", ClientControllerName + ".getIdForNewRow('subrule')");

            var newActionCommand = ((OverlayCommand)m_grid.GetCommand("NewAction"));
            newActionCommand.AddDynamicClientParameter("ParentSubruleId", ClientControllerName + ".getIdForNewRow('action')");
            newActionCommand.AddDynamicClientParameter("ParentRuleId", ClientControllerName + ".getGrandParentIdForNewRow('action')");

            var overlayEditSubrule = ((OverlayCommand)m_grid.GetCommand("EditSubRule"));
            overlayEditSubrule.AddDynamicClientParameter("ParentRuleId", ClientControllerName + ".getParentId()");

            var overlayEditAction = ((OverlayCommand)m_grid.GetCommand("EditAction"));
            overlayEditAction.AddDynamicClientParameter("ParentRuleId", ClientControllerName + ".getGrandParentId()");
            overlayEditAction.AddDynamicClientParameter("ParentSubruleId", ClientControllerName + ".getParentId()");

            m_grid.InitializeRow += m_grid_InitializeRow;
            m_grid.GetPage += m_grid_GetPage;
            m_grid.InitDataSource();

            StateChecker.AddSaveButton(btnSave);
        }

        public void m_grid_RowIslandsPopulating(object sender, ContainerRowCancelEventArgs e)
        {
            e.Cancel = true;

            var sr = (((Infragistics.Web.UI.Framework.Data.ListNode)e.Row.DataItem)).Item as SubRuleInfo;

            if (e.Row.RowIslands.Count == 0)
            {
                e.Row.RowIslands.Add(new ContainerGrid { Band = m_grid.Bands[0].Bands[0], Level = 2 });
            }

            var child = e.Row.RowIslands[0];

            child.DataSource = sr.Actions;

            child.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterScripts();
        }

        private void RegisterScripts()
        {
            PageHelper.RegisterClientLibrary("client/SchedulingRulesControl.js");
            Page.ClientScript.RegisterStartupScript(GetType(),
                                                    ClientControllerName,
                                                    string.Format("var {0} = new SchedulingRulesController({1}, {2});", ClientControllerName, GetClientSettings(), m_grid.ClientControllerName),
                                                    true);
        }

        private object GetClientSettings()
        {
            var settings = new
            {
                GridId = m_grid.GridClientId,
                ExportButtonClientId = btnExport.ClientID,
                EditRuleFunction = m_grid.GetCommand("EditRule").GetClientEventJavaScript(Page, m_grid),
                EditSubRuleFunction = m_grid.GetCommand("EditSubRule").GetClientEventJavaScript(Page, m_grid),
                EditActionFunction = m_grid.GetCommand("EditAction").GetClientEventJavaScript(Page, m_grid),
                UpdateAndMarkAsChangedFunction = m_grid.GetCommand("UpdateAndMarkAsChanged").GetClientEventJavaScript(Page, m_grid),
                CopiedRowKey = hfCopiedRowKey.ClientID,
                UpdatePanelId = updatePanel.ClientID,
                SeachDropDownClientId = ddlSearch.ClientID,
                SearchTextBoxClientId = tbSearch.ClientID
            };

            return new JavaScriptSerializer().Serialize(settings);
        }

        private void m_grid_InitializeRow(object sender, RowEventArgs e)
        {
            int level = ((ContainerGridRecord)(e.Row)).Level;

            var row = e.Row;

            if (level == (int)GridBandType.Rules)
            {
                var ruleInfo = new RuleInfo();

                ScheduleManager.FillObjectFromRow(ruleInfo, row);
                string number = WorkingSchedule.GetNumberByGuid(ruleInfo.Id.Value);

                row.Items.FindItemByKey("Number").Value = string.Format(GetResString("RuleNumber"), number);
                FillEmptySellsWithOneWhiteSpace(row);
            }
            else if (level == (int)GridBandType.Subrules)
            {
                var subRuleInfo = new SubRuleInfo();
                ScheduleManager.FillObjectFromRow(subRuleInfo, row);
                string number = WorkingSchedule.GetNumberByGuid(subRuleInfo.Id.Value);

                row.Items.FindItemByKey("Number").Value = number;

                //Set ShiftType name with ShiftTypeId
                int shiftTypeId = (int)row.Items.FindItemByKey("ShiftTypeId").Value;
                if (shiftTypeId == 0)
                {
                    row.Items.FindItemByKey("ShiftTypeName").Value = GetResString("Any");
                }
                else
                {
                    row.Items.FindItemByKey("ShiftTypeName").Value = ShiftTypeCollection.GetItemById(shiftTypeId).Name;
                }

                //Set Its name by ItsId
                int itsId = (int)row.Items.FindItemByKey("ItsId").Value;

                if (itsId == 0)
                {
                    row.Items.FindItemByKey("ItsName").Value = Strings.Any;
                }
                else
                {
                    row.Items.FindItemByKey("ItsName").Value = CachedGroupStatuses[itsId];
                }

                var filterEnabled = (bool)row.Items.FindItemByKey("FilterEnabled").Value;
                row.Items.FindItemByKey("Filter").CssClass = filterEnabled ? "" : "igg_disabledCell";
            }
            else if (level == (int)GridBandType.Actions)
            {
                //Set Action name by ActionId
                int actionId = (int)row.Items.FindItemByKey("ActionId").Value;
                row.Items.FindItemByKey("ActionName").Value = ActionCollection.GetActionById(actionId).Name;

                row.Items.FindItemByKey("Parameter").Value = row.Items.FindItemByKey("ParameterValue").Value;

                var isSchedulingParameter = Convert.ToBoolean(row.Items.FindItemByKey("IsSchedulingParameter").Value);
                if (isSchedulingParameter)
                {
                    //Set Param name by ParamId
                    int paramId = Convert.ToInt32(row.Items.FindItemByKey("ParameterValue").Value);
                    row.Items.FindItemByKey("Parameter").Value = WorkingSchedule.CustomParameters.GetItemById(paramId).Name;
                }
                else
                {
                    if (ActionManager.IsGoToAction(actionId) || ActionManager.IsSetNextRuleAction(actionId))
                    {
                        row.Items.FindItemByKey("Parameter").Value =
                            WorkingSchedule.GetNumberByGuid(
                                new Guid(row.Items.FindItemByKey("ParameterValue").Value.ToString()));
                    }
                    else if (ActionManager.IsSetCallExpirationTime(actionId) ||
                             ActionManager.IsRecallOnSpecificTime(actionId))
                    {
                        row.Items.FindItemByKey("Parameter").Value =
                            ScheduleManager.ConvertToDateTime(row.Items.FindItemByKey("ParameterValue").Value.ToString(),
                                                              ConvertDirection.ToClient);
                    }
                }

                var actionEnabled = (bool)row.Items.FindItemByKey("Enabled").Value;

                row.CssClass = actionEnabled ? "" : "igg_disabledRow";

                var filterEnabled = (bool)row.Items.FindItemByKey("FilterEnabled").Value;
                row.Items.FindItemByKey("Filter").CssClass = filterEnabled ? "" : "igg_disabledCell";
            }
        }

        protected RuleInfo[] m_grid_GetPage(out int total_count)
        {
            return ScheduleManager.GetRules(Rules, out total_count);
        }

        protected void OnChange(object sender, EventArgs eventArgs)
        {
            Page.RegisterStartupScript("Common.fireGlobalEvent('ScriptViewScheduleRuleChanged');");
            ScheduleChangedHandler(this, eventArgs);
        }

        /// <summary>
        /// Fills empty sells with one white space. 
        /// It is a hack to show empty cell borders in Quirks mode.
        /// </summary>        
        private void FillEmptySellsWithOneWhiteSpace(GridRecord row)
        {
            for (int i = 0; i < row.Items.Count; i++)
            {
                if (String.IsNullOrEmpty(row.Items[i].Text))
                {
                    row.Items[i].Text = "&nbsp";
                }
            }
        }

        public static SchedulingRulesOperationResult EnableAction(string actionKey, bool enabled, string scriptId)
        {
            var result = new SchedulingRulesOperationResult { Success = true };

            ErrorCollection errors;

            var key = new SchedulingRulesViewKey(actionKey);

            if (new EnableActionOperationHelper(GetWorkingSchedule(scriptId)).Enable(key, enabled, out errors) == false)
            {
                result.Success = false;
                result.ErrorMessage = errors.First().Message;
            }

            return result;
        }

        public static SchedulingRulesOperationResult Delete(string rowKey, string scriptId)
        {
            var result = new SchedulingRulesOperationResult { Success = true };

            ErrorCollection errors;

            var key = new SchedulingRulesViewKey(rowKey);

            if (new DeleteOperationHelper(GetWorkingSchedule(scriptId)).Delete(key, out errors) == false)
            {
                result.Success = false;
                result.ErrorMessage = errors.First().Message;
            }

            return result;
        }

        public static SchedulingRulesOperationResult Move(string rowKey, bool moveUp, string scriptId)
        {
            var result = new SchedulingRulesOperationResult { Success = true };

            ErrorCollection errors;

            var key = new SchedulingRulesViewKey(rowKey);

            if (new MoveOperationHelper(GetWorkingSchedule(scriptId)).Move(key, moveUp, out errors) == false)
            {
                result.Success = false;
                result.ErrorMessage = errors.First().Message;
            }

            return result;
        }

        public static SchedulingRulesOperationResult Paste(string copiedRowKey, string pasteRowKey, string scriptId)
        {
            var result = new SchedulingRulesOperationResult { Success = true };

            string pastedRowKey;
            ErrorCollection errors;

            var copiedRowkey = new SchedulingRulesViewKey(copiedRowKey);
            var pasteRowkey = new SchedulingRulesViewKey(pasteRowKey);

            if (new CopyPasteOperationHelper(GetWorkingSchedule(scriptId)).Paste(copiedRowkey, pasteRowkey, out errors, out pastedRowKey) == false)
            {
                result.Success = false;
                result.ErrorMessage = errors.First().Message;
            }
            else
            {
                result.HighlightRowKey = pastedRowKey;
            }

            return result;
        }

        public static Schedule GetWorkingSchedule(string workingScheduleId) {
            return (Schedule)HttpContext.Current.Session[$"WorkingSchedule_{workingScheduleId}"];
        }
    }
}
