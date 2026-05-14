namespace Confirmit.CATI.Core.PersonLogin
{
    public class StationInfo
    {
        public int DialerId { get; set; }
        public bool IsLocal { get; set; }
        public string ExtensionNumber { get; set; }
        public string StationId { get; set; }

        public override string ToString()
        {
            return "[" + string.Join(",", "DialerId: " + DialerId, "ExtensionNumber: " + ExtensionNumber, "IsLocal: " + IsLocal, "StationId: '" + StationId + "'") + "]";
        }
    }
}