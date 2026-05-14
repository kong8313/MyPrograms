using Confirmit.CATI.Build;

namespace Confirmit.CATI.DialerInterface_3_0
{
    internal static class CodiVersionInfo
    {
        public const string BuildNumber = "0";
        public const string Version = "17.5.4.0"; // Note, there should be no build number here
        public const string InformationalVersion = "18.5.5." + CatiBuildNumber.Value;
    }
}
