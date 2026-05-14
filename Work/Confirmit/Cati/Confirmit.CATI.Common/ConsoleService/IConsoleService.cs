using System;
using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Common.ConsoleService
{
    [ServiceContract(Name = "ConsoleService", Namespace = "http://www.confirmit.com/ConsoleService/04/24/2009")]
    public interface IConsoleService
    {
        /// <summary>
        /// Login to CATIConsoleWebServ.
        /// </summary>
        /// <returns>
        /// <param name="catiConsoleProperties">The properties required for CATI console</param>
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(NotSupportedOsExceptionDetails))]
        [FaultContract(typeof(UserAlreadyLoggedInExceptionDetails))]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void Login(
            string stationId,
            ConsoleDescription consoleDescription,
            out PersonInfo personInfo,
            out DiallerInfo diallerInfo,
            out CatiConsolePropertiesContainer catiConsoleProperties);

        /// <summary>
        /// The method initiates an interviewer login to the dialer.
        /// CATI console calls this method if there is a dialer in the system.
        /// </summary>
        /// <param name="extensionNumber">The interviewer extension phone number.</param>
        /// <param name="surveyId">Survey Id can be null or empty for AUTOMATIC users.</param>
        /// <param name="isPredictive"></param>
        /// <remarks>
        /// Confirmit surveyId like pNNNNNNN is used.
        /// Login to dialer operation is asynchronous. CATI console must call GetState method 
        /// to find out if login to MN dialer has finished.
        /// </remarks>
        [OperationContract]
        [FaultContract(typeof(SurveyInManualDialingModeExceptionDetails))]
        [FaultContract(typeof(ManualUserInPredictiveModeExceptionDetails))]
        [FaultContract(typeof(LoginToInactiveDialerExceptionDetails))]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void LoginToDialer(string extensionNumber, string surveyId, out bool isPredictive);

        /// <summary>
        /// Returns the mode of the logged in person.
        /// Throws "ThePersonIsNotLoggedIn" exception if there is no logged in person.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        int GetPersonMode();

        /// <summary>
        /// Notifies Fusion if the logged in person wants to logout.
        /// If the person has no active interviews then the function initiates logout process.
        /// </summary>
        /// <param name="logout">true if the person wants to logout, false if the person wants to continue his/her session</param>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void SetPendingLogout(bool logout);

        /// <summary>
        /// CATI console calls the method at the end of the interviewer session.
        /// CATI console must call it if and only if the interviewer login state becomes NOT_LOGGED_IN
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void ConfirmLogout();

        //////////////////////////////////////////////////////////////////////////
        //Call delivery level methods 

        /// <summary>
        /// It is the request to start an interview for the logged in person.
        /// </summary>
        /// <remarks>
        /// The function is used for persons in any mode: AUTOMATIC, SURVEY_ASSIGNMENT or MANUAL.
        /// if <param name="surveyId"></param> == null and <param name="interviewId"></param> == 0
        /// then CATIConsoleWebService considers the user is in AUTOMATIC mode.
        /// if <param name="surveyId"></param> != null and <param name="interviewId"></param> == 0
        /// then CATIConsoleWebService considers the user is in SURVEY_ASSIGNMENT mode.
        /// if <param name="surveyId"></param> != null and <param name="interviewId"></param> != 0
        /// then CATIConsoleWebService considers the user is in MANUAL mode.
        /// 
        /// Confirmit surveyId like pNNNNNNN is used.
        /// 
        /// CATI console uses GetState method in order to obtain 
        /// startup interview parameters and interviewer/interview state.
        /// </remarks>
        /// <returns>
        /// true if the system successfully started searching for an interview for the interviewer,
        /// or if there is no need to search for a new interview.
        /// false otherwise.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ManualUserInPredictiveModeExceptionDetails))]
        [FaultContract(typeof(PredictiveSurveyWithoutDialerExceptionDetails))]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        bool StartInterview(string surveyId, int interviewId);

        /// <summary>
        /// It is the request to create a new interview for the logged in person.
        /// Can be used only in MANUAL mode
        /// </summary>
        /// <param name="surveyId">Survey id like pNNNNNNN</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        [FaultContract(typeof(CreateNewInterviewExceptionDetails))]
        int CreateNewInterview(string surveyId);

        /// <summary>
        /// Returns the array of opened surveys available for the logged in person.
        /// The function is used for persons in MANUAL and SURVEY_ASSIGNMENT modes.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        Survey[] GetOpenedSurveys();

        /// <summary>
        /// Returns the list of available interviews for a survey.
        /// The function is used for persons in MANUAL mode.
        /// </summary>
        /// <returns>
        /// DataTable
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        DataTable GetSurveyInterviews(string surveyId, SearchParameter[] parameters);

        /// <summary>
        /// Returns the array of spell errors.
        /// </summary>
        /// <param name="languageId">Language identifier for current text block.</param>
        /// <param name="textBlock">Text block for check spelling.</param>
        [OperationContract]        
        [FaultContract(typeof(SpellCheckerLanguageIsNotSupportedExceptionDetails))]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        SpellError[] CheckTextSpelling(int languageId, string textBlock);

        //////////////////////////////////////////////////////////////////////////
        // Appointments methods 

        /// <summary>
        /// Returns the list of available appointments for a concrete interview.
        /// </summary>
        /// <returns>
        /// The array of <seealso cref="Appointment"/>.
        /// </returns>
        /// <remarks>
        /// CATI console calls the function only while an interview is in progress.
        /// </remarks>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        Appointment[] GetInterviewAppointmentList(string surveyId, int interviewId);

        [OperationContract]
        [FaultContract(typeof (UserMessageExceptionDetails))]
        Timezone GetInterviewTimezone(string surveyId, int interviewId);

        /// <summary>
        /// Sets an appointment list for a definite interview.
        /// (All previously set appointments for the interview are being deleted).
        /// </summary>
        /// <remarks>
        /// CATI console calls the function only while an interview is in progress.
        /// </remarks>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void SetInterviewAppointmentList(string surveyId, int interviewId, Appointment[] appointments, bool allowOutsideShift);

        /// <summary>
        /// Returns the list of all available appointments.
        /// </summary>
        /// <returns>
        /// The array of <seealso cref="Appointment"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        Appointment[] GetAllAppointmentList();

        /// <summary>
        /// Returns the list of all available messages for current interviewer
        /// </summary>
        /// <returns>
        /// The array of <seealso cref="Message"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        Messages[] GetMessages();

        /// <summary>
        /// If openend review is available for the survey the interviewer currently works on then
        /// the function switches the interviewer into openend review mode and returns true.
        /// The function returns false otherwise.
        /// </summary>
        /// <param name="attemptNumber"></param>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        bool GetForceOpenendReview(int attemptNumber);

        //////////////////////////////////////////////////////////////////////////
        //TCI support

        /// <summary>
        /// The method initiates dial process.
        /// CATI console can call this method only if there is a dialler in the system.
        /// </summary>
        /// <param name="phoneNumber"> The respondent telephone number </param>
        /// <param name="initiator"> 0 - script, 1 - telephone menu </param>
        /// <param name="attemptNumber"></param>
        /// <returns>
        /// true if dialling process was successfully started, false otherwise.
        /// Note: It now always returns true, CATI console is now being notified about errors via TELEPHONY_ERROR.
        /// </returns>
        /// <remarks>
        /// Dial operation is asynchronous. CATI console must call GetState method 
        /// to find out if dialling process has finished.
        /// </remarks>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]        
        void Dial(string phoneNumber, int initiator, int attemptNumber);

        /// <summary>
        /// The method cancels the dial process.
        /// CATI console can call this method only if there is a dialler in the system.
        /// </summary>
        /// <returns>
        /// true if cancel dialling process was successfully started, false otherwise.
        /// </returns>
        /// <remarks>
        /// CancelDialing operation is asynchronous. CATI console must call GetState method 
        /// to find out if dialling process has finished.
        /// 
        /// !The function is not supported for the moment. It is reserved for future needs.
        /// </remarks>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void CancelDialing();

        /// <summary>
        /// Respondent hangup.
        /// CATI console can call this method only if there is a dialler in the system.
        /// </summary>
        /// <param name="initiator"> 0 - script, 1 - telephone menu </param>
        /// <returns>
        /// true if hangup succeeded, false if an error occured during hangup.
        /// </returns>
        /// <remarks>
        /// We suppose Hangup operation is synchronous.
        /// </remarks>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        bool Hangup(int initiator);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TransferResource"></param>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void TransferStart(TransferOptions TransferResource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferConnectionState"></param>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void TransferSetConnectionState(TransferConnectionState transferConnectionState);

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void TransferComplete();

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void TransferCancel();

        /// <summary>
        /// CATI console must call this method at the moment when an interview is finished.
        /// </summary>
        /// <param name="lookUpForNewCalls">If set to False the system will not look up for new calls.</param>
        /// <param name="attemptNumber"></param>
        /// <param name="interviewId"></param>
        /// <param name="details"></param>
        /// <param name="transferComplete"></param>
        [OperationContract]
        [FaultContract(typeof (UserMessageExceptionDetails))]
        void WrapUp(int interviewId, bool lookUpForNewCalls, int attemptNumber, CompletedInterviewDetails details);

        /// <summary>
        /// Start playing a voice file
        /// </summary>
        /// <param name="soundFileName"></param>
        /// <param name="timeOfPlayingInSeconds"></param>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void StartPlayback(string soundFileName, out int timeOfPlayingInSeconds);

        /// <summary>
        /// Stop play voice file
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void StopPlayback();

        /// <summary>
        /// Pause or Resume the voice file currently played
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void PauseOrResumePlayback();

        /// <summary>
        /// Switch agent from hear respondent to hear playing and back
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void ToggleInterviewerListensToPlaybackOrRespondent();

        /// <summary>
        /// Updates person mode        
        /// </summary>
        /// <remarks>
        /// Updates bvPerson table
        /// </remarks>
        /// <param name="personMode">New PersonMode</param>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void UpdatePersonMode(int personMode);

        /// <summary>
        /// This method notifies BE if the logged in person wants to break. If the person has no active interviews then break status is applied immediately. 
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        bool SetPendingBreakStatus(PendingBreakStatus status, int? breakType);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void ContinueWorkAfterBreak(int attemptNumber);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        LanguageCollection GetSurveyLanguages(string projectId);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        QuestionHistoryCollection GetInterviewHistory(string projectId, string respondentIdentity, int languageId);

        [OperationContract]
        [FaultContract(typeof (UserMessageExceptionDetails))]
        Guid GenerateAuthenticationKey();

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void TerminateTask();

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        InternalTransferTarget[] GetInternalTransferTargets();

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        ExternalTransferTarget[] GetExternalTransferTargets();

    }

    public enum FilterFields
    {
        [EnumMember]
        RESPONDENTNAME = 1,
        [EnumMember]
        TELEPHONENUMBER = 2,
        [EnumMember]
        ID = 3
    }

    public enum TimezoneFields
    {
        NAME = 1,
        BIAS = 2,
        STANDARDNAME = 4,
        STANDARDDATE = 5,
        STANDARDDAYOFWEEK = 6,
        STANDARDBIAS = 7,
        DAYLIGHTNAME = 8,
        DAYLIGHTDATE = 9,
        DAYLIGHTDAYOFWEEK = 10,
        DAYLIGHTBIAS = 11,
        ID = 12
    }

    /// <summary>
    /// class of KeepAlive method result
    /// </summary>
    public struct KeepAliveResult
    {
        ///<summary>
        /// True if monitored, false otherwise.
        ///</summary>
        public bool m_isMonitored;

        /// <summary>
        /// FusionEnterprise security identifier of the interviewer.
        /// </summary>
        public int m_interviwerSID;

        public long m_monitoringSessionID;

        ///<summary>
        /// True means that there is new message for user. 
        ///</summary>
        public bool m_NewMessage;
    }
}
