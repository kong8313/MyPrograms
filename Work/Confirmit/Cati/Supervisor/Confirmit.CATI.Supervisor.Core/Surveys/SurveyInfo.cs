using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    /// <summary>
    /// Class that contains information about survey, including assigned person count
    /// </summary>
    [Serializable]
    public class SurveyInfo : SurveyInfoItem
    {
        #region Members
        private int m_AssignedPersonCount = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Contains person count assigned on survey
        /// </summary>
        public int AssignedPersonCount
        {
            get
            {
                return m_AssignedPersonCount;
            }
        }

        #endregion

        #region Constructors

        public SurveyInfo(int id)
            : base(id)
        {
            BvSurveyEntity survey = SurveyRepository.GetById(id);
            ConfirmitID = survey.Name;
            Name = survey.Description;
        }

        public SurveyInfo(int id, string name, string confirmitID, int assignedPersonCount)
            : base(id, name, confirmitID)
        {
            m_AssignedPersonCount = assignedPersonCount;
        }

        #endregion

    }
}