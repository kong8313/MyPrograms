using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents state data of check redo question action. Contains single field - question identifier.
    /// </summary>
    [Serializable]
    public class RedoQuestionStateData : BaseStateData
    {
        #region Properties

        /// <summary>
        /// Question identifier.
        /// </summary>
        public string QuestionId
        {
            get;
            set;
        }

        #endregion
    }
}
