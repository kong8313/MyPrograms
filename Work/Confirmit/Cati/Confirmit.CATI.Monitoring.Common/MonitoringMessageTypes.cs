namespace Confirmit.CATI.Monitoring.Common
{
    /// <summary>
    /// Represents enumeration of monitoring messages.
    /// </summary>
    public enum MonitoringMessageTypes
    {
        /// <summary>
        /// Occurs when console received monitoring start notification.
        /// </summary>
        MonitoringInitialMessage = 17,

        /// <summary>
        /// Occurs when console received monitoring end notification.
        /// </summary>
        MonitoringEndMessage = 18,

        /// <summary>
        /// Occurs when appointment form is loaded.
        /// </summary>
        AppointmentFormInitialMessage = 1,

        /// <summary>
        /// Occurs when person name text is changed in appointment form.
        /// </summary>
        AppointmentFormNameChangedMessage = 2,

        /// <summary>
        /// Occurs when appointment date is changed in appointment form.
        /// </summary>
        AppointmentFormAppointmentDateChangedMessage = 3,

        /// <summary>
        /// Occurs when appointment time is changed in appointment form.
        /// </summary>
        AppointmentFormAppointmentTimeChangedMessage = 4,

        /// <summary>
        /// Occurs when expiration date is changed in appointment form.
        /// </summary>
        AppointmentFormExpirationDateChangedMessage = 5,

        /// <summary>
        /// Occurs when expiration time is changed in appointment form.
        /// </summary>
        AppointmentFormExpirationTimeChangedMessage = 6,

        /// <summary>
        /// Occurs when "Never expire" property is changed in appointment form.
        /// </summary>
        AppointmentFormExpireChangedMessage = 7,

        /// <summary>
        /// Occurs when "Log out" property is changed in appointment form.
        /// </summary>
        AppointmentFormLogoutChangedMessage = 9,

        /// <summary>
        /// Occurs when user closed appointment form.
        /// </summary>
        AppointmentFormCloseMessage = 10,

        /// <summary>
        /// Occurs when interview is started.
        /// </summary>
        InterviewStartMessage = 39,

        /// <summary>
        /// Occurs when interview is shown for first time.
        /// </summary>
        InterviewInitialMessage = 11,

        /// <summary>
        /// Occurs when interview is finished
        /// </summary>
        InterviewFinishMessage = 12,

        /// <summary>
        /// Occurs when new page is loaded or page is changed via ajax
        /// </summary>
        InterviewPageBrowserPageCompletedMessage = 13,

        /// <summary>
        /// Occured when new question is selected
        /// </summary>
        InterviewPageBrowserQuestionSelectedMessage = 14,

        /// <summary>
        /// Occurs when user enteres or changes answer for question 
        /// </summary>
        InterviewPageBrowserAnswerValueChangedMessage = 15,

        /// <summary>
        /// Occurs when value in KeyboardInputControl is chanded
        /// </summary>
        InterviewPageBrowserKeyboardInputControlValueChangedMessage = 16,

        /// <summary>
        /// Occurs when user press enter button and data from InputControl should be applied to current question
        /// </summary>
        InterviewPageBrowserProcessKeyboardInputMessage = 20,

        /// <summary>
        /// Occurs when openend review has started.
        /// </summary>
        InterviewReviewStartMessage = 24,

        /// <summary>
        /// Occurs when a page in the interview browser undergoes partial changes.
        /// </summary>
        InterviewPageBrowserPagePartiallyChangedMessage = 101,

        /// <summary>
        /// Occurs when user selectes Redo action.
        /// </summary>
        RedoQuestionMessage = 19,

        /// <summary>
        /// Occurs when user see survey/inteview selecting screen
        /// </summary>
        InterviewSelectingStartMessage = 25,

        /// <summary>
        /// Occurs when process of getting new interview is started
        /// </summary>
        GettingInterviewStartMessage = 26,

        /// <summary>
        /// Occurs when process of pending logout is started
        /// </summary>
        PendingLogoutStartMessage = 27,

        /// <summary>
        /// Occurs when interviewer logged out
        /// </summary>
        InterviewerLoggedOutMessage = 28,

        /// <summary>
        /// Occurs when user do pending logout
        /// </summary>
        PendingLogoutMessage = 29,

        /// <summary>
        /// Occurs when user set default answer
        /// </summary>        
        SetDefaultAnswerMessage = 30,

        /// <summary>
        /// Occurs when user set refused answer
        /// </summary>        
        SetRefusedAnswerMessage = 31,

        /// <summary>
        /// Occurs when user openes redo drop down list
        /// </summary>        
        RedoDropDownListOpenMessage = 32,

        /// <summary>
        /// Occurs when user openes redo drop down list
        /// </summary>        
        RedoDropDownListCloseMessage = 33,

        /// <summary>
        /// Occurs when user changes survey language
        /// </summary>        
        SelectedSurveyLanguageChangedMessage = 34,

        /// <summary>
        /// Occurs when interview termination confirmation dialog is shown
        /// </summary>                
        InterviewTerminateDialogShownMessage = 35,

        /// <summary>
        /// Occurs when interview termination confirmation dialog is closed
        /// </summary>                
        InterviewTerminateDialogCloseMessage = 36,

        /// <summary>
        /// Occurs when request to monitoring server is finished by security exception after some events were successfully received. 
        /// This messages can't be generated by Cati console and are used only inside Player.
        /// </summary>    
        MonitoringEndBySecurityExceptionMessage = 37,

        /// <summary>
        /// Occurs when request to monitoring server is finished by security exception and no events were successfully received. 
        /// This messages can't be generated by Cati console and are used only inside Player.
        /// </summary>                
        MonitoringEndBySecurityExceptionOnFirstRequestMessage = 38,

        /// <summary>
        /// Occurs when form 'All appointments' is shown 
        /// </summary>    
        AppointmentsListFormShownMessage = 41,

        /// <summary>
        /// Occurs when form 'All appointments' is closed 
        /// </summary>    
        AppointmentsListFormClosedMessage = 42,

        /// <summary>
        /// Occurs when form 'Check Spelling' is shown 
        /// </summary>    
        CheckSpellingFormShownMessage = 43,

        /// <summary>
        /// Occurs when form 'Check Spelling' is closed 
        /// </summary>    
        CheckSpellingFormClosedMessage = 44,

        /// <summary>
        /// Occurs when dialog 'OnABreak' is shown. 
        /// </summary>            
        OnABreakDialogShownMessage = 45,

        /// <summary>
        /// Occurs when dialog 'OnABreak' is closed. 
        /// </summary>            
        OnABreakDialogClosedMessage = 46,

        /// <summary>
        /// Occurs when process of pending break is started.
        /// </summary>
        PendingBreakMessage = 47,

        /// <summary>
        /// Occurs when we show blank control in order to hide any main window content.
        /// </summary>
        BlankControlShownMessage = 400,

        /// <summary>
        /// Occurs when we interview page browser control.
        /// </summary>
        InterviewPageBrowserShownMessage = 401,

        /// <summary>
        /// Occurs when dialing operation is started.
        /// </summary>
        DialStartMessage = 21,

        /// <summary>
        /// Occurs when hang up operation is started.
        /// </summary>
        HangupMessage = 22,

        /// <summary>
        /// Occurs when telephony operation is completed.
        /// </summary>
        TelephonyCompleteMessage = 23,

        /// <summary>
        /// Occurs at StartPlayback operation 
        /// </summary>
        StartPlaybackMessage = 51,

        /// <summary>
        /// Occurs at PauseOrResumePlayback operation 
        /// </summary>
        PauseOrResumePlaybackMessage = 52,

        /// <summary>
        /// Occurs at StopPlayback operation 
        /// </summary>
        StopPlaybackMessage = 53,

        /// <summary>
        /// Occurs at ToggleInterviewerListensToPlaybackOrRespondent operation
        /// </summary>
        ToggleInterviewerListensToPlaybackOrRespondentMessage = 54,

        /// <summary>
        /// Occurs when audio file should be played.
        /// </summary>
        AudioStartMessage = 40,

        /// <summary>
        /// Occurs when Redial Form is loaded
        /// </summary>
	    RedialFormInitialMessage = 60,

        /// <summary>
        /// Occurs when Dial button is pressed
        /// </summary>
        RedialFormDialCalledMessage = 61,

        /// <summary>
        /// Occurs when redial is changed to redial default number or new number
        /// </summary>
        RedialFormRedialTypeChangedMessage = 62,

        /// <summary>
        /// Occurs when user enters new number to dial
        /// </summary>
        RedialFormDialNumberChangedMessage = 63,

        /// <summary>
        /// Occurs when call outcome is received
        /// </summary>
        RedialFormTelephonyResultMessage = 64,

        /// <summary>
        /// Occurs when Redial Form is closed
        /// </summary>
        RedialFormCloseMessage = 65,

        /// <summary>
        /// Occurs when we use InitialQuestion and need to jump to it
        /// </summary>
        ReturnToInitialQuestion = 66,

        /// <summary>
        /// Occurs when "Allow appointments outside of permitter shift" property is changed in appointment form.
        /// </summary>
	    AppointmentFormAllowAppointmentsOutsideShiftChangedMessage = 67,

        /// <summary>
        /// Occurs when user press "Cancel" during dialing process in redial form.
        /// </summary>
	    RedialFormHangupDialCalledMessage = 68,

        /// <summary>
        /// Occurs when user enteres or changes answer for question in responsive layout
        /// </summary>
        InterviewPageBrowserResponsiveQuestionChangedMessage = 70,

        /// <summary>
        /// Occurs when Internal Call Transfer Form is shown
        /// </summary>
        InternalCallTransferFormShownMessage = 71,

        /// <summary>
        /// Occurs when Internal Call Transfer Form is closed
        /// </summary>
        InternalCallTransferFormClosedMessage = 72,

        /// <summary>
        /// Occurs when External Call Transfer Form7 is shown
        /// </summary>
        ExternalCallTransferFormShownMessage = 73,

        /// <summary>
        /// Occurs when External Call Transfer Form is closed
        /// </summary>
        ExternalCallTransferFormClosedMessage = 74,

        /// <summary>
        /// Occurs when there is no explicit consent for live monitoring
        /// </summary>
        MonitoringNotPermittedMessage = 75,

        /// <summary>
        /// Occurs when audio playback should stopped
        /// </summary>
        AudioStopMessage = 76,

        /// <summary>
        /// Occurs when console executes some activity which isn't supported monitoring.
        /// </summary>
        ShowInformationMessage = 77,

        /// <summary>
        /// Reserved for browser based interface.
        /// </summary>
        StopAudioPlayback = 1000,
        /// <summary>
        /// Occurs when searchable question is scrolled, only for browser based interface.
        /// </summary>
        ResponsiveQuestionScrolled = 1001,

        /// <summary>
        /// Occurs when interview hangup confirmation dialog is shown
        /// </summary>
        InterviewHangupDialogShownMessage = 1002,

        /// <summary>
        /// Occurs when interview hangup confirmation dialog is closed
        /// </summary>
        InterviewHangupDialogCloseMessage = 1003,

        /// <summary>
        /// Occurs when appointment time zone is changed in appointment form.
        /// </summary>
        AppointmentFormTimezoneChangedMessage = 1006,

        /// <summary>
        /// Occurs when appointment interviewer date is changed in appointment form.
        /// </summary>
        AppointmentFormInterviewerDateChangedMessage = 1007
    }

    public static class MonitoringMessageTypesExtention
    {
        public static bool IsFinishedMessage(this MonitoringMessageTypes mmt)
        {
            return (mmt == MonitoringMessageTypes.InterviewFinishMessage ||
                    mmt == MonitoringMessageTypes.MonitoringEndMessage);
        }
    }
}
