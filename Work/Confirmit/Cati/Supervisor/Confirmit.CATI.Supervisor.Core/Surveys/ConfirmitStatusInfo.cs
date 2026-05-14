using System;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
	/// <summary>
	/// Represents Confirmit status info item.
	/// </summary>
	public class ConfirmitStatusInfo
	{
		#region Members.
		private string m_ConfrimitCode;
		private string m_ConfirmitName;
		private int m_FusionCode;
		#endregion

		/// <summary>
		/// Gets Confirmit status code.
		/// WARNING: Can be null.
		/// </summary>
		public string ConfirmitCode
		{
			get { return m_ConfrimitCode; }
		}

		/// <summary>
		/// Gets Confirmit status name.
		/// </summary>
		public string ConfirmitName
		{
			get { return m_ConfirmitName; }
		}

		/// <summary>
		/// Gets Fusion internal status code.
		/// </summary>
		public int FusionCode
		{
			get { return m_FusionCode; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ConfirmitStatusInfo(string confirmitCode, string confirmitName, int fusionCode)
		{
			m_ConfrimitCode = confirmitCode;
			m_ConfirmitName = confirmitName;
			m_FusionCode = fusionCode;
		}

		/// <summary>
		/// Overridden Equals method.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != this.GetType())
				return false;
			ConfirmitStatusInfo info = (ConfirmitStatusInfo)obj;
			return  (m_FusionCode == info.FusionCode) && (m_ConfrimitCode == info.ConfirmitCode) && (m_ConfirmitName == info.ConfirmitName);
		}

		/// <summary>
		/// Overridden GetHashCode() method.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (m_FusionCode.ToString() + m_ConfrimitCode + m_ConfirmitName).GetHashCode();
		}
	}
}
