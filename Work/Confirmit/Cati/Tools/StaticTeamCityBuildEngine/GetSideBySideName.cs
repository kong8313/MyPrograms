using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using StaticTeamCityBuildEngine.CommonEngines;
using StaticTeamCityBuildEngine.Interfaces;

namespace StaticTeamCityBuildEngine
{
    public class GetSideBySideName : Task
    {
        [Output]
        public string SideBySideName { get; private set; }

        private string _defaultSideBySideName;

        public string DefaultSideBySideName
        {
            set { _defaultSideBySideName = value; }
        }

        public override bool Execute()
        {
            IExternalExecutor externalExecutor = new ExternalExecutor(Log);
            var createBranchFilesEngine = new CreateBranchFilesEngine(Log, externalExecutor);

            Log.LogMessage("DefaultSideBySideName=" + _defaultSideBySideName);

            try
            {
                SideBySideName = createBranchFilesEngine.GetSideBySideName(_defaultSideBySideName);
            }
            catch (Exception ex)
            {
                Log.LogMessage("An error occured during getting branch name: " + ex.Message + ". Set default value to SideBySideName.");
                SideBySideName = "Rel";
            }


            Log.LogMessage("SideBySideName=" + SideBySideName);

            return true;
        }
    }
}
