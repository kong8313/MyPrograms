using System;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
	/// <summary>
	/// Class that contains information about users and groups in person's tree
	/// </summary>
    /// <remarks>
    /// It is used only for PersonTree
    /// </remarks>
	public class PersonGroupInfo
	{        
        private int m_SID;
        private bool m_isGroup;
		private int m_MembersCount = 0;
		private int m_CurSurveyAssign = 0;

		private int m_AllSurveyAssign = 0;
		private int m_TotalAssignedSurvey = 0;

		private bool m_IsAssignedOnCurrentSurvey = false;

		public PersonGroupInfo( bool is_group, int sid, string name )
		{
			m_isGroup = is_group;
			m_SID = sid;
			Name = name;
		}

        public PersonGroupInfo(bool is_group, int sid, string name, string description)
            : this(is_group, sid, name)
        {
            Description = description;
        }

		public PersonGroupInfo( bool is_group, int sid, string name, int members_count, int cur_survey_assign, int all_survey_assign, int total_assigned_survey, bool is_assigned_on_current )
		{
			m_isGroup = is_group;
			m_SID = sid;
			Name = name;
			m_MembersCount = members_count;
			m_CurSurveyAssign = cur_survey_assign;
			m_AllSurveyAssign = all_survey_assign;
			m_TotalAssignedSurvey = total_assigned_survey;
			m_IsAssignedOnCurrentSurvey = is_assigned_on_current;
		}

		/// <summary>
		/// Defines is it person or group
		/// </summary>
		public bool IsGroup
		{
			get { return m_isGroup; }
		}

		/// <summary>
		/// Person's SID
		/// </summary>
		public int SID
		{
			get { return m_SID; }
		}

		/// <summary>
		/// Person's name
		/// </summary>
		public string Name
		{
			get;
			set;
		}

        public string Description
        {
            get;
            set;
        }

		/// <summary>
		/// Number of person's children
		/// </summary>
		public int MembersCount
		{
			get { return m_MembersCount; }
		}

		/// <summary>
		/// Number of assigned calls from current survey
		/// </summary>
		public int CurSurveyAssign
		{
			get { return m_CurSurveyAssign; }
		}

		/// <summary>
		/// Number of assigned calls from all surveys
		/// </summary>
		public int AllSurveyAssign
		{
			get { return m_AllSurveyAssign; }
		}

		/// <summary>
		/// Number of assigned surveys
		/// </summary>
		public int TotalAssignedSurvey
		{
			get { return m_TotalAssignedSurvey; } 
		}

		/// <summary>
		/// Shows if person is assigned on current survey
		/// </summary>
		public bool IsAssignedOnCurrentSurvey
		{
			get { return m_IsAssignedOnCurrentSurvey; }
		}
	}
}
