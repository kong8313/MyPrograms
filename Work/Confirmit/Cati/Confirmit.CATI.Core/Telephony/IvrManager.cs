using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public class IvrManager
    {
        private readonly ITaskRepository _taskRepository;

        public IvrManager()
        {
            _taskRepository = ServiceLocator.Resolve<ITaskRepository>();
        }
        /// <summary>
        /// Transfers interview to IVR.
        /// </summary>
        /// <param name="projectId">The project ID (pXXXXXXX).</param>
        /// <param name="interviewId"></param>
        /// <param name="endpoint"></param>
        /// <param name="attributes"></param>
        public void TransferToIvr(string projectId, int interviewId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes)
        {
            var survey = SurveyRepository.GetByName(projectId);

            //var evt = new TransferToIvrEvent(survey.SID, projectId, interviewId, attributes);

            try
            {
                var task = _taskRepository.GetByIdWithCheck(survey.SID, interviewId);

                var personEntity = PersonRepository.GetById(task.PersonSID);

                if (!BvCallHandlerRoot.IsLoggedInToDialer(task))
                {
                    Trace.TraceWarning(
                        "IvrManager.TransferToIvr: " +
                        "Person [{0}({1})] is not logged in to dialer. TransferToIvr is not called on dialer [{2}] /// " +
                        "projectId={3}, surveyId={4}, interviewId={5}, endpoint={6}, attributes=[{7}]",
                        personEntity.Name,
                        personEntity.SID,
                        task.DialerId,
                        projectId,
                        survey.SID,
                        interviewId,
                        endpoint,
                        attributes.Aggregate("", (current, attribute) => current + attribute.ToString()));

                    return;
                }

                var telephony = ServiceLocator.Resolve<ITelephony>();

                var result = telephony.TransferToIvr(
                    task.DialerId,
                    survey.CampaignId,
                    task.PersonSID.ToString(),
                    interviewId,
                    task.CallID.GetValueOrDefault(),
                    endpoint,
                    attributes);

                if (result != DialerErrorCode.Success)
                {
                    throw new InternalErrorException(string.Format(
                        "IvrManager.TransferToIvr: failed. /// Error code={0}, dialerId={1}, person [{2}({3})], " +
                        "projectId={4}, surveyId={5}, interviewId={6}, endpoint={7}, attributes=[{8}]",
                        result,
                        task.DialerId,
                        personEntity.Name,
                        personEntity.SID,
                        projectId,
                        survey.SID,
                        interviewId,
                        endpoint,
                        attributes.Aggregate("", (current, attribute) => current + attribute.ToString())));
                }
            }
            finally
            {
                //evt.Finish();
            }
        }
    }
}