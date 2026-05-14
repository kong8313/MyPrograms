using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Classes.Script
{
    public class RuleInfo
    {
        #region Fields

        private Guid? m_id = null;
        private string m_description;
        private string _sampleUpdate;
        private List<SubRuleInfo> m_subRules = new List<SubRuleInfo>();

        #endregion

        #region Properties
        /// <summary>
        /// Unique identifier which has type Guid. It is nullable value. If this value is null that means
        /// that object is not initialized.
        /// </summary>
        [RowRead( "Id" )]
        public Guid? Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        /// <summary>
        /// Rule description.
        /// </summary>
        [RowRead( "Description" )]
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// Sample update flag
        /// </summary>
        [RowRead("SampleUpdate")]
        public string SampleUpdate
        {
            get => _sampleUpdate;
            set => _sampleUpdate = value;
        }

        /// <summary>
        /// Collection of sub-rules which belong to current rule.
        /// </summary>
        public List<SubRuleInfo> SubRules
        {
            get { return m_subRules; }
            set { m_subRules = value; }
        }
        #endregion
    }
}
