using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
	/// <summary>
	/// Describes reason why user closes appointment form.
	/// </summary>
	public enum RedialFormCloseReasons
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
}
