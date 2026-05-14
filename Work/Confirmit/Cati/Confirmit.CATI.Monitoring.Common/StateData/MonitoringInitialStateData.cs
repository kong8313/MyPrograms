using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents initial state data of Cati Interviewer Console as a whole. 
    /// Contains state of all monitorable controls of application.
    /// </summary>
    [Serializable]
    public class MonitoringInitialStateData : BaseStateData
    {
        #region Constructors

        /// <summary>
        /// Initializes new instance of MonitoringInitialStateData class.
        /// </summary>
        public MonitoringInitialStateData()
            : base()
        {
            ControlName = "CatiInterviewerConsole";
            InterviewBrowserState = null;
            AppointmentFormState = null;
            ConsoleState = ConsoleState.Selecting;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets Interview page browser control state
        /// </summary>
        public BaseStateData InterviewBrowserState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets Appointment form state.
        /// </summary>
        public BaseStateData AppointmentFormState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets Appointment form state.
        /// </summary>
        public BaseStateData OnABreakControlState
        {
            get;
            set;
        }


        /// <summary>
        /// Gets/sets current state of console.
        /// </summary>
        public ConsoleState ConsoleState
        {
            get;
            set;
        }

        #endregion
    }
}
