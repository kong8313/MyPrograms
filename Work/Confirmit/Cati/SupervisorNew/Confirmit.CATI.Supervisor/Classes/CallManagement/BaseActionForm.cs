using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.CallManagement;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Newtonsoft.Json;

namespace Confirmit.CATI.Supervisor.Classes.CallManagement
{
    [CheckSurveyPermission(RequestParameterName = "SurveyID")]
    public class BaseActionForm : BaseForm  
    {
        protected readonly ICachedLocalTimezoneManager LocalTimezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private List<int> _ids;

        private ActionValueCalculator _calculator;

        /// <summary>
        /// Selection type.
        /// Indicate how user select calls. Can be 'Selected' or 'Filtered'
        /// </summary>
        protected CallSelectionType SelectionType
        {
            get => (CallSelectionType)ViewState["CallSelectionType"];
            set => ViewState["CallSelectionType"] = value;
        }

        protected string QuotaName
        {
            get => (string)ViewState["QuotaName"];
            set => ViewState["QuotaName"] = value;
        }

        protected string[] QuotaFields
        {
            get => ((string)ViewState["QuotaFields"]).Split(',');
            set => ViewState["QuotaFields"] = string.Join(",", value);
        }

        /// <summary>
        /// QuotaFieldsFromQuota is passed from React quota UI where multiple quotas can be processed
        /// </summary>
        protected QuotaWithCellsAndFieldsParameters[] QuotaWithCellsAndFields
        {
            get => ViewState["QuotaWithCellsAndFields"] as QuotaWithCellsAndFieldsParameters[];
            set => ViewState["QuotaWithCellsAndFields"] = value;
        }

        protected ActionValueCalculator Calculator => _calculator ?? (_calculator = new ActionValueCalculator(new CallDatabaseProvider()));

        /// <summary>
        /// CallState passed in dialog params from 'CallManagement' page
        /// </summary>
        protected CallStates CallState
        {
            get => (CallStates)ViewState["CallState"];
            set => ViewState["CallState"] = value;
        }

        /// <summary>
        /// SurveyID passed in dialog params.
        /// </summary>
        public int SurveyID
        {
            get => (int)ViewState["SurveyID"];
            set => ViewState["SurveyID"] = value;
        }

        /// <summary>
        /// Selected FilterID
        /// If filter hasn't been 
        /// </summary>
        protected int FilterID
        {
            get
            {
                int id = 0;
                object o = ViewState["FilterID"];
                if (o != null)
                {
                    if (Int32.TryParse(o.ToString(), out id))
                    {
                        return id;
                    }
                }
                return id;
            }
        }

        /// <summary>
        /// Selected call ids
        /// </summary>
        protected List<int> IDS
        {
            get
            {
                if (_ids == null)
                {
                    string requestIDS = (String)ViewState["IDS"];
                    _ids = new List<int>();
                    if (!string.IsNullOrEmpty(requestIDS))
                    {
                        string[] ids = requestIDS.Split(',');
                        foreach (string id in ids)
                        {
                            _ids.Add(Int32.Parse(id));
                        }
                    }
                }

                return _ids;
            }
        }

        /// <summary>
        /// Gets search parameters used on call list page.
        /// </summary>
        private SearchParameterCollection SearchParameters
        {
            get
            {
                SearchParameterCollection result = new SearchParameterCollection();
                string viewState = (string)(ViewState["SearchParams"] ?? String.Empty);
                if(!String.IsNullOrEmpty(viewState))
                {
                    result = SearchManager.DeserializeWithDecode(viewState);
                }

                return result;
            }
        }

        protected BatchParameters BatchParameters
        {
            get
            {
                switch (SelectionType)
                {
                    case CallSelectionType.Selected:
                        return new SelectedBatchParameters(IDS);
                    case CallSelectionType.Filtered:
                        return new FilteredBatchParameters(SurveyID, FilterID, LocalTimezoneProvider.GetLocalTimezoneId(), CallState, SearchParameters);
                    case CallSelectionType.QuotaCellFiltered:
                    {
                        var projectId = SurveyRepository.GetById(SurveyID).Name;
                        // Handling multiple quotas
                        if (QuotaWithCellsAndFields != null && QuotaWithCellsAndFields.Any())
                        {
                            var totalQuotas = new List<QuotaParameter>();
                            foreach (var q in QuotaWithCellsAndFields)
                            {
                                var quota = QuotaManager.GetQuotaList(projectId, q.QuotaName);
                                var cells = QuotaManager.GetCellsValues(quota, q.CellIds.ToList(), q.QuotaFields);
                                totalQuotas.Add(new QuotaParameter(q.QuotaFields, cells));
                            }
                            return new FilteredByMultipleCellsBatchParameters(totalQuotas, SurveyID);
                        }

                        // Handle single quota
                        QuotaList quotaList = QuotaManager.GetQuotaList(projectId, QuotaName);
                        var cellsFields = QuotaManager.GetCellsValues(quotaList, IDS, QuotaFields);
                        return new FilteredByCellsBatchParameters(SurveyID, QuotaFields, cellsFields);
                    }
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                SurveyID = Int32.Parse(Request.Params["SurveyID"]);
                CallState = (CallStates)Int32.Parse(Request.Params["CallState"]);
                SelectionType = (CallSelectionType)Int32.Parse(Request.Params["CallSelectionType"]);
                ViewState["IDS"] = Request.Params["IDS"];
                ViewState["FilterID"] = Request.Params["FilterID"];
                ViewState["SearchParams"] = Request.Params["SearchParams"];
                ViewState["QuotaFields"] = Request.Params["QuotaFields"];
                ViewState["QuotaName"] = Request.Params["QuotaName"];
                ViewState["QuotaWithCellsAndFields"] = Request.Params["QuotaWithCellsAndFields"] != null ? JsonConvert.DeserializeObject<QuotaWithCellsAndFieldsParameters[]>(Request.Params["QuotaWithCellsAndFields"]) : null;
            }

            base.OnLoad(e);
        }

        protected void NotifyUser(string message)
        {
            message = message.Replace("'", "");
            message = message.Replace("\n", " ");
            message = message.Replace("\r", "");
            string script = $"<script type='text/javascript'>alert( '{message}' )</script>;";
            Page.ClientScript.RegisterStartupScript(GetType(), "Notification", script);
        }

        protected void Redirect(BvAsyncOperationQueueEntity operationEntity)
        {
            Redirect($"~/AsyncOperations/AsyncOperationProgress.aspx?OperationId={operationEntity.Id}");
        }
    }
}
