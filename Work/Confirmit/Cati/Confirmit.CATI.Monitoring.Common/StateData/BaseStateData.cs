using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
	/// <summary>
	/// Base abstract class for state data classes. Each state data class contains data
	/// which describe state of monitorable control. Classes should inherit this
	/// class. All state data classes should be serializable and contain name of control,
	/// which data they contain.
	/// </summary>
	[Serializable]
	public abstract class BaseStateData
	{
	    /// <summary>
		/// Initializes new instance of BaseStateData class.
		/// </summary>
	    protected BaseStateData()
		{
			ControlName = String.Empty;
		}

		/// <summary>
		/// Initializes new instance of BaseStateData class and fills it with given data.
		/// </summary>
		/// <param name="controlName">Name of the control which data current object contains.</param>
		protected BaseStateData(string controlName)
		{
			ControlName = controlName;
		}

	    /// <summary>
		/// Gets/sets name of the control which data current object contains.
		/// </summary>
		public virtual string ControlName
		{
			get;
			set;
		}
	}
}
