using System;

namespace Confirmit.CATI.Supervisor.Classes.Script
{
    public class ShiftTypeInfo : IComparable<ShiftTypeInfo>
    {
        #region Fields

        private int? m_id = null;
        private string m_name = string.Empty;
        private bool m_isExlusion = false;
        private string m_colorName = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Unique identifier.
        /// </summary>
        [RowRead( "Id" )]
        public int? Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        [RowRead( "Name" )]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        [RowRead( "ColorName" )]
        public string ColorName
        {
            get
            {
                return m_colorName;
            }
            set
            {
                m_colorName = value;
            }
        }

        [RowRead( "IsExclusion" )]
        public bool IsExclusion
        {
            get
            {
                return m_isExlusion;
            }
            set
            {
                m_isExlusion = value;
            }
        }


        #endregion

        #region IComparable<ShiftTypeInfo> Members

        public int CompareTo( ShiftTypeInfo other )
        {
            return m_id.Value.CompareTo( other.m_id.Value );
        }

        #endregion
    }
}