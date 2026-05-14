using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class ChangeLimitForMultipleQuotas : BaseForm
    {
        private Dictionary<string, List<int>> SelectedQuotasCellIds
        {
            get
            {
                var result = new Dictionary<string, List<int>>();

                foreach (var quota in Quotas)
                {
                    result[quota] = (List<int>)ViewState[quota] ?? new List<int>();
                }

                return result;
            }
        }

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

        [StoreInViewState]
        protected int SurveySid;

        [StoreInViewState]
        protected List<string> Quotas;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Quotas = Request.Params["Quotas"].Split(',').ToList();
                SurveySid = Convert.ToInt32(Request.Params["ID"]);

                foreach (var quota in Quotas)
                {
                    ViewState[quota] = Request.Params[quota].Split(',').Select(x => int.Parse(x)).ToList();
                }

                var firstTargetQuota = SelectedQuotasCellIds.First();

                string projectId = SurveyRepository.GetById(SurveySid).Name;
                var quotaList = QuotaManager.GetQuotaList(projectId, firstTargetQuota.Key);
                Limit = quotaList.QuotaRows.First(x => firstTargetQuota.Value.Contains(x.QuotaRowId)).Target;

                var selectedQuotas = Convert.ToInt32(Request.Params["selectedQuotas"]);
                var selectedCells = Convert.ToInt32(Request.Params["selectedCells"]);

                lblInfo.Text = string.Format(Strings.ChangeLimitInfoMessage, selectedQuotas, selectedCells);

                Hint.Text = Strings.ChangeLimitHintMessage;
            }
        }

        private string ConcatQuotas(List<string> quotas)
        {
            if (quotas.Count == 0)
            {
                return Strings.AllQuotasChangeLimitNoQuotas;
            }

            var concatedElements = new StringBuilder();

            for (int i = 0; i < quotas.Count; i++)
            {
                concatedElements.Append(quotas[i]);

                if (i < quotas.Count - 1)
                {
                    concatedElements.Append(", ");
                }
            }

            return concatedElements.ToString();
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            var done = new List<string>();
            var notUpdated = new List<string>();
            var notSynced = new List<string>();
            
            try
            {
                var projectId = SurveyRepository.GetById(SurveySid).Name;

                foreach (var selectedQuotasCellId in SelectedQuotasCellIds)
                {
                    QuotaList quotaList = QuotaManager.GetQuotaList(projectId, selectedQuotasCellId.Key);

                    foreach (QuotaRow row in quotaList.QuotaRows)
                    {
                        if (selectedQuotasCellId.Value.Contains(row.QuotaRowId))
                        {
                            row.Target = Limit;
                        }
                    }

                    try
                    {
                        var updateEvent = new UpdateQuotaLimitsEvent(SurveySid, projectId, quotaList.QuotaId,
                        quotaList.QuotaName, selectedQuotasCellId.Value, Limit);
                        QuotaManager.UpdateQuotaList(projectId, selectedQuotasCellId.Key, quotaList);
                        updateEvent.Finish();
                    }
                    catch (Exception ex)
                    {
                        Context.AddError(ex);
                        notUpdated.Add(selectedQuotasCellId.Key);
                        continue;
                    }

                    try
                    {
                        var synchronizeEvent = new SynchronizeQuotaEvent(SurveySid, projectId, selectedQuotasCellId.Key);
                        QuotaManager.SynchronizeQuota(projectId, selectedQuotasCellId.Key);
                        synchronizeEvent.Finish();
                    }
                    catch (Exception ex)
                    {
                        Context.AddError(ex);
                        notSynced.Add(selectedQuotasCellId.Key);
                        continue;
                    }

                    done.Add(selectedQuotasCellId.Key);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
            finally
            {
                if (done.Count != SelectedQuotasCellIds.Count)
                {
                    var message = string.Format(
                        Strings.AllQuotasChangeLimitErrorMessage,
                        ConcatQuotas(done),
                        ConcatQuotas(notSynced),
                        ConcatQuotas(notUpdated));

                    CloseOverlay(true, message);
                }
                else
                {
                    CloseOverlay(true);
                }

            }
        }
    }
}
