using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
	/// <summary>
	/// Describes reason why user closes appointment form.
	/// </summary>
	public enum AppointmentFormCloseReasons
	{
		/// <summary>
		/// Save appointment changes and close form.
		/// </summary>
		Ok = 1,

		/// <summary>
		/// Cancel appointment changes and close form.
		/// </summary>
		Cancel = 2
	}

	/// <summary>
	/// Represents appointment form closing event state. Contains reason why we close from.
	/// For now we have 3: Ok, Cancel and Remove.
	/// </summary>
	[Serializable]
	public class AppointmentCloseStateData : BaseStateData
	{
		#region Properties

		/// <summary>
		/// Gets/sets reason why we close appointment form.
		/// </summary>
		public AppointmentFormCloseReasons CloseReason 
		{ 
			get; 
			set; 
		}

		#endregion
	}
}
