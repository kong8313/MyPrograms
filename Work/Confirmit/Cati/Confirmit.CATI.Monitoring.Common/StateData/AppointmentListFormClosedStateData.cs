using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
	/// Represents initial state data of interviewing form as a whole. Contains state of WebBrowser and KeyboardInputControl.
	/// </summary>
	[Serializable]
    public class AppointmentListFormClosedStateData  : BaseStateData
    {
        #region Constructors
		/// <summary>
        /// Initializes new instance of AppointmentListFormClosedStateData class.
		/// </summary>
        public AppointmentListFormClosedStateData()
			: base()
		{
		}

		#endregion

		#region Properties
      
        /// <summary>
        /// Gets/sets interview mode
        /// </summary>
        public ConsoleState ConsoleState
        {
            get;
            set;
        }

        #endregion
    }
}
