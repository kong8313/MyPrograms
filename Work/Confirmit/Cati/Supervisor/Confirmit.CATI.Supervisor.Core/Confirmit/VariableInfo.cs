using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Core.Confirmit
{
    /// <summary>
    /// Contains information about single variable used in filters.
    /// </summary>
    [Serializable]
    public class VariableInfo : IComparable
    {
        string m_name;
        VariableTypes m_variableType;
        TableTypes m_tableType;
        ConfirmitVariableType m_confirmitVariableType;
        string m_column;
        string m_value;

        public bool IsBackground{ get; set; }

        public VariableInfo(
            string name, 
            VariableTypes variableType, 
            TableTypes tableType, 
            string column, 
            string value,
            ConfirmitVariableType confirmitVariableType)
        {
            m_name = name;
            m_variableType = variableType;
            m_tableType = tableType;
            m_column = column;
            m_value = value;
            m_confirmitVariableType = confirmitVariableType;

            IsBackground = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public VariableInfo(string name, VariableTypes variableType, TableTypes tableType, string column)
            : this(name, variableType, tableType, column, "", ConfirmitVariableType.NotSet)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public VariableInfo(string name, VariableTypes variableType, TableTypes tableType, string column, string value)
            : this(name, variableType, tableType, column, value, ConfirmitVariableType.NotSet)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public VariableInfo(string name, VariableTypes variableType, TableTypes tableType)
            : this(name, variableType, tableType, name, "", ConfirmitVariableType.NotSet)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public VariableInfo(string name, VariableTypes variableType, TableTypes tableType, ConfirmitVariableType confirmitVariableType)
            : this(name, variableType, tableType, name, "", confirmitVariableType)
        { }

        #region Properties

        /// <summary>
        /// Variable's name.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Variable's data type.
        /// </summary>
        public VariableTypes VariableType
        {
            get { return m_variableType; }
            set { m_variableType = value; }
        }

        /// <summary>
        /// Variable's type (call field, subfilter, ...).
        /// </summary>
        public TableTypes TableType
        {
            get { return m_tableType; }
            set { m_tableType = value; }
        }

        /// <summary>
        /// Confirmit variable's type (Loop, OpenForm, SingleForm, ...).
        /// </summary>
        public ConfirmitVariableType ConfirmitVariableType
        {
            get { return m_confirmitVariableType; }
            set { m_confirmitVariableType = value; }
        }

        /// <summary>
        /// Variable's column.
        /// </summary>
        public string Column
        {
            get { return m_column; }
            set { m_column = value; }
        }

        /// <summary>
        /// Variable's value.
        /// </summary>
        public string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        /// <summary>
        /// Gets localiazed string value representing variable type.
        /// </summary>
        public string ConfirmitVariableTypeLocalizedString 
        {
            get
            {
                return StringHelper.GetStringFromEnum(m_confirmitVariableType);
            }
        }

        #endregion

        /// <summary>
        /// Determines whether the specified Object is equal to the current VariableInfo object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            VariableInfo info = (VariableInfo)obj;

            return (Name == info.Name && TableType == info.TableType);

        }
        /// <summary>
        /// Serves as a hash function for a AssignInfo.
        /// </summary>
        /// <returns>A hash code for the current AssignInfo.</returns>
        public override int GetHashCode()
        {
            return (Name + ((int)TableType).ToString()).GetHashCode();
        }

        #region IComparable Members
        /// <summary>
        /// Compares this instance to a specified VariableInfo object
        /// and returns an indication of their relative values.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return 0;
            }

            VariableInfo var = (VariableInfo)obj;

            return Name.CompareTo(var.Name);
        }

        #endregion
    }
}