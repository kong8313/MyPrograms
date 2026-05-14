using System;
using System.Linq;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    /// <summary>
    /// Represents the collection of shift types.
    /// </summary>
    [XmlRoot("ShiftTypes")]
    [Serializable]
    public class ShiftTypeCollection : BaseIdInt32Collection<ShiftType>
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return SchedulingUtilities.CloneBaseCollection<ShiftTypeCollection, ShiftType, int>(
                this
                );
        }
    }
}