using System;
using System.ComponentModel;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents the method that handles a validation event. 
	/// </summary>
	/// <param name="sender">Object to be validated.</param>
	/// <param name="e">Event data.</param>
	public delegate void ValidationEventHandler( object sender, ValidationEventArgs e );

	/// <summary>
	/// Interface for objects which must be validated outside of it's class.
	/// </summary>
	public interface IExternalVerifiable
	{
		/// <summary>
		/// This event fires when object needs to be validated.
		/// </summary>
		event ValidationEventHandler Validating;
	}
}
