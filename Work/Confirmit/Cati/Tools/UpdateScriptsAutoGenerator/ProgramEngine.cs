using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using UpdateScriptsAutoGenerator.Interfaces;

namespace UpdateScriptsAutoGenerator
{
    public class ProgramEngine : IProgramEngine
    {
        private readonly ILogger _logger;
        private readonly IExternalInvoker _externalInvoker;

        public ProgramEngine(ILogger logger, IExternalInvoker externalInvoker)
        {
            _logger = logger;
            _externalInvoker = externalInvoker;
        }

        public static ConsoleKey AskQuestion(string message)
        {
            Console.Write(message);
            ConsoleKey key = Console.ReadKey().Key;
            Console.WriteLine();
            return key;
        }

        public string GetUpdateScriptNamePath(string gitPath, string scriptFolderRootPath)
        {
            Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string scriptFolderPath = Path.Combine(scriptFolderRootPath, string.Format("{0}.{1}", FormatNumber(assemblyVersion.Major), FormatNumber(assemblyVersion.Minor)));
            string lastUpdateScriptPath = GetNotCommitedUpdateScriptPath(gitPath, scriptFolderPath);

            if (!Directory.Exists(scriptFolderPath))
            {
                Directory.CreateDirectory(scriptFolderPath);
            }

            string scriptName;
            if (!string.IsNullOrEmpty(lastUpdateScriptPath))
            {
                ConsoleKey key = AskQuestion("The utiltiy has found out that your last update script isn't committed to Git.\r\nWhat the utility should do?\r\nPress 1 to rewrite the existed file.\r\nPress 2 to backup the existed file.\r\nPress any key to stop execution.\r\n");

                if (key == ConsoleKey.D2)
                {
                    string backupLastUpdateScriptPath = lastUpdateScriptPath + ".backup";
                    int i = 0;
                    do
                    {
                        i++;
                    } while (File.Exists(backupLastUpdateScriptPath + i));

                    File.Move(lastUpdateScriptPath, backupLastUpdateScriptPath + i);
                }
                else if (key != ConsoleKey.D1)
                {
                    throw new Exception("Stop execution by user request");
                }

                scriptName = Path.GetFileName(lastUpdateScriptPath);
            }
            else
            {
                scriptName = string.Format("{0}.sql", DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss"));
            }

            _logger.WriteLog("Update script name: {0}", scriptName);

            return Path.GetFullPath(Path.Combine(scriptFolderPath, scriptName));
        }

        /// <summary>
        /// We have to understand, does git see uncommitted new update scripts or not
        /// We have to run 'git.exe status -s' command to understand this.
        /// We need to find out from output a string started with a letter 'A' and looks like new update script
        /// If we can't find out such string, then we have no uncommitted new update script
        /// </summary>
        /// <param name="gitPath">Path to git.exe utility</param>
        /// <param name="scriptFolderPath">Path to folder with update script files</param>
        /// <returns></returns>
        private string GetNotCommitedUpdateScriptPath(string gitPath, string scriptFolderPath)
        {
            int tempIndex = scriptFolderPath.IndexOf(Program.DatabaseRootFolderName, StringComparison.Ordinal);
            string updateScriptPartPath = scriptFolderPath.Substring(tempIndex).Replace("\\", "/");

            _logger.WriteLog(true, TraceEventType.Information, "The utiltiy is looking for an uncommitted new update script file");
            string gitResponse = _externalInvoker.Invoke(gitPath, string.Format("status -s"));
            _logger.WriteLog("gitResponse=" + gitResponse);

            string gitResponseLine = gitResponse.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(
                x => (x.StartsWith("A") || x.StartsWith("??")) && x.Contains(updateScriptPartPath) && !x.Contains("backup"));

            if (string.IsNullOrEmpty(gitResponseLine))
            {
                return string.Empty;
            }

            int index = gitResponseLine.LastIndexOf('/');
            return Path.Combine(scriptFolderPath, gitResponseLine.Substring(index + 1).TrimEnd(new[] { '\r', '\n', ' ' }));
        }

        private string FormatNumber(int intNumber)
        {
            string number = intNumber.ToString(CultureInfo.InvariantCulture);

            if (number.Length == 1)
            {
                return "0" + number;
            }

            if (number.Length > 2)
            {
                throw new Exception("A number '" + number + "' is too big. The number should be less then 99");
            }

            if (number.Length == 0)
            {
                throw new Exception("Internal error: 'FormatNumber' method got empty number");
            }

            return number;
        }

        public void AddUpdateScriptNameToScriptsDefinitionFile(string scriptsDefinitionFilePath, string updateScriptNamePath)
        {
            string fileContent = File.ReadAllText(scriptsDefinitionFilePath);

            string scriptFolderPath = Path.GetDirectoryName(scriptsDefinitionFilePath) ?? string.Empty;
            string relatedScriptFilePath = updateScriptNamePath.Substring(scriptFolderPath.Length + 1);

            string lastString = string.Empty;
            if (fileContent.Length > 0)
            {
                lastString = fileContent.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Last();
            }

            if (!lastString.StartsWith(relatedScriptFilePath))
            {
                if (fileContent.Length > 0)
                {
                    fileContent = fileContent.TrimEnd(new[] { '\r', '\n' }) + "\r\n";
                }

                fileContent += relatedScriptFilePath + " ";

                File.WriteAllText(scriptsDefinitionFilePath, fileContent);
            }
        }
    }
}