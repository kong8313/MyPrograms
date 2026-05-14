using System;
using System.ComponentModel;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Provides base class of even data for cancel event with errors reporting.
	/// </summary>
	public class CancelWithErrorsEventArgs : CancelEventArgs
	{
		#region Fields

		private ErrorCollection m_errors = new ErrorCollection();

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public CancelWithErrorsEventArgs()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the CancelWithErrorsEventArgs class with the Cancel 
		/// property set to the given value.
		/// </summary>
		/// <param name="cancel">true to cancel the event; otherwise, false.</param>
		public CancelWithErrorsEventArgs( bool cancel )
			: base( cancel )
		{
		}
		/// <summary>
		/// Initializes a new instance of the CancelWithErrorsEventArgs class with 
		/// the Errors property set to the given value.
		/// </summary>
		/// <param name="errors">Errors collection.</param>
		/// <exception cref="ArgumentNullException">Error collection is null.</exception>
		public CancelWithErrorsEventArgs( ErrorCollection errors )
			: base()
		{
			if(errors == null)
			{
				throw new ArgumentNullException( "errors", Strings.ItemNullExceptionMessage );
			}

			m_errors = errors;
		}

		/// <summary>
		/// Initializes a new instance of the CancelWithErrorsEventArgs class with 
		/// the Cancel property and Errors property set to the given value.
		/// </summary>
		/// <param name="cancel">true to cancel the event; otherwise, false.</param>
		/// <param name="errors">Errors collection.</param>
		public CancelWithErrorsEventArgs( bool cancel, ErrorCollection errors )
			: base( cancel )
		{
			if(errors == null)
			{
				throw new ArgumentNullException( "errors", Strings.ItemNullExceptionMessage );
			}

			m_errors = errors;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Collection of validation errors.
		/// </summary>
		/// <exception cref="ArgumentNullException">Error collection is null.</exception>
		public ErrorCollection Errors
		{
			get { return m_errors; }
			set 
			{
				if(value == null)
				{
					throw new ArgumentNullException( "value", Strings.ItemNullExceptionMessage );
				}

				m_errors = value; 
			}
		}

		#endregion
	}
}
