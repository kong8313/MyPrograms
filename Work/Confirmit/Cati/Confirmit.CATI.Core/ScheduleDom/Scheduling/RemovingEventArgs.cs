using System;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents the method which handles a removing event.
	/// </summary>
	/// <param name="sender">Object to be removed.</param>
	/// <param name="e">Event data.</param>
	public delegate void RemovingEventHandler( object sender, RemovingEventArgs e );

	/// <summary>
	/// Provides data for removing event.
	/// </summary>
	public class RemovingEventArgs : CancelWithErrorsEventArgs
	{
		#region Fields

		private int m_index;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the RemovingEventArgs class with the Index 
		/// property set to the given value.
		/// </summary>
		/// <param name="index">Index of item to be removed.</param>
		public RemovingEventArgs( int index )
			: base()
		{
			m_index = index;
		}

		/// <summary>
		/// Initializes a new instance of the RemovingEventArgs class with the Index 
		/// and Cancel properties set to the given values.
		/// </summary>
		/// <param name="index">Index of item to be removed.</param>
		/// <param name="cancel">true to cancel the event; otherwise, false.</param>
		public RemovingEventArgs( int index, bool cancel )
			: base( cancel )
		{
			m_index = index;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The index of item to be removed.
		/// </summary>
		public int Index
		{
			get { return m_index; }
		}

		#endregion
	}
}
