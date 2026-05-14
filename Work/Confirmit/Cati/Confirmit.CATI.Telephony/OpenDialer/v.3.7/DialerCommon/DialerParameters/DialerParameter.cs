using System;
using System.Runtime.Serialization;

namespace DialerCommon.DialerParameters
{
    /// <summary>
    /// Class represents dialer parameter entity.
    /// </summary>
    [Serializable]
    [DataContract]
    public class DialerParameter
    {
        [DataMember]
        public string Id { get; set; }
        
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public string Type { get; set; }
        
        [DataMember]
        public string Value { get; set; }
        
        [DataMember]
        public string Description { get; set; }
    }
}


