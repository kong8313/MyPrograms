using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Surveys
{
    /// <summary>
    /// Pop up window used to change quota cell limits.
    /// </summary>
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class ChangeLimit : BaseForm
    {
        private List<int> m_CellIds;

        /// <summary>
        /// Selected quota limit.
        /// </summary>
        private int Limit
        {
            get
            {
                if (wneLimit.ValueInt != wneLimit.ValueLong)
                {
                    throw new UserMessageException(string.Format(Strings.ValueYouHaveEnteredIsTooLarge, Int32.MaxValue));
                }

                return wneLimit.ValueInt;
            }
            set
            {
                wneLimit.ValueInt = value;
            }
        }

        /// <summary>
        /// Selected quota limit.
        /// </summary>
        private string QuotaName
        {
            get
            {
                return (string)(ViewState["QuotaName"] ?? string.Empty);
            }
            set
            {
                ViewState["QuotaName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets current survey SID.
        /// </summary>
        private int SurveySid
        {
            get
            {
                return (int)(ViewState["ID"] ?? 0);
            }
            set
            {
                ViewState["ID"] = value;
            }
        }

        /// <summary>
        /// Cell IDs to change limit for.
        /// </summary>
        private List<int> CellIds
        {
            get
            {
                if (m_CellIds == null)
                {
                    string requestIds = (string)ViewState["IDS"];
                    string[] ids = requestIds.Split(',');
                    m_CellIds = ids.Select(x => Int32.Parse(x)).ToList();
                }
                return m_CellIds;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["IDS"] = Request.Params["IDS"];
                SurveySid = Convert.ToInt32(Request.Params["ID"]);
                QuotaName = Request.Params["QuotaName"];

                string projectId = SurveyRepository.GetById(SurveySid).Name;
                var quotaList = QuotaManager.GetQuotaList(projectId, QuotaName);
                Limit = quotaList.QuotaRows.First(x => CellIds.Contains(x.QuotaRowId)).Target;
            }
        }

        /// <summary>
        /// Handles the OK button click event.
        /// Updates the quota cells limit with the specified value via the web service.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                string projectId = SurveyRepository.GetById(SurveySid).Name;

                QuotaList quotaList = QuotaManager.GetQuotaList(projectId, QuotaName);

                foreach (QuotaRow row in quotaList.QuotaRows)
                {
                    if (CellIds.Contains(row.QuotaRowId))
                    {
                        row.Target = Limit;
                    }
                }

                var evt = new UpdateQuotaLimitsEvent(SurveySid, projectId, quotaList.QuotaId, quotaList.QuotaName, CellIds, Limit);

                QuotaManager.UpdateQuotaList(projectId, QuotaName, quotaList);

                QuotaManager.SynchronizeQuota(projectId, QuotaName);

                evt.Finish();

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                AddUserMessage(Strings.NoPermissionToChangeQuotaLimitErrorMessage, ex);
            }
        }
    }
}
