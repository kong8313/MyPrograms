using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
	/// <summary>
	/// Represents initial state data of appointment form as a whole. Contains state of all controls
	/// of appointment form.
	/// </summary>
	[Serializable]
	public class AppointmentInitialStateData : BaseStateData
	{
		#region Constructors

		/// <summary>
		/// Initializes new instance of AppointmentInitialStateData class.
		/// </summary>
		public AppointmentInitialStateData()
			: base()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Contact person name.
		/// </summary>
		public string ContactName
		{
			get;
			set;
		}

		/// <summary>
		/// Appointment date.
		/// </summary>
		public DateTime AppointmentDate
		{
			get;
			set;
		}

		/// <summary>
		/// Appointment time.
		/// </summary>
		public string AppointmentTime
		{
			get;
			set;
		}

		/// <summary>
		/// Expiration date.
		/// </summary>
		public DateTime ExpirationDate
		{
			get;
			set;
		}

		/// <summary>
		/// Expiration time.
		/// </summary>
		public string ExpirationTime
		{
			get;
			set;
		}

		/// <summary>
		/// Flag indicates that appointment never expire.
		/// </summary>
		public bool NeverExpire
		{
			get;
			set;
		}

		/// <summary>
		/// Flag indicates
		/// </summary>
		public bool Logout
		{
			get;
			set;
		}

        /// <summary>
        /// Current local interviewer time.
        /// </summary>
        public DateTime CurrentInterviewerTime
        {
            get;
            set;
        }

        /// <summary>
        /// Current local respondent time.
        /// </summary>
        public DateTime CurrentRespondentTime
        {
            get;
            set;
        }

	    public bool ShowAppointmentsOutsidePermittedShiftTime { get; set; }

		/// <summary>
		/// Current timezone name.
		/// </summary>
		public string TimezoneDisplayName
		{
			get;
			set;
		}
		#endregion
	}
}
