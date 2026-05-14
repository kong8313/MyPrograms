using System;
using System.ComponentModel;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Provides data for a validation event.
	/// </summary>
	public class ValidationEventArgs : CancelWithErrorsEventArgs
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the ValidationEventArgs class with the Cancel 
		/// property set to the given value.
		/// </summary>
		/// <param name="cancel">true to cancel the event; otherwise, false.</param>
		public ValidationEventArgs( bool cancel )
			: base( cancel )
		{
		}

		#endregion
	}
}
		
