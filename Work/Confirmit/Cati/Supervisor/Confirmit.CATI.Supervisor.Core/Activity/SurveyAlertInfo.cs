using System;
using System.Collections.Generic;
using System.Text;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
	/// <summary>
	/// Class represents single alert item.
	/// </summary>
	[Serializable]
	public class SurveyAlertInfo: BaseAlertInfo
	{

		private int m_ThresholdsTypeId;

		/// <summary>
		/// Thresholds type id, defined in BE (see BvThresholdsTypeId table).
		/// </summary>
		public int ThresholdsTypeId
		{
			get { return m_ThresholdsTypeId; }
			set { m_ThresholdsTypeId = value; }
		}

		/// <summary>
		/// String column name - just title text from resourses for selected thresholds type id.
		/// </summary>
		public string ColumnName
		{
			get { return ResourceWrapper.Instance.GetString(((BvThresholdType)m_ThresholdsTypeId).ToString()); }
		}

        public BvThresholdType ThresholdType
        {
            get { return (BvThresholdType)m_ThresholdsTypeId; }
        }

		/// <summary>
		/// Default construcrot.
		/// </summary>
		/// <param name="objectSID">Object sid.</param>
		/// <param name="thresholdsTypeId">Thresholds type id.</param>
		/// <param name="amber">Amber threshold level.</param>
		/// <param name="red">Red threshold level.</param>
		public SurveyAlertInfo(int objectSID, int amber, int red, int thresholdsTypeId)
			: base(objectSID, amber, red)
		{
			m_ThresholdsTypeId = thresholdsTypeId;
		}
	}
}
