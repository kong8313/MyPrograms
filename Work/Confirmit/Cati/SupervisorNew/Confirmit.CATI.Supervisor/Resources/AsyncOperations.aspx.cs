using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using Infragistics.Web.UI;
using Infragistics.Web.UI.GridControls;
using ICompanyInfoProvider = Confirmit.CATI.Supervisor.Core.Common.ICompanyInfoProvider;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class AsyncOperations : BaseForm
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
        private readonly ICallCenterRepository _callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
        private readonly ICompanyInfoProvider _companyInfoProvider = ServiceLocator.Resolve<ICompanyInfoProvider>(); 
        
        public override string TopTitle
        {
            get { return Strings.Tasks; }
        }

        public int? CallCenterId
        {
            get
            {
                if (_companyInfoProvider.HasCallCentersAddon)
                {
                    return (cbShowAllCallCenters.Checked ? (int?)null : _callCenterProvider.GetCurrentId());                
                }

                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs args)
        {            
            InitSearchingToolBar();

            if (User.IsCatiAdministratorOrPros == false || 
                _companyInfoProvider.HasCallCentersAddon == false)
            {
                cbShowAllCallCenters.Visible = false;
            }

            if (_companyInfoProvider.HasCallCentersAddon == false)
            {
                grid.Columns.FromKey("CallCenterName").Hidden = true;
            }

            if (!IsPostBack)
            {
                if (Request.Params["projectId"]!=null)
                {
                    var values = new SearchParameterCollection();

                    var searchField = grid.Columns.FromKey("ProjectId") as ISearchableField;
                    if (searchField != null)
                    {
                        values.Add(new SearchParameter
                        {
                            ColumnName = searchField.Key,
                            Value = Request.Params["projectId"],
                            ColumnType = searchField.SearchColumnType,
                            Operator = SearchOperator.Equal
                        });

                        Session[grid.GetSearchParametersSessionKey()] = values;
                    }
                }

            }

            grid.GetPage += GetPage;
            grid.InitializeRow += grid_InitializeRow;
        }

        protected void Page_PreRender(object sender, EventArgs args)
        {
            var abortCommand = grid.GetCommand("Abort");
            abortCommand.OnClientClick = String.Format("canTaskBeAborted({0}, {1}, {2})", grid.ClientControllerName, (int)AsyncOperationState.Queued, (int)AsyncOperationState.Executing);
        }

        private object GetPage(out int totalCount)
        {                        
            var list = AsyncOperationRepository.GetPage(CallCenterId,
                                                        grid.PageArguments,
                                                        _timezoneProvider.GetLocalTimezoneId(),
                                                        User.Name,
                                                        out totalCount);
            foreach (var item in list)
            {
                if (item.StartedTime.HasValue)
                    item.StartedTime = _timezoneProvider.ConvertToLocalTime(item.StartedTime.Value);

                if (item.FinishedTime.HasValue)
                    item.FinishedTime = _timezoneProvider.ConvertToLocalTime(item.FinishedTime.Value);

                if (item.InitiatedTime.HasValue)
                    item.InitiatedTime = _timezoneProvider.ConvertToLocalTime(item.InitiatedTime.Value);
            }

            return list;
        }

        private void grid_InitializeRow(object sender, RowEventArgs e)
        {
            var entity = (BvSpAsyncOperations_ListPageEntity)e.Row.DataItem;

            e.Row.Items.FindItemByKey("OperationType").Text = ((OperationTypes)entity.OperationType).GetStringFromEnum();
            e.Row.Items.FindItemByKey("OperationStateName").Text = ((AsyncOperationState)entity.OperationState).GetStringFromEnum();
            
            var durationCell = e.Row.Items.FindItemByKey("Duration");
            durationCell.CssClass += " boldLabel";
            durationCell.Text = GetDurationText(entity);

            var elapsedTimeCell = e.Row.Items.FindItemByKey("ElapsedTime");
            elapsedTimeCell.CssClass += " boldLabel";
            elapsedTimeCell.Text = GetElapsedTimeText(entity);
        }        

        private string GetDurationText(BvSpAsyncOperations_ListPageEntity entity)
        {
            var duration = new TimeSpan(0);

            if (entity.StartedTime.HasValue && entity.FinishedTime.HasValue)
            {
                duration = entity.FinishedTime.Value - entity.StartedTime.Value;
            }
            else if (entity.StartedTime.HasValue)
            {
                duration = _timezoneProvider.GetCurrentLocalTime() - entity.StartedTime.Value;
            }

            return String.Format("{0}:{1}:{2}", ((int)duration.TotalHours).ToString("D2"), duration.Minutes.ToString("D2"), duration.Seconds.ToString("D2"));
        }

        private string GetElapsedTimeText(BvSpAsyncOperations_ListPageEntity entity)
        {
            var elapsedTime = new TimeSpan(0);

            var operationState = (AsyncOperationState) entity.OperationState.Value;

            if (entity.InitiatedTime.HasValue && entity.FinishedTime.HasValue)
            {
                elapsedTime = entity.FinishedTime.Value - entity.InitiatedTime.Value;
            }
            else if (entity.InitiatedTime.HasValue &&
                     (operationState == AsyncOperationState.Queued || operationState == AsyncOperationState.Executing))
            {
                elapsedTime = _timezoneProvider.GetCurrentLocalTime() - entity.InitiatedTime.Value;
            }

            return String.Format("{0}:{1}:{2}", ((int)elapsedTime.TotalHours).ToString("D2"), elapsedTime.Minutes.ToString("D2"), elapsedTime.Seconds.ToString("D2"));
        }

        private void InitSearchingToolBar()
        {
            var column = (GeneralGridColumn)grid.Columns.FromKey("CallCenterName");            
            
            _callCenterRepository.GetAll().ForEach(
                callCenter => column.Items.Add(new ListItem(callCenter.Name, callCenter.ID.ToString(CultureInfo.InvariantCulture)))
            );            
            
            column = (GeneralGridColumn)grid.Columns.FromKey("OperationType");

            foreach (OperationTypes value in Enum.GetValues(typeof(OperationTypes)))
            {
                column.Items.Add(new ListItem(value.GetStringFromEnum(), ((int)value).ToString()));
            }

            column = (GeneralGridColumn)grid.Columns.FromKey("OperationStateName");

            foreach (AsyncOperationState value in Enum.GetValues(typeof(AsyncOperationState)))
            {
                column.Items.Add(new ListItem(value.GetStringFromEnum(), ((int)value).ToString()));
            }          
        }

        protected void AbortTask(object sender, EventArgs e)
        {
            try
            {
                if (grid.SelectedKeysInt.Any())
                {
                    var operationId = grid.SelectedKeysInt[0];
                    var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

                    queue.Abort(operationId, User.Name);
                }                       
            }
            catch (Exception ex)
            {
                Context.AddError(ex);                
            }
            
        }
    }
}
