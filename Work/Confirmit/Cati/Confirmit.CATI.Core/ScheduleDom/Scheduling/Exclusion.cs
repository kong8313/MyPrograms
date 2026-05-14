using System;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents exclusion. Exclusion does nothing, it is just container of data.
	/// </summary>
	[Serializable]
	public class Exclusion : BaseShift<ExclusionData>
	{
	    /// <summary>
		/// Default constructor.
		/// </summary>
		public Exclusion()
			: base()
		{
			base.ShiftTypeId = ExclusionShiftTypeId;
		}

		/// <summary>
		/// Initialize new instance of the object and fills it with the
		/// data of given object.
		/// </summary>
		/// <param name="obj">Object to copy.</param>
		protected Exclusion( Exclusion obj )
			: base( obj )
		{
			base.ShiftTypeId = ExclusionShiftTypeId;
		}

	    /// <summary>
		/// Shift type identifier. It is nullable value. If this value is null that means
		/// that object is not proper initialized.
		/// </summary>
		public override int? ShiftTypeId
		{
			get
			{
				return base.ShiftTypeId;
			}
			set
			{
				if(value.HasValue && value.Value != ExclusionShiftTypeId)
				{
					throw new ArgumentException( Strings.ExclusionTypeExceptionMessage, "value" );
				}
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new Exclusion( this );
		}

	    /// <summary>
		/// Gets shift type identifier of exclusion shift type.
		/// </summary>
		private int ExclusionShiftTypeId
		{
			get
			{
				ShiftType exclusionShiftType = new ShiftType();
				exclusionShiftType.ConvertToExclusionShiftType();

				return exclusionShiftType.Id.Value;
			}
		}
	}

	/// <summary>
	/// Represents collection of excusions.
	/// </summary>
	[XmlRoot("Exclusions")]
	[Serializable]
	public class ExclusionCollection : BaseShiftCollection<Exclusion, ExclusionData>
	{
	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return SchedulingUtilities.CloneBaseCollection<ExclusionCollection, Exclusion, int>(
				this
			);
		}
	}
}
