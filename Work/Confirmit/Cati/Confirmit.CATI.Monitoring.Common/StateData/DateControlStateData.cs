using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
	/// <summary>
	/// Represents state data of DateTime picker control. Contains single field - seleted date.
	/// </summary>
	[Serializable]
	public class DateControlStateData : BaseStateData
	{
		#region Constructors

		/// <summary>
		/// Initializes new instance of DateControlStateData class.
		/// </summary>
		public DateControlStateData()
			: base()
		{
		}

		/// <summary>
		/// Initializes new instance of DateControlStateData class and fills it
		/// with given data.
		/// </summary>
		/// <param name="controlName">Control name.</param>
		/// <param name="date">Control value as date.</param>
		public DateControlStateData(string controlName, DateTime date)
			: base(controlName)
		{
			Date = date;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets/sets date.
		/// </summary>
		public DateTime Date
		{
			get;
			set;
		}

		#endregion
	}
}
