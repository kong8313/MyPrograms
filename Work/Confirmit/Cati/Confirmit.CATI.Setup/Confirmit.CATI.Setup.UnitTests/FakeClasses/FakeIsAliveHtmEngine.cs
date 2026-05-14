using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Setup.UnitTests.FakeClasses
{
    class FakeIsAliveHtmEngine : IIsAliveHtmEngine
    {
        public void VerifyAccesToPageByUrl(string urlAddress)
        {
            
        }

        public string GetPhysicalPathToPage(string isAlivePageUrl)
        {
            return string.Empty;
        }

        public string AddFirstSlash(string pageUrl)
        {
            return string.Empty;
        }

        public bool BackupIsAliveHtmFile(string isAlivePageUrl)
        {
            return true;
        }

        public void RestoreIsAliveHtmFile(string isAlivePageUrl)
        {
            
        }
    }
}
