using System;
using System.Diagnostics;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;

namespace DatabaseCheckUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Initializer.Initialize(args);

                var connectionStrings = ServiceLocator.Resolve<IConnectionStrings>();
                var databaseInfoProvider = new DatabaseInfoProvider(connectionStrings);

                var databaseNames = databaseInfoProvider.GetDatabaseNames();
                var script = new VaildateScriptProvider().GetValidateScripts(Initializer.Params.CheckLevel);
            
                var databaseValidator = new DatabaseValidator(databaseInfoProvider, script);


                var errorAgregator = new DatabaseErrorAgregator();

                foreach (var dbName in databaseNames)
                {
                    Trace.TraceInformation("Check {0} database...", dbName);
                    databaseValidator.CheckDatabase(dbName, errorAgregator);
                }

                errorAgregator.ShowSummary();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
