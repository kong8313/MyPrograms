using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
	/// <summary>
	/// Represents single item info in status breakdown for survey row in CATI activity view.
	/// </summary>
	public class StatusInfo
	{
		private int m_Id;
		/// <summary>
		/// Gets BvFEE id for status.
		/// </summary>
		public int Id 
		{ 
			get 
			{ 
				return m_Id; 
			} 
		}
		
		private string m_Name;
		/// <summary>
		/// Gets confirmit status name.
		/// </summary>
		public string Name 
		{ 
			get 
			{ 
				return m_Name; 
			} 
		}
		
		private int m_Value;
		/// <summary>
		/// Gets number of calls for the status.
		/// </summary>
		public int Value 
		{ 
			get 
			{ 
				return m_Value; 
			} 
		}

		private AlertStatus m_Alert = AlertStatus.Ok;
		/// <summary>
		/// Gets alerts status for the status.
		/// </summary>
		public AlertStatus Alert
		{
			get 
			{
				return m_Alert;
			}
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="id">BvFEE status id.</param>
		/// <param name="name">Status name.</param>
		/// <param name="value">Number of calls for the status.</param>
		public StatusInfo(int id, string name, int value, AlertStatus alert)
		{
			m_Id = id;
			m_Name = name;
			m_Value = value;
			m_Alert = alert;
		}
	}
}
