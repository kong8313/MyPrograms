using System;

namespace Confirmit.CATI.Core.Services.Survey
{
    public class ProjectIdConverter
    {
        public static long ProjectIdToCampaignId(string projectId)
        {
            long result;

            if ((!Int64.TryParse(projectId.Substring(1), out result)) || (projectId[0] != 'p'))
            {
                throw new ArgumentException(string.Format("Project id '{0}' is incorrect", projectId));
            }

            return result;
        }
    }
}
