using System;
using System.Xml.Serialization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents exclusion data. Exclusion data contain start and end dates.
	/// </summary>
	[Serializable]
	public struct ExclusionData : IIntersectable<ExclusionData>
	{
	    /// <summary>
		/// Date is stored in UTC time and it is serialiazed in Xml in UTC time.
		/// See <![CDATA[http://msdn2.microsoft.com/en-us/library/ms973825.aspx#datetime_topic4]]>
		/// for description.
		/// </summary>
		private DateTime? m_startDate;

		/// <summary>
		/// Date is stored in UTC time and it is serialiazed in Xml in UTC time.
		/// See <![CDATA[http://msdn2.microsoft.com/en-us/library/ms973825.aspx#datetime_topic4]]>
		/// for description.
		/// </summary>
		private DateTime? m_endDate;

	    /// <summary>
		/// Initializes a new instance of the ExclusionData structure to a specified
		/// start date and end date. Dates are stored in UTC time.
		/// </summary>
		/// <param name="startDate">Shift start date.</param>
		/// <param name="endDate">Shift end date.</param>
		public ExclusionData( DateTime startDate, DateTime endDate )
		{
		    m_startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
			m_endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
		}

	    /// <summary>
		/// Exclusion start date. Date is in UTC time.
		/// </summary>
		[XmlElement]
		public DateTime? StartDate
		{
			get { return m_startDate; }
			set 
			{
                m_startDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value;
			}
		}

		/// <summary>
		/// Exclusion end date. Date is in UTC time.
		/// </summary>
		[XmlElement]
		public DateTime? EndDate
		{
			get { return m_endDate; }
			set 
			{
                m_endDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value;
			}
		}

	    /// <summary>
		/// Determines if current object has intersection with given object.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <returns>true, if object intersects; otherwise false.</returns>
		public bool HasIntersection( ExclusionData obj )
		{
            var validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
			ErrorCollection errors;
            if (!validator.Validate(this, out errors))
			{
				throw new ApplicationException( Strings.InvalidItemExceptionMessage );
			}

            if (!validator.Validate(obj, out errors))
			{
				throw new ArgumentException( Strings.InvalidItemExceptionMessage, "obj" );
			}

			return !((EndDate.Value <= obj.StartDate.Value) ||
						(obj.EndDate.Value <= StartDate.Value));
		}
	}
}
