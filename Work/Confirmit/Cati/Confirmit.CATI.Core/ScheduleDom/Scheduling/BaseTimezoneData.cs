using System;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents base class which contains shift data for specified timezone.
	/// It is used only for serialization of shift data for specified timezone.
	/// </summary>
	/// <typeparam name="T">Shift data type.</typeparam>
	[Serializable]
	public class BaseTimezoneData<T>
	{
		#region Fields

		private int? m_id = null;
		private T m_data = default(T);

		#endregion

		#region Constructors

		/// <summary>
		/// Empty constructor. It must exist for standard serialization.
		/// </summary>
		public BaseTimezoneData()
		{
		}

		/// <summary>
		/// Constructs object with specified data.
		/// </summary>
		/// <param name="timezoneId">Timezone identifier.</param>
		/// <param name="timezoneData">Shift data for specified timezone.</param>
		public BaseTimezoneData( int timezoneId, T timezoneData )
		{
			m_id = timezoneId;
			m_data = timezoneData;
		}

		/// <summary>
		/// Constructs object with specified data. Timezone identifier is not specified
		/// so we consider that the data corresponds respondent timezone.
		/// </summary>
		/// <param name="timezoneData">Shift data.</param>
		public BaseTimezoneData( T timezoneData )
		{
			m_id = null;
			m_data = timezoneData;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Timezone identifier.
		/// </summary>
		public int? Id
		{
			get { return m_id; }
			set { m_id = value; }
		}

		/// <summary>
		/// Shift data.
		/// </summary>
		public T Data
		{
			get { return m_data; }
			set { m_data = value; }
		}

		#endregion
	}
}
