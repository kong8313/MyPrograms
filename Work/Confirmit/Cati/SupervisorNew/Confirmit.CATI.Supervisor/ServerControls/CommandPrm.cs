using System;

namespace Confirmit.CATI.Supervisor.ServerControls
{
	public class CommandPrm
	{
		private string m_columnName;
		private string m_prmName;

		public string ColumnName
		{
			get { return m_columnName; }
			set { m_columnName = value; }
		}

		public string PrmName
		{
			get { return m_prmName; }
			set { m_prmName = value; }
		}
	}
}
