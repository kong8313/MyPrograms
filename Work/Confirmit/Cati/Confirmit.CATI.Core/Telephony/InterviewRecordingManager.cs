using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public class InterviewRecordingManager : IInterviewRecordingManager
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITelephony _telephony;
        private readonly IMnTciTools _mnTciTools;
        private readonly IDeferredMonitoringRepository _deferredMonitoringRepository;

        public InterviewRecordingManager(
            ITaskRepository taskRepository,
            ITelephony telephony,
            IMnTciTools mnTciTools, 
            IDeferredMonitoringRepository deferredMonitoringRepository)
        {
            _taskRepository = taskRepository;
            _telephony = telephony;
            _mnTciTools = mnTciTools;
            _deferredMonitoringRepository = deferredMonitoringRepository;
        }

        /// <summary>
        /// Starts open-end or sectional audio recording of the interview.
        /// </summary>
        /// <param name="projectId">The project ID (pXXXXXXX).</param>
        /// <param name="interviewId">The ID of currently active interview to start recording of.</param>
        /// <param name="label">The label. It will be included in the name of the recorded audio file.</param>
        public void StartRecording(string projectId, int interviewId, string label)
        {
            var survey = SurveyRepository.GetByName(projectId);

            var warning = string.IsNullOrEmpty(label) ? "WARNING: label is empty" : null;

            var evt = new StartAudioRecordingEvent(survey.SID, projectId, interviewId, label, warning);

            var task = _taskRepository.GetByIdWithCheck(survey.SID, interviewId);
            evt.InterviewerSid = task.PersonSID;

            if (!BvCallHandlerRoot.IsLoggedInToDialer(task))
            {
                Trace.TraceWarning(
                    "InterviewRecordingManager.StartRecording: " +
                    "survey name = {0}, interviewId = {1}, label = {2}: " +
                    "StartRecording is not called on dialer because person " +
                    "(person Id = {3}) is not logged in to dialer.",
                    projectId,
                    interviewId,
                    label,
                    task.PersonSID);
                return;
            }

            var result = _telephony.StartRecording(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID.ToString(),
                interviewId,
                task.CallID.GetValueOrDefault(),
                label);

            if (result != DialerErrorCode.Success)
            {
                throw new InternalErrorException(string.Format(
                    "InterviewRecordingManager.StartRecording failed. /// Error code={0}, dialerId={1}",
                    result,
                    task.DialerId));
            }

            var monitoringRecords = _deferredMonitoringRepository.TryGetByInterviewId(survey.SID, interviewId);
            var recordToUpdate = monitoringRecords
                .Where(r => !r.IsComplete && r.IsRecording)
                .OrderByDescending(r => r.ID)
                .FirstOrDefault();
            if (recordToUpdate != null)
            {
                BvPersonDeferredMonitoringAdapterEx.UpdateHasAudioAndRequestAudio(recordToUpdate.ID, true, true);
            }

            evt.Save();
        }

        /// <summary>
        /// Stops interview recording.
        /// </summary>
        /// <param name="surveyName">Survey name (project ID)</param>
        /// <param name="interviewId">Interview ID</param>
        /// <param name="stopRecordingMode">
        /// StopRecordingMode: stop whole interview recording, or sectional or both?
        /// </param>
        public void StopRecording(string surveyName, int interviewId, string stopRecordingMode)
        {
            var survey = SurveyRepository.GetByName(surveyName);

            var evt = new StopAudioRecordingEvent(survey.SID, surveyName, interviewId, stopRecordingMode);

            var task = _taskRepository.GetByIdWithCheck(survey.SID, interviewId);
            evt.InterviewerSid = task.PersonSID;

            if (!BvCallHandlerRoot.IsLoggedInToDialer(task))
            {
                Trace.TraceWarning(
                    "InterviewRecordingManager.StopRecording: " +
                    "surveyName = {0}, interviewId = {1}, stopRecordingMode = {2}: " +
                    "StopRecording is not called on dialer because person " +
                    "(person Id = {3}) is not logged in to dialer.",
                    surveyName,
                    interviewId,
                    stopRecordingMode,
                    task.PersonSID);
                return;
            }

            StopRecordingMode typedStopRecordingMode;

            if (!ParseStopRecordingMode(stopRecordingMode, out typedStopRecordingMode))
            {
                evt.Details.Warning = string.IsNullOrEmpty(stopRecordingMode)
                    ? "WARNING: stop voice recording mode is empty, default value is 'Both'"
                    : "WARNING: stop voice recording mode is incorrect, default value is 'Both'";

                Trace.TraceError(
                    "InterviewRecordingManager.StopRecording: " +
                    "surveyName = {0}, interviewId = {1}: " +
                    "Unknown StopRecordingMode '{2}', StopRecordingMode.Both will be used.",
                    surveyName,
                    interviewId,
                    stopRecordingMode);
            }

            var result = _telephony.StopRecording(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID.ToString(),
                interviewId,
                task.CallID.GetValueOrDefault(), typedStopRecordingMode);

            if (result != DialerErrorCode.Success &&
                result != DialerErrorCode.WrongAgentState /* Trying to stop sectional recording that was not started */)
            {
                throw new InternalErrorException(string.Format(
                    "InterviewRecordingManager.StopRecording failed. /// Error code={0}, dialerId={1}",
                    result,
                    task.DialerId));
            }

            evt.Save();
        }

        public bool ParseStopRecordingMode(string strStopRecordingMode, out StopRecordingMode typedStopRecordingMode)
        {
            var isParseSucceeded = Enum.TryParse(strStopRecordingMode, true, out typedStopRecordingMode);

            if (!isParseSucceeded)
            {
                typedStopRecordingMode = StopRecordingMode.Both; //Use Both by default
            }

            var isDefinedInEnum = Enum.IsDefined(typeof(StopRecordingMode), typedStopRecordingMode);
            if (!isDefinedInEnum)
            {
                typedStopRecordingMode = StopRecordingMode.Both; //Use Both by default
            }

            return isParseSucceeded && isDefinedInEnum;
        }

        /// <summary>
        /// Gets the list of boolean flags indicating whether there are some recordings are available for the specific interview ID.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <param name="interviewIds">The list of interview IDs to determine whether recordings are available for.</param>
        /// <returns>
        /// The list of boolean flags. Flags count is always equal to the count of interview IDs list.
        /// </returns>
        public bool[] AreRecordsExists(int surveySid, int[] interviewIds)
        {
            if (surveySid <= 0)
            {
                throw new ArgumentOutOfRangeException("surveySid");
            }

            if (!_mnTciTools.DoesCompanyUseTelephony())
            {
                return Enumerable.Repeat(false, interviewIds.Count()).ToArray();
            }

            try
            {
                return _telephony.AreRecordsExists(surveySid, interviewIds);
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                        "BvCallHandlerRoot.AreRecordsExists failure: " +
                        "surveySid={0}, interviewIds=[{1}], campaignId={2}, ex ={3}",
                        surveySid,
                        string.Join(", ", interviewIds.Select(s => s.ToString()).ToArray()),
                        BackendInstance.Current.CompanyId,
                        ex);
                return Enumerable.Repeat(false, interviewIds.Count()).ToArray();
            }
        }

        /// <summary>
        /// Gets the list of URLs to interview recordings by interview ID.
        /// </summary>
        /// <param name="surveySid">The surveySID.</param>
        /// <param name="interviewId">The interview ID to get recordings for.</param>
        /// <returns>Enumeration of interview recordings</returns>
        /// <exception cref="ArgumentOutOfRangeException"><c>callId</c> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><c>surveySid</c> is out of range.</exception>
        /// <exception cref="ArgumentException">Invalid ProjectID</exception>
        public IEnumerable<AudioRecordInfo> GetInterviewRecordings(int surveySid, int interviewId)
        {
            if (surveySid <= 0)
            {
                throw new ArgumentOutOfRangeException("surveySid", surveySid, "must be (> 0)");
            }

            if (interviewId <= 0)
            {
                throw new ArgumentOutOfRangeException("interviewId", interviewId, "must be (> 0)");
            }

            if (!_mnTciTools.DoesCompanyUseTelephony())
            {
                return (new AudioRecordInfo[0]).ToList();
            }

            try
            {
                return RemoveDuplicateRecords(_telephony.GetAudioRecords(surveySid, interviewId));
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                        "BvCallHandlerRoot.GetInterviewRecordings.failure: " +
                        "surveySid={0}, interviewId={1}, CompanyId={2}, ex ={3}",
                        surveySid,
                        interviewId,
                        BackendInstance.Current.CompanyId,
                        ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the list of URLs to interview recordings by interview ID.
        /// </summary>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="audioUrl">Url to the audio file which was returned by GetAudioRecords method</param>
        /// <returns>An object with the content of audio file</returns>
        public AudioFile GetAudioFile(int dialerId, string audioUrl)
        {
            if (dialerId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dialerId), dialerId, "must be (> 0)");
            }

            if (string.IsNullOrEmpty(audioUrl))
            {
                throw new ArgumentOutOfRangeException(nameof(audioUrl), audioUrl, "must be filled");
            }

            if (!Uri.TryCreate(audioUrl, UriKind.Absolute, out _))
            {
                throw new ArgumentException("audioUrl parameter is not a URL");
            }

            if (!_mnTciTools.DoesCompanyUseTelephony())
            {
                return new AudioFile();
            }

            try
            {
                return _telephony.GetAudioFile(dialerId, audioUrl);
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    "BvCallHandlerRoot.GetAudioFile.failure: " +
                    "dialerId={0}, audioUrl={1}, CompanyId={2}, ex={3}",
                    dialerId,
                    audioUrl,
                    BackendInstance.Current.CompanyId,
                    ex);
                throw;
            }
        }
        
        private IEnumerable<AudioRecordInfo> RemoveDuplicateRecords(IEnumerable<AudioRecordInfo> audioRecords)
        {
            if (audioRecords == null || !audioRecords.Any()) return new List<AudioRecordInfo>();

            return audioRecords.GroupBy(x => x.DateTime + x.Url).Select(x => x.First());
        }
    }
}
