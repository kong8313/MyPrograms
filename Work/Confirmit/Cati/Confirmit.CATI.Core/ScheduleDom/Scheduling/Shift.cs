using System;
using System.Xml.Serialization;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents shift.
	/// </summary>
	[Serializable]
	public class Shift : BaseShift<ShiftData>
	{
	    /// <summary>
		/// Default constructor.
		/// </summary>
		public Shift()
			:base()
		{
		}

		/// <summary>
		/// Initialize new instance of the object and fills it with the
		/// data of given object.
		/// </summary>
		/// <param name="obj">Object to copy.</param>
		protected Shift( Shift obj )
			: base( obj )
		{
		}

	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new Shift( this );
		}
	}

	/// <summary>
	/// Represents the collection of shifts.
	/// </summary>
	[XmlRoot("Shifts")]
	[Serializable]
	public class ShiftCollection : BaseShiftCollection<Shift, ShiftData>
	{
	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return SchedulingUtilities.CloneBaseCollection<ShiftCollection, Shift, int>(
				this
			);
		}
	}
}
