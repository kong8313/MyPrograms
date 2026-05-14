using Confirmit.CATI.Build;

namespace ConfirmitDialerInterface
{
    internal static class CodiVersionInfo
    {
        public const string BuildNumber = "0";
        public const string Version = "3.4.0.0"; // Note, there should be no build number here
        public const string InformationalVersion = "3.4.4." + CatiBuildNumber.Value;
    }
}
