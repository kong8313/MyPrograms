using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Resources;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Classes
{
    /// <summary>
    /// Methods designed to work with predictive surveys.
    /// </summary>
    public class PredictiveHelper
    {
        private IPersonRepository _personRepository;
        private ISurveyRepository _surveyRepository;
        private ISurveyService _surveyService;

        public PredictiveHelper()
        {
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
            _surveyService = ServiceLocator.Resolve<ISurveyService>();
        }

        /// <summary>
        /// Gets sorted list of person names with task choice != "Survey Selection".
        /// </summary>
        /// <param name="personSids">The person sids.</param>
        private List<string> GetFormatedNonSurveySelectionPersonNamesList(IEnumerable<int> personSids)
        {
            List<string> result = new List<string>();
            foreach (int personSid in personSids)
            {
                BvPersonEntity person = _personRepository.GetById(personSid);
                if (person.ManualSelection != (int)AgentTaskChoiceMode.CampaignAssignment)
                {
                    result.Add(person.Name);
                }
            }

            result.Sort();
            return result;
        }

        /// <summary>
        /// Gets the formated sorted predictive survey names list. Name formated as "Preject name (Project ID)".
        /// </summary>
        /// <param name="surveySids">The survey sids.</param>
        private List<string> GetFormatedPredictiveSurveyNamesList(IEnumerable<int> surveySids)
        {
            List<string> result = new List<string>();
            foreach (int surveyId in surveySids)
            {
                if (_surveyService.GetDialingMode(surveyId) == DialingMode.Predictive)
                {
                    BvSurveyEntity survey = _surveyRepository.GetById(surveyId);
                    result.Add(String.Format("{0} ({1})", survey.Description, survey.Name));
                }
            }

            result.Sort();
            return result;
        }

        /// <summary>
        /// If isGroup=false - just returns sid parameter,
        /// otherwise - returns SIDs of all persons inside group with SID passed in and all its subgroups.
        /// </summary>
        /// <param name="sid">The SID of peron or group.</param>
        /// <param name="isGroup">if set to <c>true</c> SID passed in is a SID of group.</param>
        public static List<int> GetAllPersonSidsList(int sid, bool isGroup)
        {
            List<int> result = new List<int>();
            if (!isGroup)
            {
                result.Add(sid);
            }
            else
            {
                var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

                result.AddRange(PersonManager.GetAllPersons(sid, callCenterId).Select(x => x.Id));
            }

            return result;
        }

        /// <summary>
        /// Gets the warning text to be shown during assigment of interviewers with wrong task choice to predictive surveys.
        /// </summary>
        /// <param name="surveySids">The survey sids.</param>
        /// <param name="interviewerSids">The interviewer sids.</param>
        /// <returns>Warning message. Empty if warning should not be shown.</returns>
        public string GetPredictiveSurveyAssignmentWarning(IEnumerable<int> surveySids, IEnumerable<int> interviewerSids)
        {
            string result = null;
            List<string> personNames = GetFormatedNonSurveySelectionPersonNamesList(interviewerSids);
            if (personNames.Count > 0)
            {
                List<string> predictiveSurveyNames = GetFormatedPredictiveSurveyNamesList(surveySids);
                if (predictiveSurveyNames.Count > 0)
                {
                    result = String.Format(
                        Strings.WrongTaskChoiceDuringAssignmentOnPredictiveSurvey,
                        String.Join(", ", predictiveSurveyNames.ToArray()),
                        String.Join(", ", personNames.ToArray()));
                }
            }

            return result;
        }
    }
}