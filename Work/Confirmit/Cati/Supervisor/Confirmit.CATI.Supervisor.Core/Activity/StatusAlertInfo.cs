using System;
using System.Collections.Generic;
using System.Text;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
	/// <summary>
	/// Represents status alert info item.
	/// </summary>
	public class StatusAlertInfo: BaseAlertInfo
	{
		private int m_StatusId;
		/// <summary>
		/// Gets or sets BvFEE ITS code.
		/// </summary>
		public int StatusId
		{
			get{ return m_StatusId; }
			set{ m_StatusId = value; }
		}

		
		private string m_StatusName;
		/// <summary>
		/// Gets or sets BvFEE ITS name.
		/// </summary>
		public string StatusName
		{
			get { return m_StatusName; }
			set { m_StatusName = value; }
		}

		/// <summary>
		/// Default consrtuctor.
		/// </summary>
		/// <param name="objectSID">Object SID (reserved, always 0).</param>
		/// <param name="amber">Amber threshold.</param>
		/// <param name="red">Red threshold.</param>
		/// <param name="statusName">ITS name.</param>
		public StatusAlertInfo(int objectSID, int amber, int red, int statusId, string statusName) : base(objectSID, amber, red)
		{
			m_StatusId = statusId;
			m_StatusName = statusName;
		}
	}
}
