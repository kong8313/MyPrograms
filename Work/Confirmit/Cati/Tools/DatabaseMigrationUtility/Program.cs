using Confirmit.CATI.DatabaseUpdateLibrary;
using System;
using CommandLine;

namespace DatabaseMigrationUtility
{
    class Program
    {
        const string ScriptDescription = @"24.00\2019-05-17_11.08.36.ps1 CATI-2864";

        static void Main(string[] args)
        {
            try
            {
                MigratorOptions options = null;
                var parserResult = Parser.Default.ParseArguments<MigratorOptions>(args)
                    .WithParsed(parsed => options = parsed);
                if (parserResult.Tag != ParserResultType.Parsed)
                    return;

                var migrator = new Migrator();

                migrator.Initialize();

                var resources = new Resources();

                var scriptInfo = new UpdateScriptInfo(ScriptDescription);
                scriptInfo.ScriptText = resources.GetByName(scriptInfo.Name);

                var scopeText = options.CompanyId != null ? options.CompanyId + " company" : "All companies";

                migrator.Logger.WriteLog(true,
                    $"Do you really want to apply ${scriptInfo.Name} for {scopeText}?(Y/n)");

                if (Console.ReadKey().Key != ConsoleKey.Y)
                {
                    migrator.Logger.WriteLog(true, $"Migration was canceled!");
                    return;
                }

                migrator.ExecuteScript(options, scriptInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
