namespace Confirmit.CATI.Core.Misc
{
    public class CommandLineParser : ICommandLineParser
    {
        /// <summary>
        /// Gets the instance ID from command line.
        /// </summary>
        /// <remarks>To use inside backend service only.</remarks>
        /// <returns>The ID of the instance. Empty string for default instance.</returns>
        public int GetCompanyId(string[] commandLineArgs)
        {
            int companyId = 0;

            // Try to find instance name as value of parameter in the command line
            for (int i = 0; i < commandLineArgs.Length; i++)
            {
                if ((System.String.Compare(commandLineArgs[i], "-" + GeneralConstants.InstanceServiceParameterName, System.StringComparison.OrdinalIgnoreCase) == 0) ||
                   (System.String.Compare(commandLineArgs[i], "/" + GeneralConstants.InstanceServiceParameterName, System.StringComparison.OrdinalIgnoreCase) == 0))
                {
                    ++i;
                    if (i < commandLineArgs.Length)
                    {
                        companyId = int.Parse(commandLineArgs[i]);

                        break;
                    }
                }
            }

            return companyId;
        }
    }
}
