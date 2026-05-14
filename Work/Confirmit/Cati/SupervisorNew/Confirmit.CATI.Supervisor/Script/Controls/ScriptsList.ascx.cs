using System;
using System.Linq;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using System.Collections.Generic;
using System.Web.UI;
using System.Xml.Serialization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Script.Classes;
using Confirmit.CATI.Supervisor.ServerControls;

using Infragistics.Web.UI.GridControls;

using Strings = Confirmit.CATI.Supervisor.Resources.Strings;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public partial class ScriptsList : BaseWUC
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider;
        private readonly ISupervisorServiceClient _supervisorService;
        private readonly IScheduleService _scheduleService;

        private const string ExportFileName = "Schedule.xml";

        private int _defaultScriptId;
        private BvScheduleEntity _defaultScript;   
        
        [StoreInViewState]
        protected int? ScriptId;

        public ScriptsList()
        {
            _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
            _supervisorService = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
        }

        public int ScriptEventTypeId
        {
            get
            {
                return (ViewState["ScriptEventTypeId"] == null ? 0 : (int)ViewState["ScriptEventTypeId"]);
            }
            set
            {
                ViewState["ScriptEventTypeId"] = value;
            }
        } 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["ScriptId"] != null)
                {
                    ScriptId = int.Parse(Request["ScriptId"]);
                }
            }
            
            _defaultScriptId = _scheduleService.DefaultScheduleId;
            _defaultScript = ScheduleRepository.GetById(_defaultScriptId);

            Scripts.OnDblClickCommand = "View";
            Scripts.InitializeRow += OnInitializeRow;

            Scripts.GetCommand("CopySchedulingScriptToDefault").Confirmation = string.Format(Strings.CopyToDefaultSchedulingScriptConfirmation, _defaultScript.Name);

            var column = Scripts.Columns.FromKey("State") as ISearchableField;

            if (column != null)
            {
                var items = Enum.GetValues(typeof(SchedulingScriptState))
                        .Cast<SchedulingScriptState>()
                        .Select(x => new ListItem(StringHelper.GetStringFromEnum(x), ((int)x).ToString()));
                column.Items.AddRange(items);
            }

            Scripts.GetPage += delegate(out int totalCount)
            {
                PagingArgs args = new PagingArgs(
                    Scripts.PageIndex,
                    Scripts.PageSize,
                    Scripts.SortedColumnName,
                    Scripts.SortIndicatorAsc,
                    Scripts.SearchParameterCollection);

                var scripts = ScheduleRepository.GetPage(
                    args,
                    _timezoneProvider.GetLocalTimezoneId(),
                    out totalCount);

                foreach (var script in scripts)
                {
                    script.Name = script.Name.Replace('\0', ' ');
                    
                    if (script.CreateDate != null)
                    {
                        script.CreateDate = _timezoneProvider.ConvertToLocalTime(script.CreateDate.Value);
                    }

                    if (script.ModifyDate != null)
                    {
                        script.ModifyDate = _timezoneProvider.ConvertToLocalTime(script.ModifyDate.Value);
                    }
                }

                return scripts;
            };
            

        }

        private void OnInitializeRow(object sender, RowEventArgs args)
        {
            var stateItem = args.Row.Items.FindItemByKey("State");
            stateItem.Value = StringHelper.GetStringFromEnum((SchedulingScriptState)((BvSpSchedule_ListPageEntity)args.Row.DataItem).State);
            stateItem.Column.Type = typeof(string);

            if (args.Row.Items.FindItemByKey("ID").Text == _defaultScript.ScheduleID.ToString())
            {
                args.Row.CssClass = "highlighted-in-bold";
            }

            bool isSynchronized = ((BvSpSchedule_ListPageEntity)args.Row.DataItem).State == 2;

            stateItem.Column.Type = typeof(string);
            if (isSynchronized)
            {
                stateItem.CssClass += " greenFont";
            }
            else
            {
                stateItem.CssClass += " orangeFont";
            }
        }

        /// <summary>
        /// Deletes selected scripts
        /// </summary>
        public void DeleteScript(object sender, EventArgs e)
        {
            try
            {
                _scheduleService.DeleteSchedulingScripts(Scripts.SelectedKeysInt);

                Scripts.RefreshData();
                Page.CloseInfoFrame();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        public void DuplicateScript(object sender, EventArgs e)
        {
            try
            {
                const int findNameTriesLimit = 1000;
                foreach (int scriptId in Scripts.SelectedKeysInt)
                {

                    var existingScript = ScheduleRepository.GetById(scriptId);

                    int copyCount = 0;
                    string newName = string.Empty;

                    while (copyCount < findNameTriesLimit)
                    {
                        newName = $"Copy {(copyCount == 0 ? "" : "(" + copyCount + ") ")}of {existingScript.Name}";

                        if (ScheduleRepository.IsNameUsed(newName))
                        {
                            copyCount += 1;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (copyCount < findNameTriesLimit) // New name has been successfully found.
                    {
                        var evt = new ScriptDuplicateEvent();

                        Schedule duplicateSchedule = ScheduleManager.ScheduleById(scriptId);
                        _supervisorService.CheckSchedule(ScheduleManager.SerializeSchedule(duplicateSchedule));

                        using (var transactionScope = new DatabaseTransactionScope("ScriptsList.DuplicateScript", DeadlockPriority.Supervisor))
                        {
                            var newBvSchedule = ScheduleManager.AddSchedule(newName);

                            duplicateSchedule.Id = newBvSchedule.ScheduleID;
                            newBvSchedule.XmlUnderDev = ScheduleManager.SerializeSchedule(duplicateSchedule);

                            ScheduleRepository.Update(newBvSchedule);

                            evt.ObjectId = newBvSchedule.ScheduleID;
                            evt.ObjectName = newBvSchedule.Name;

                            transactionScope.Commit();
                        }

                        evt.Finish();
                    }
                }

                Scripts.RefreshData();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        public void CopySchedulingScriptToDefault(object sender, EventArgs e)
        {
            try
            {
                if (!SupervisorPrincipal.Current.IsCatiAdministratorOrPros)
                {
                    throw new UserMessageException(Strings.Error_NoPermissionForCopyToDefaultAction);
                }

                _scheduleService.CopySchedulingScriptToDefault(Scripts.SelectedKeysInt[0], _timezoneProvider.GetCurrentLocalTime());
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }


        public void ExportScript(object sender, EventArgs e)
        {
            try
            {
                var scripts =
                    from scriptId in Scripts.SelectedKeysInt
                    let schedule = ScheduleManager.ScheduleById(scriptId)
                    let sheduleEntity = ScheduleRepository.GetById(scriptId)
                    select new SchedulingScript(sheduleEntity.Name, schedule);

                Page.FileToClientSender.Send(scripts.ToList(), ExportFileName);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        public void ImportScript(object sender, EventArgs e)
        {            
            try
            {
                if (FileLoad.HasFile)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<SchedulingScript>));
                    List<SchedulingScript> list;
                    try
                    {
                        list = (List<SchedulingScript>)serializer.Deserialize(FileLoad.PostedFile.InputStream);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Page.AddUserMessage(Strings.ErrorFileIncorrect, ex);
                        return;
                    }

                    string importResult = string.Empty;

                    using (var transactionScope = new DatabaseTransactionScope("ScriptsList.ImportScript", DeadlockPriority.Supervisor))
                    {
                        foreach (SchedulingScript schedulingScript in list)
                        {
                            Schedule schedule = schedulingScript.Schedule;
                            _supervisorService.CheckSchedule(ScheduleManager.SerializeSchedule(schedule));

                            int[] usedTimezoneIds = schedule.GetUsedTimezoneIds();

                            string message;
                            if (!CheckImportingTimezones(usedTimezoneIds, out message))
                            {
                                Page.AddUserMessage(
                                    Environment.NewLine + String.Format(
                                                              Strings.UnableToAddTimezone,
                                                              message,
                                                              schedulingScript.Name));
                                continue;
                            }
                            importResult += message;

                            BvScheduleEntity scheduleEntity;
                            try
                            {
                                scheduleEntity = ScheduleManager.AddSchedule(schedulingScript.Name);
                            }
                            catch (SqlException ex)
                            {
                                if (BaseMethods.IsUniqueConstraint(ex))
                                {
                                    Page.AddUserMessage(String.Format(Strings.ScriptExists, schedulingScript.Name));
                                }
                                else
                                {
                                    Context.AddError(ex);
                                }
                                //go exit because transaction after exception cannot be commited
                                return;
                            }

                            var evt = new ScriptImportEvent(scheduleEntity.ScheduleID, scheduleEntity.Name);

                            BvScheduleEntity bvschedule = ScheduleRepository.GetById(scheduleEntity.ScheduleID);
                            bvschedule.XmlUnderDev = ScheduleManager.SerializeSchedule(schedule);

                            ScheduleRepository.Update(bvschedule);

                            evt.Finish();
                        }

                        transactionScope.Commit();
                    }

                    if (!String.IsNullOrEmpty(importResult))
                    {
                        importResult = Strings.TimezoneAdded + importResult;
                        Page.AddUserMessage(importResult);
                    }

                    Scripts.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }            
        }

        /// <summary>
        /// Checks importing script for using timezones that are not activated in the system.
        /// If timezone is in Master Timezone List it is activated, True returns; False returns otherwise. 
        /// </summary>
        /// <param name="usedTimezoneIds">Used iimezones Ids</param>
        /// <param name="message">Return message with results</param>
        /// <returns>True if timezone is activated; false otherwise.</returns>
        private static bool CheckImportingTimezones(IEnumerable<int> usedTimezoneIds, out string message)
        {
            message = string.Empty;
            BvTimezoneEntityCollection activeTimezones = TimezoneManager.ActiveTimezonesList;

            var newTimezoneIds = new List<int>();

            foreach (int timezoneID in usedTimezoneIds)
            {
                BvTimezoneEntity timezone;
                if (!activeTimezones.TryGetItemById(timezoneID, out timezone)
                    && timezoneID != Shift.RespondentTimezoneId)
                {
                    if (!TimezoneManager.GetMasterTimezonesList().TryGetItemById(timezoneID, out timezone))
                    {
                        message = timezoneID.ToString();
                        return false;
                    }
                    newTimezoneIds.Add(timezoneID);
                }
            }

            foreach (int id in newTimezoneIds)
            {
                TimezoneManager.AddTimezone(id);
                message = message + Environment.NewLine +
                          String.Format("ID={0} {1}", id, TimezoneManager.GetTimezoneByID(id).Name);
            }
            return true;
        } 
        
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (ScriptId.HasValue && IsPostBack == false)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), null, String.Format("openScriptInfoFrame({0});", ScriptId), true);
            }
        }
    }
}