using System;

namespace StaticTeamCityBuildEngine.CommonEngines
{
    /// <summary>
    /// Information about product
    /// </summary>
    public class GlobalInfo
    {
        public string BuildNumber { get; private set; }
        public string CompanyName { get; private set; }
        public string LegalCopyright { get; private set; }
        public string LegalTrademarks { get; private set; }
        public string ProductName { get; private set; }
        public string Title { get; private set; }

        public GlobalInfo(string sideBySideName, string buildNumber, string lastCommitHash)
        {
            CompanyName = "Forsta AS";
            LegalCopyright = "Copyright " + DateTime.Now.Year + " Forsta AS. All rights reserved.";
            LegalTrademarks = "Forsta Plus";
            ProductName = "Forsta Plus";
            BuildNumber = buildNumber;
            Title = string.Format("SxS: {0}. Hash: {1}", sideBySideName, lastCommitHash);
        }
    }
}
