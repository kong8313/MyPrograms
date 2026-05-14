using System;

namespace DialerCommon.DialerParameters
{
    /// <summary>
    /// Class represents dialer parameter entity.
    /// </summary>
    [Serializable]
    public class DialerParameter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}


