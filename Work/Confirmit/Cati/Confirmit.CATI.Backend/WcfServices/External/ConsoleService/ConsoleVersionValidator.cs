using System;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    public class ConsoleVersionValidator : IConsoleVersionValidator
    {
        private readonly ISystemSettingRepository _systemSettingRepository;

        public ConsoleVersionValidator(ISystemSettingRepository systemSettingRepository)
        {
            _systemSettingRepository = systemSettingRepository;
        }

        public void ValidateVersion(string version)
        {
            if (!IsLatestVersion(version))
            {
                throw new UserMessageException("A new version of the console is available. The console will be closed in order to start the upgrade process. \n" +
                    "Please launch the console again to continue working.", "NewVersionOfConsoleIsAvailable");
            }
        }

        /// <summary>
        /// Detects if supplied version of Interviewer Console is the latest one
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns></returns>
        public bool IsLatestVersion(string version)
        {
            const int defaultInstanceCompanyId = 0;

            var versionInDb = _systemSettingRepository.Get(SystemSettingConstants.Setup.InterviewerConsoleVersion, defaultInstanceCompanyId);

            return versionInDb.Equals(version, StringComparison.OrdinalIgnoreCase);
        }
    }
}