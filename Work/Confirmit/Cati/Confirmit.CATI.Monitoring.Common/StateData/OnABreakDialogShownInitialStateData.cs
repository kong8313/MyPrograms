using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
	/// <summary>
	/// Represents initial state data of 'OnABreak' dialog.
	/// </summary>
	[Serializable]
	public class OnABreakDialogShownInitialStateData : BaseStateData
	{
	    /// <summary>
        /// Initializes new instance of OnABreakDialogShownInitialStateData class.
		/// </summary>
        public OnABreakDialogShownInitialStateData()
			: base()
		{
		}

	    /// <summary>
		/// Interviewer break start time.
		/// </summary>
		public DateTime StartTime
		{
			get;
			set;
		}

	    public string BreakInfo { get; set; }

	    public bool DisplayBreakType { get; set; }
	}
}
