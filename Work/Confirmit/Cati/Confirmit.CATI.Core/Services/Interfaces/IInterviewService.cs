using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IInterviewService
    {
        void AddAppointments(int surveySid, int interviewId, int batchId, Appointment[] appointments, bool allowOutsideShift);

        void DeleteRespondents(int surveySID, int[] respondentIDs, CancellationToken cancellationToken);

        BvInterviewWithOriginEntity AddRespondent(BvSurveyEntity survey, int respondentId, int its, OperationType operationType, Role role, int? personSid = null);
        BvInterviewWithOriginEntity AddRespondent(BvSurveyEntity survey, int respondentId, SchedulingScriptExecutionOptions options);

        void BindDialerIdToInterview(int surveyId, int interviewId, int dialerId);

        void BindDialerIdToInterview(BvInterviewEntity interview, int dialerId);

        string GenereteSecurityKey(BvInterviewEntity interview);

        /// <summary>
        /// Gets the interview respondent timezone ID for the specified survey SID and interview ID
        ///  or local company timezone if it is not specified in the interview.
        /// </summary>
        /// <param name="interview">The interview.</param>
        /// <returns>Respondent timezone ID</returns>
        int GetInterviewTimezoneOrDefault(BvInterviewEntity interview);

        /// <summary>
        /// Gets the interview respondent timezone ID for the specified survey SID and interview ID
        ///  or local company timezone if it is not specified in the interview.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <param name="interviewId">The interview ID.</param>
        /// <returns>Respondent timezone ID</returns>
        int GetInterviewTimezoneOrDefault(int surveySid, int interviewId);
        int[] GetInterviewIdsWithoutRespondents(int surveyId);
    }
}
