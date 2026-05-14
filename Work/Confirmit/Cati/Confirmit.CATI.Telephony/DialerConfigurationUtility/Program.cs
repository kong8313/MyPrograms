using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using DialerCommon;
using System.Data.SqlClient;

namespace DialerConfigurationUtility
{
    public class Program
    {
        private class StubIDbLibProvider : IDbLibProvider
        {
            public string CatiDefaultConnectionString { get; set; }
            public string ConfirmConnectionString { get; set; }
            public string ConfirmlogConnectionString { get; set; }

            public string ConfirmAdminConnectionString(string projectId)
            {
                throw new NotImplementedException();
            }

            public string GetConnectionStringForSpecificCompany(int companyId)
            {
                throw new NotImplementedException();
            }

            public int GetRandomCatiSqlServerId()
            {
                throw new NotImplementedException();
            }

            public string GetMasterConnectionStringForServer(int sqlServerId)
            {
                throw new NotImplementedException();
            }

            public string GetConfirmAdminConnectionStringForSpecificServer(int sqlServerId)
            {
                throw new NotImplementedException();
            }
        }

        public static void Main(string[] args)
        {
            var locator = new ServiceLocator();
            locator.Initialize();
            ServiceLocator.Register<ISideBySideManager, SideBySideManager>();
            ServiceLocator.Register<IIpFilterCache, IpFilterCache>();
            ServiceLocator.Register<IDialerType, DialerType>();

            new BackendRegistry().RegisterTypes(locator);
            
            new SystemSettingBackendRegistrator().RegisterTypes(locator);

            ShowHelp();

            if (!ProcessArguments(
                args, 
                out string dialerConfigFile, 
                out int companyId, 
                out int action, 
                out ICollection<int> dialerIds, 
                out bool isAlwaysYes, 
                out string connectionString,
                out string confirmConnectionString,
                out string confirmlogConnectionString))
            {
                ShowIncorrectParametersWarning();
                return;
            }

            var stubIDbLibProvider = RegistryStub<IDbLibProvider, StubIDbLibProvider>();
            stubIDbLibProvider.CatiDefaultConnectionString = connectionString;
            stubIDbLibProvider.ConfirmConnectionString = confirmConnectionString;
            stubIDbLibProvider.ConfirmlogConnectionString = confirmlogConnectionString;

            BackendInstance.Current = ServiceLocator.Resolve<IBackendInstanceFactory>().Create(
                companyId,
                HostType.BackendNamedInstance);

            using (var dialerAuthorizationKeyEncryptor = new DialerAuthorizationKeyEncryptor())
            {
                var dialersConfigurator = new DialersConfigurator(dialerAuthorizationKeyEncryptor);
                dialersConfigurator.UpdateDialerConfigurationParametersFromConfigurationFile(dialerConfigFile, action, dialerIds, companyId, isAlwaysYes);
            }
        }
        
        private static T RegistryStub<I, T>() where T : I
        {
            ServiceLocator.Resolve<IServiceRegistrator>().RegisterSingleton<I, T>();
            return (T)ServiceLocator.Resolve<I>();
        }
        private static bool ProcessArguments(
            string[] args,
            out string dialerConfigFile,
            out int companyId,
            out int actionType,
            out ICollection<int> dialerIds,
            out bool isAlwaysYes,
            out string connectionString,
            out string confirmConnectionString,
            out string confirmlogConnectionString)
        {
            connectionString = confirmConnectionString = confirmlogConnectionString = null;
            var argsList = new List<string>(args);

            bool needContinue = true;
            while (needContinue)
            {
                needContinue = false;

                SqlConnectionStringBuilder scsb;
                switch (argsList[0].ToLowerInvariant())
                {
                    case "/connectionstring":
                    case "-connectionstring":
                    case "/cs":
                    case "-cs":
                        scsb = new SqlConnectionStringBuilder(argsList[1]) { InitialCatalog = MultimodeInstanceName.CompanyIdToDatabaseName(0) };
                        connectionString = scsb.ConnectionString;
                        needContinue = true;
                        argsList.RemoveRange(0, 2);
                    break;
                    case "/confirmitconnectionstring":
                    case "-confirmitconnectionstring":
                    case "/ccs":
                    case "-ccs":
                        scsb = new SqlConnectionStringBuilder(argsList[1]) { InitialCatalog = "confirm" };
                        confirmConnectionString = scsb.ConnectionString;
                        scsb = new SqlConnectionStringBuilder(argsList[1]) { InitialCatalog = "confirmlog" };
                        confirmlogConnectionString = scsb.ConnectionString;
                        needContinue = true;
                        argsList.RemoveRange(0, 2);
                        break;
                }
            }

            return ProcessArguments(argsList.ToArray(), out dialerConfigFile, out companyId, out actionType, out dialerIds, out isAlwaysYes);
        }

        private static bool ProcessArguments(
            string[] args,
            out string dialerConfigFile,
            out int companyId,
            out int actionType,
            out ICollection<int> dialerIds,
            out bool isAlwaysYes)
        {
            try
            {
                dialerConfigFile = args[0];
                companyId = int.Parse(args[1]);
                actionType = ActionType.ParseAction(args[2]);
                dialerIds = new List<int>();

                isAlwaysYes = false;

                for (var i = 3; i < args.Length; i++)
                {
                    if (args[i].ToLower().Equals("/yes"))
                    {
                        isAlwaysYes = true;
                    }
                    else
                    {
                        dialerIds.Add(int.Parse(args[i]));
                    }
                }

                Console.WriteLine(@"Configuration file:                 " + dialerConfigFile);
                Console.WriteLine(@"Company Id:                         " + companyId);
                Console.WriteLine(@"Action:                             " + args[2]);
                Console.WriteLine(@"Dialer Ids:                         " + string.Join(", ", dialerIds.Select(s => s.ToString()).ToArray()));
                Console.WriteLine(@"Answer all yes/no questions as yes: " + isAlwaysYes);

                if (companyId <= 0)
                {
                    Console.WriteLine(@"Company Id must be non-negative");
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                dialerConfigFile = null;
                companyId = 0;
                actionType = ActionType.AddDialer;
                dialerIds = null;
                isAlwaysYes = false;
                return false;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine(@"Instructions:");
            Console.WriteLine(@"1. Open the company properties window in Confirmit, select 'Survey Channels' tab.");
            Console.WriteLine(@"   if 'Telephony enabled' is checked then uncheck it and save properties by pressing 'Save' button.");
            Console.WriteLine(@"2. Edit dialer config xml file:");
            Console.WriteLine(@"   - set required dialer type in <DialerType> section,");
            Console.WriteLine(@"   - edit section of the selected dialer type (section with corresponding dialer type name), you can point several ");
            Console.WriteLine(@"3. Run this utility as:");
            Console.WriteLine(@"   DialerConfigUtility.exe [/connectionString <connection string to CATI SQL server>] [/confirmitConnectionString <connection string to Confirmit SQL server with confirm and confirmlog databases>] <dialer config file> <company id> <action> <list of dialer ids separated by blanks> [<Flag to say yes on all questions>]");
            Console.WriteLine(@"4. Open the company properties window in Confirmit, select 'Survey Channels' tab.");
            Console.WriteLine(@"   Check 'Telephony enabled' box and save properties by pressing 'Save' button.");
            Console.WriteLine(@"Now the company is configured to work with dialers.");
            Console.WriteLine(string.Empty);
            Console.WriteLine(@"----------------------------");
            Console.WriteLine(@"Dialer configuration utility");
            Console.WriteLine(@"----------------------------");
        }

        private static void ShowIncorrectParametersWarning()
        {
            Console.WriteLine(@"Error:   Incorrect input parameters. Point configuration file name, company id, and one of the next actions:");
            Console.WriteLine(@"         /add, /update or /remove followed by an optional key /yes and a list of dialer ids separated by blanks.");
            Console.WriteLine(@"         The first parameter can be optional parameter with connection string to CATI SQL server.");
            Console.WriteLine(@"Examples: ");
            Console.WriteLine(@"          DialerConfigurationUtility.exe /cs ""Data Source=localhost;UID=sa;PWD=firm"" DialerConfig.xml 2 /add 1 2 3");
            Console.WriteLine(@"          DialerConfigurationUtility.exe DialerConfig.xml 2 /add 1 2 3");
            Console.WriteLine(@"          DialerConfigurationUtility.exe DialerConfig.xml 2 /add 1 /yes");
            Console.WriteLine(@"          DialerConfigurationUtility.exe DialerConfig.xml 2 /update 1");
            Console.WriteLine(@"          DialerConfigurationUtility.exe DialerConfig.xml 2 /remove 3");
        }
    }
}