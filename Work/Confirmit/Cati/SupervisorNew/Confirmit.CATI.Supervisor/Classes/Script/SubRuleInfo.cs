using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Supervisor.Classes.Script
{
    public class SubRuleInfo
    {
        #region Fields

        private Guid? m_id = null;
        private int m_itsId;
        private int m_shiftTypeId;
        private string m_filter;
        private bool m_filterEnabled;
        private string m_description;
        private List<ActionInfo> m_actions = new List<ActionInfo>();

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
        /// ITS identifier. It is nullable value. If this value is null that means
        /// that object is not proper initialized.
        /// </summary>
        [RowRead( "ItsId" )]
        public int ItsId
        {
            get { return m_itsId; }
            set { m_itsId = value; }
        }

        /// <summary>
        /// Shift type identifier. It is nullable value. If this value is null that means
        /// that object is not proper initialized.
        /// </summary>
        [RowRead( "ShiftTypeId" )]
        public int ShiftTypeId
        {
            get { return m_shiftTypeId; }
            set { m_shiftTypeId = value; }
        }

        /// <summary>
        /// Filter of sub-rule. By default it is empty.
        /// </summary>
        [RowRead( "Filter" )]
        public string Filter
        {
            get { return m_filter; }
            set { m_filter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the sub-rule filter is enabled. It is false by default.
        /// </summary>
        [RowRead( "FilterEnabled" )]
        public bool FilterEnabled
        {
            get { return m_filterEnabled; }
            set { m_filterEnabled = value; }
        }

        /// <summary>
        /// Sub-rule description.
        /// </summary>
        [RowRead( "Description" )]
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        public List<ActionInfo> Actions
        {
            get { return m_actions; }
            set { m_actions = value; }
        }
        #endregion

        #region Methods
        public void FillSubRule( SubRule subRule )
        {
            if (subRule != null)
            {
                subRule.ShiftTypeId = m_shiftTypeId;
                subRule.ItsId = m_itsId;
                subRule.Filter = m_filter;
                subRule.FilterEnabled = m_filterEnabled;
                subRule.Description = m_description;
            }
        }
        #endregion
    }
}
