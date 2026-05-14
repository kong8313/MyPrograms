using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Core.Confirmit
{
    /// <summary>
    /// Represent information about confirmit question with type and ability to mark this question checked/unchecked.    
    /// </summary>    
    [Serializable]
    public class SearchableFieldForSelection : IComparable
    {               
        #region Properties

        public SearchableFieldForSelection(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Field name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets table identifier of current variable in BvReplicatedColumns table
        /// </summary>
        public int TableId { get; set; }

        /// <summary>
        /// Gets/sets column identifier of current variable in BvReplicatedColumns table
        /// </summary>
        public int ColumnId { get; set; }

        /// <summary>
        /// Gets/sets flag indicated is field checked and available in console or not 
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Returns concatenation of TableId and ColumnId separated with '_' 
        /// </summary>
        public string Key 
        { 
          get
          {
              return IsSystem ? String.Format("0_{0}", Name) : String.Format("{0}_{1}", TableId, ColumnId);
          }
        }

        /// <summary>
        /// Confirmit variable's type (Loop, OpenForm, SingleForm, ...).
        /// </summary>
        public ConfirmitVariableType ConfirmitVariableType
        {
            get;
            set;
        }     

        /// <summary>
        /// Gets localiazed string value representing variable type.
        /// </summary>
        public string ConfirmitVariableTypeLocalizedString 
        {
            get
            {
                return StringHelper.GetStringFromEnum(ConfirmitVariableType);
            }
        }

        /// <summary>
        /// Determines whether the field is system field or variable.
        /// </summary>
        public bool IsSystem
        {
            get;
            set;
        } 

        /// <summary>
        /// Gets localiazed string value representing variable type.
        /// </summary>
        public string ConfirmitTypeLocalizedString
        {
            get
            {
                return IsSystem ? "System" : StringHelper.GetStringFromEnum(ConfirmitVariableType);
            }
        }

        #endregion

        #region Methods

        public SearchableFieldForSelection(int tableId, int columnId, string name)
        {
            TableId = tableId;
            ColumnId = columnId;
            Name = name;
        }
        
        /// <summary>
        /// Determines whether the specified Object is equal to the current VariableInfo object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SearchableFieldForSelection info = (SearchableFieldForSelection)obj;

            return (ColumnId == info.ColumnId && TableId == info.TableId);

        }

        /// <summary>
        /// Serves as a hash function for a AssignInfo.
        /// </summary>
        /// <returns>A hash code for the current AssignInfo.</returns>
        public override int GetHashCode()
        {
            return (ColumnId.ToString() + TableId.ToString()).GetHashCode();
        } 

        #endregion

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

            SearchableFieldForSelection var = (SearchableFieldForSelection)obj;

            return Name.CompareTo(var.Name);
        }

        #endregion
    }
}