using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class ChangeAutomaticSurvey : BaseForm
    {
        [StoreInViewState]
        protected IList<int> Sids;

        [StoreInViewState]
        protected bool IsGroup;

        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
        private readonly IPersonRepository _personRepository = ServiceLocator.Resolve<IPersonRepository>();
        private readonly IChangeAutomaticSurveyService _changeAutomaticSurveyService = ServiceLocator.Resolve<IChangeAutomaticSurveyService>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ServiceLocator.Resolve<IToggleSettings>().EnableSeamlessSurveySwitching == false)
            {
                throw new Exception(Strings.ActionIsNotAllowed);
            }

            if (!IsPostBack)
            {
                Sids = Request["ObjectSid"].Split(',').Select(Int32.Parse).ToList();
                IsGroup = Boolean.Parse(Request["IsGroup"]);
            }

            ((GeneralGrid)m_SurveyList.Controls[0]).HintText = Strings.ChangeAutomaticSurvey_Hint;
        }

        protected void SelectButtonClick(object sender, EventArgs e)
        {
            if (!m_SurveyList.SelectedSurveyId.HasValue)
            {
                ShowClientMessage(Strings.Err_NoSurveyWasSpecified);
                return;
            }

            var id = m_SurveyList.SelectedSurveyId.Value;
            var projectId = SurveyRepository.GetById(id).Name;
            var notSuccessfulUserIds = new List<int>();
            foreach (var sid in Sids)
            {
                if (IsGroup)
                {
                    var persons = PersonGroupService.GetChildPersons(sid, _callCenterProvider.GetCurrentId());
                    foreach (var person in persons)
                    {
                        if (!ChangeSeamless(person.SID.Value, id, projectId))
                            notSuccessfulUserIds.Add(person.SID.Value);
                    }
                }
                else
                {
                    if (!ChangeSeamless(sid, id, projectId))
                        notSuccessfulUserIds.Add(sid);
                }
            }

            if (notSuccessfulUserIds.Any())
            {
                var warnUsers = _personRepository.GetAll()
                    .Where(p => notSuccessfulUserIds.Contains(p.SID))
                    .Select(x => String.Format("{0}({1})", x.Name, x.SID));

                ShowClientMessage(
                    String.Format(Strings.Wrn_SeamlessAutomaticSurveySwitch, projectId, String.Join(", ", warnUsers)), true);
            }

            CloseOverlay(true);
        }

        private bool ChangeSeamless(int personId, int surveyId, string projectId)
        {
            try
            {
                return _changeAutomaticSurveyService.ChangeSeamless(personId, surveyId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("ChangeAutomaticSurvey(personId={0}, surveyId={1}, projectId={2}) is failed:{3}", personId, surveyId, projectId, ex);
            }

            return false;
        }
    }
}