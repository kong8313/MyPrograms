using System.Text.RegularExpressions;
using Confirmit.Security.Crypto.Web;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    /// <summary>
    /// Comparer class which compares starting intertview urls.
    /// </summary>
    public class InterviewUrlComparer
    {
        /// <summary>
        /// Compares two interview starting urls.
        /// </summary>
        /// <param name="url1">First url.</param>
        /// <param name="url2">Second url.</param>
        /// <remarks>This method decodes __SID__ parameter during comparision.</remarks>
        /// <returns><c>true</c> if urls are equal; otherwise <c>false</c></returns>
        public static bool AreEqual(string url1, string url2)
        {
            bool firstIsEmtpy = string.IsNullOrEmpty(url1);
            bool secondIsEmpty = string.IsNullOrEmpty(url2);

            if (firstIsEmtpy && secondIsEmpty)
            {
                return true;
            }

            if (firstIsEmtpy || secondIsEmpty)
            {
                return false;
            }

            var regEx = new Regex(@"(?<=sid__\=).+?((?=&)|$)");
            Match match1 = regEx.Match(url1);
            Match match2 = regEx.Match(url2);
            if (match1.Success && match2.Success)
            {
                // both urls contain __SID__
                if (AreEqualWithDecode(match1.Value, match2.Value) == false)
                {
                    return false;
                }

                string s1 = url1.Replace(match1.Value, string.Empty);
                string s2 = url2.Replace(match2.Value, string.Empty);

                return s1 == s2;
            }

            if (match1.Success || match2.Success)
            {
                return false;
            }
                
            return url1 == url2;
        }

        /// <summary>
        /// Determines if two strings encoded with <c>MachineKeyEncryptionManager</c>
        /// are equal.
        /// </summary>
        /// <param name="first">First string.</param>
        /// <param name="second">Second string.</param>
        /// <remarks>This method decodes input pamarameters and compares decoded strings.</remarks>
        /// <returns><c>true</c>, if strings are equal; otherwise <c>false.</c></returns>
        private static bool AreEqualWithDecode(string first, string second)
        {
            bool firstIsEmpty = string.IsNullOrEmpty(first);
            bool secondIsEmpty = string.IsNullOrEmpty(second);

            if (firstIsEmpty && secondIsEmpty)
            {
                return true;
            }

            if (firstIsEmpty || secondIsEmpty)
            {
                return false;
            }

            return EncryptionUsingMachineKey.Decrypt(DataProtection.All, first) ==
                   EncryptionUsingMachineKey.Decrypt(DataProtection.All, second);
        }
    }
}
