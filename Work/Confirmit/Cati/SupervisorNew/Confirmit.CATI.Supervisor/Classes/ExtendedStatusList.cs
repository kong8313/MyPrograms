using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Classes
{
    /// <summary>
    /// Extended status list for export/import
    /// </summary>
    [Serializable]
    public class ExtendedStatusList
    {
        public ExtendedStatusList()
        {
        }

        public ExtendedStatusList(List<BvStateEntity> states, string groupName)
        {
            StateGroupName = groupName;
            States = states;
        }

        [XmlElement]
        public string StateGroupName { get; set; }

        [XmlArray]
        [XmlArrayItem(typeof(BvStateEntity), ElementName = "State")]
        public List<BvStateEntity> States { get; set; }
    }
}
