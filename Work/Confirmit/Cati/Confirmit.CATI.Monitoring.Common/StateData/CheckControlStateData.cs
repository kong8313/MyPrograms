using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
	/// <summary>
	/// Represents state data of check box/radio control. Contains single field - boolean flag.
	/// </summary>
    [Serializable]
	public class CheckControlStateData : BaseStateData
	{
		#region Constructors

		/// <summary>
		/// Initialiazes new instance of CheckControlStateData.
		/// </summary>
		public CheckControlStateData()
			: base()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets/sets checked state.
		/// </summary>
		public bool Checked
		{
			get;
			set;
		}

		#endregion
	}
}
