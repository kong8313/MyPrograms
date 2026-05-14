using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DatabaseCheckUtility
{
    public class DatabaseErrorAgregator
    {
        Dictionary<string, List<string>> DbName2Errors = new Dictionary<string, List<string>>();

        public void OnError(string dbName, string message)
        {
            if (!DbName2Errors.ContainsKey(dbName))
            {
                DbName2Errors[dbName] = new List<string>();
            }

            DbName2Errors[dbName].Add(message);
        }

        internal void ShowSummary()
        {
            Trace.TraceInformation("Summary( Check Level = {0} ):", Initializer.Params.CheckLevel);
            foreach (var dbErrors in DbName2Errors)
            {
                foreach (var error in dbErrors.Value)
                {
                    Trace.TraceError("{0} {1}", dbErrors.Key, error);
                }
            }
        }
    }
}
