using System;

namespace Confirmit.CATI.Supervisor.Classes.Script
{
    public class SchedulingParamInfo : IComparable<SchedulingParamInfo>
    {
        #region Properties

        /// <summary>
        /// Unique identifier.
        /// </summary>
        [RowRead("Id")]
        public int? Id
        {
            get; set;
        }

        [RowRead( "Name" )]
        public string Name
        {
            get;
            set;
        }

        [RowRead("Description")]
        public string Description
        {
            get;
            set;
        }

        [RowRead("Type")]
        public int? Type
        {
            get;
            set;
        }

        [RowRead( "DefaultValue" )]
        public int DefaultValue
        {
            get;
            set;
        }


        #endregion

        #region IComparable<ShiftTypeInfo> Members

        public int CompareTo(SchedulingParamInfo other)
        {
            return Id.Value.CompareTo( other.Id.Value );
        }

        #endregion
    }
}