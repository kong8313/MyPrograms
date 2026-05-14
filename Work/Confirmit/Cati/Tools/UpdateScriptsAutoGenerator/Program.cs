using System;
using System.IO;
using System.Windows.Forms;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using UpdateScriptsAutoGenerator.Interfaces;

namespace UpdateScriptsAutoGenerator
{
    public class Program
    {
        private readonly ILogger _logger;
        private readonly IExternalInvoker _externalInvoker;
        private readonly IProgramEngine _programEngine;

        private readonly string _gitPath;
        private readonly string _msbuildPath;
        private readonly string _sqlPackagePath;
        private readonly string _databaseProjectRootPath;
        private readonly string _allProjectRootPath;
        private readonly string _hashName;
        private string _updateScriptNamePath;

        public const string DatabaseRootFolderName = "Confirmit.CATI.Database.2012";
        private const string ScriptsRootFolderName = "Scripts";
        private const string ScriptsDefinitionFileName = "ScriptsDefinitionFile.txt";
        private const string LocalPath = @"c:\_AutoGeneratorTempFolder";

        const string HelpInfo = @"Usage: UpdateScriptsAutoGenerator.exe [hash]
hash - it can be a real (full or short) hash of commit like 'a735209a' or a string like 'HEAD^N'
       empty hash means 'HEAD'";

        private Program(ILogger logger, IExternalInvoker externalInvoker, IProgramEngine programEngine, IPathProvider pathProvider, string hashName)
        {
            _logger = logger;
            _externalInvoker = externalInvoker;
            _programEngine = programEngine;
            _hashName = hashName;

            _logger.WriteLog(true, "Start generation");

            _gitPath = pathProvider.GetPathToGit();
            _msbuildPath = pathProvider.GetPathToMsBuild();
            _allProjectRootPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\"));
            _databaseProjectRootPath = Path.Combine(_allProjectRootPath, DatabaseRootFolderName);

            _sqlPackagePath = pathProvider.GetSqlPackageUtilityPath();
        }

        private void CheckoutUnmodifiedDatabaseProjectToExternalFolder()
        {
            if (Directory.Exists(LocalPath))
            {
                Directory.Delete(LocalPath, true);
            }

            Directory.CreateDirectory(LocalPath);

            _logger.WriteLog(true, "Getting a DB project related to commit '{0}' to a temp location '{1}'", _hashName, LocalPath);
            _externalInvoker.Invoke(_gitPath, $@"--work-tree={LocalPath} checkout {_hashName} -- {DatabaseRootFolderName}");
            _externalInvoker.Invoke(_gitPath, "reset HEAD");
        }

        private void BuildDbProjects()
        {
            File.Copy(Path.Combine(Application.StartupPath, @"..\..\GlobalAssemblyInfo.cs"), Path.Combine(LocalPath, "GlobalAssemblyInfo.cs"));

            _logger.WriteLog(true, "Build the current DB project");
            _externalInvoker.Invoke(_msbuildPath, Path.Combine(_allProjectRootPath, @"MSBuild\cati.proj /t:db"));

            _logger.WriteLog(true, "Build a DB project related to commit '{0}'", _hashName);
            _externalInvoker.Invoke(_msbuildPath, Path.Combine(LocalPath, DatabaseRootFolderName, @"Confirmit.CATI.Database\Confirmit.CATI.Database.sqlproj /p:SolutionDir=" + _allProjectRootPath));
        }

        private void GenerateUpdateScript()
        {
            _logger.WriteLog(true, "Start a comparison of two dacpac files");

            _externalInvoker.Invoke(_sqlPackagePath, string.Format("/a:Script /tdn:MyDatabase /p:BlockOnPossibleDataLoss=false /p:DropObjectsNotInSource=true /v:master=master /sf:\"{0}\" /tf:\"{1}\" /op:\"{2}\"",
                Path.Combine(_databaseProjectRootPath, @"Confirmit.CATI.Database\sql\Confirmit.CATI.Database.dacpac"),
                Path.Combine(LocalPath, DatabaseRootFolderName, @"Confirmit.CATI.Database\sql\Confirmit.CATI.Database.dacpac"),
                _updateScriptNamePath));

            string updateScriptContent = File.ReadAllText(_updateScriptNamePath);

            const string searchString = "USE [$(DatabaseName)];";
            int index = updateScriptContent.IndexOf(searchString, StringComparison.Ordinal);
            updateScriptContent = updateScriptContent.Substring(index + searchString.Length + 2).Replace(searchString, "");

            const string warningText = "RAISERROR('Look at the script and check that everything is correct, then remove these lines', 18, 0 );\r\n" +
                                       "RAISERROR('!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!', 18, 0 );\r\n";

            File.WriteAllText(_updateScriptNamePath, warningText + updateScriptContent);
        }

        private void AddNewUpdateScriptToScriptsDefinitionFile()
        {
            _logger.WriteLog(true, "Addition a new update script to the scripts definition file");

            string scriptsDefinitionFilePath = Path.Combine(_databaseProjectRootPath, ScriptsRootFolderName, ScriptsDefinitionFileName);

            _programEngine.AddUpdateScriptNameToScriptsDefinitionFile(scriptsDefinitionFilePath, _updateScriptNamePath);
        }

        private int Start()
        {
            _updateScriptNamePath = _programEngine.GetUpdateScriptNamePath(_gitPath, Path.Combine(_databaseProjectRootPath, ScriptsRootFolderName));

            CheckoutUnmodifiedDatabaseProjectToExternalFolder();

            BuildDbProjects();

            GenerateUpdateScript();

            AddNewUpdateScriptToScriptsDefinitionFile();

            _logger.WriteLog(true, "Execution has finished successfully");

            return 0;
        }

        private static void VerifyArguments(string[] args)
        {
            if (args.Length == 2 && (args[1] == "/?" || args[1].ToLowerInvariant() == "/help"))
            {
                throw new FormatException(HelpInfo);
            }

            if (args.Length > 2)
            {
                throw new FormatException("Count of parameters is wrong.\r\n" + HelpInfo);
            }
        }

        public static int Main()
        {
            ILogger logger = new FileAndConsoleLogger(Path.Combine(Application.StartupPath, "UpdateScriptsAutoGenerator.log"));

            try
            {
                string[] args = Environment.GetCommandLineArgs();

                VerifyArguments(args);

                string hashName = args.Length == 2 ? args[1] : "HEAD";
                IExternalInvoker externalInvoker = new ExternalInvoker(logger, 0);
                IProgramEngine programEngine = new ProgramEngine(logger, externalInvoker);
                IPathProvider pathProvider = new PathProvider();
                return new Program(logger, externalInvoker, programEngine, pathProvider, hashName).Start();
            }
            catch (FormatException ex)
            {
                logger.WriteLog(true, ex.Message);
                return 1;
            }
            catch (Exception ex)
            {
                logger.WriteLog(true, "An error occured during a program execution: {0}", ex.Message);
                logger.WriteLog(ex.ToString());
                return 1;
            }
        }
    }
}
