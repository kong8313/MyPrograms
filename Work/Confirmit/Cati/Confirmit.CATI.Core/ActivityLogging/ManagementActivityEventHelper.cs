using System.Text.RegularExpressions;

namespace Confirmit.CATI.Core.ActivityLogging
{
    public static class ManagementActivityEventHelper
    {
        public static readonly Regex ProjectIdRegex = new Regex(@"p\d*", RegexOptions.Compiled);
    }
}