using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace DatabaseCheckUtility
{
    public class VaildateScriptProvider
    {
        public string[] GetValidateScripts(int checkLevel)
        {
            var script = new StringBuilder();
            var assembly = Assembly.GetExecutingAssembly();
            var resorces = assembly.GetManifestResourceNames();

            foreach (var resorce in resorces.Where(x => x.StartsWith("DatabaseCheckUtility.Sql.")))
            {
                using( var stream = assembly.GetManifestResourceStream(resorce))
                using (var reader = new StreamReader(stream))
                {
                    script.Append(reader.ReadToEnd());
                    script.AppendLine();
                    script.Append("GO");
                    script.AppendLine();
                }
            }
            var query = script.Replace("@CheckLevel", checkLevel.ToString()).ToString();

            var sc = new ServerConnection();
            sc.SqlExecutionModes = SqlExecutionModes.CaptureSql;
            sc.ExecuteNonQuery(query);

            return sc.CapturedSql.Text.Cast<string>().ToArray();

        }
    }
}