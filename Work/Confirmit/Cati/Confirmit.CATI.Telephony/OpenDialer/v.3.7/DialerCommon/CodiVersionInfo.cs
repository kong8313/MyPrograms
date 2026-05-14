namespace Confirmit.CATI.Telephony
{
    public class CodiVersionInfoCommon
    {
        public string CodiMajorVersion { get; private set; }
        public string CodiFullVersion { get; private set; }
        public string DialerDriverNameAndVersion { get; private set; }

        public CodiVersionInfoCommon(string codiMajorVersion,  string codiFullVersion, string dialerDriverNameAndVersion)
        {
            CodiMajorVersion = codiMajorVersion;
            CodiFullVersion = codiFullVersion;
            DialerDriverNameAndVersion = dialerDriverNameAndVersion;
        }

        public override string ToString()
        {
            return "[" + string.Join("/", CodiMajorVersion, CodiFullVersion, DialerDriverNameAndVersion) + "]";
        }
    }
}