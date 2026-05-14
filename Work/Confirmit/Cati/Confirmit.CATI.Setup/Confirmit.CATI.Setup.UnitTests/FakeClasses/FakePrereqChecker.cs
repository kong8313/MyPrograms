using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Setup.UnitTests.FakeClasses
{
    public class FakePrereqChecker : IPrereqChecker
    {
        public bool IsFramework462Installed { get; set; }
       
        public FakePrereqChecker()
        {
            IsFramework462Installed = true;
        }

        public FakePrereqChecker(bool isFramework462Installed)
        {
            IsFramework462Installed = isFramework462Installed;
        }

        public void VerifyIsFramework462Installed()
        {
            if (!IsFramework462Installed)
            {
                throw new PrerequisiteException("No Framework 4.6.2");
            }
        }        
    }
}