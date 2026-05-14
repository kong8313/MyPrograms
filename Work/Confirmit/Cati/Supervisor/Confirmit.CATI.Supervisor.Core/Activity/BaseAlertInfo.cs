using System;
using System.Collections.Generic;
using System.Text;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
	public class BaseAlertInfo
	{
		private int m_ObjectSID;
		private int m_Amber;
		private int m_Red;

		/// <summary>
		/// Object SID for alert (for now it's unsupported in BE, always 0)
		/// </summary>
		public int ObjectSID
		{
			get { return m_ObjectSID; }
			set { m_ObjectSID = value; }
		}

		/// <summary>
		/// Amber threshold value.
		/// </summary>
		public int Amber
		{
			get { return m_Amber; }
			set { m_Amber = value; }
		}

		/// <summary>
		/// Red threshold value.
		/// </summary>
		public int Red
		{
			get { return m_Red; }
			set { m_Red = value; }
		}
		
		/// <summary>
		/// Default construcrot.
		/// </summary>
		/// <param name="objectSID">Object sid.</param>
		/// <param name="amber">Amber threshold level.</param>
		/// <param name="red">Red threshold level.</param>
		public BaseAlertInfo(int objectSID, int amber, int red)
		{
			m_ObjectSID = objectSID;
			m_Amber = amber;
			m_Red = red;
		}
	}
}
