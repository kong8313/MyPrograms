using System;

using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.Installation.Common.Properties;
using Microsoft.Win32;

namespace Confirmit.CATI.Installation.Common
{
    public class PrereqChecker : IPrereqChecker
    {
        public void VerifyIsFramework462Installed()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
            {
                if (ndpKey == null)
                {
                    throw new PrerequisiteException(Resources.NoFramework462Message);
                }

                var releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));

                if (releaseKey < 394802)
                {
                    throw new PrerequisiteException(Resources.NoFramework462Message);
                }
            }
        }
    }
}
