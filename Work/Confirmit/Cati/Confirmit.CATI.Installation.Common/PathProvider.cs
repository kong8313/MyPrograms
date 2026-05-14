using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Installation.Common
{
    public class PathProvider : IPathProvider
    {
        private readonly string[] _msTestPossiblePaths =
        {
            @"Microsoft Visual Studio\2019\Enterprise\Common7\IDE\MSTest.exe",
            @"Microsoft Visual Studio\2019\Professional\Common7\IDE\MSTest.exe",
            @"Microsoft Visual Studio\2019\TestAgent\Common7\IDE\MSTest.exe"
        };

        private string GetPathToProgramFiles()
        {
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            if (!Directory.Exists(programFilesPath))
            {
                programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }

            return programFilesPath;
        }

        public string GetStartupPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        }

        public string GetSqlPackageUtilityPath()
        {
            string backStep = "";
            do
            {
                string sqlPackageUtilityPath = Path.Combine(GetStartupPath(), backStep + @"_3rdpart\SqlDbDac\sqlpackage.exe");

                if (File.Exists(sqlPackageUtilityPath))
                {
                    return sqlPackageUtilityPath;
                }

                backStep += @"..\";
            } while (backStep.Length < 15);


            throw new Exception(@"Path to _3rdpart\SqlDbDac\sqlpackage.exe file was not found.");
        }

        public string GetPathToGit()
        {
            string pathsVariable = Environment.GetEnvironmentVariable("Path") ?? string.Empty;

            string[] paths = pathsVariable.Split(';');
            string gitCmdPath = paths.First(x => x.Contains(@"\Git\")); // something like this: C:\Program Files (x86)\Git\cmd

            if (string.IsNullOrEmpty(gitCmdPath))
            {
                throw new Exception("Git path was not found among Path environment variables");
            }

            return Path.Combine(Directory.GetParent(gitCmdPath).FullName, @"bin\git.exe");
        }

        public string GetPathToMsBuild()
        {
            string msBuildPath = Path.Combine(GetPathToProgramFiles(), @"Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe");

            if (!File.Exists(msBuildPath))
            {
                msBuildPath = Path.Combine(GetPathToProgramFiles(), @"Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe");

                if (!File.Exists(msBuildPath))
                {
                    throw new Exception($"'{msBuildPath}' file does not exists. Install Visual Studio 2019 Professional or Enterprise");
                }
            }

            return msBuildPath;
        }

        public string GetPathToMsTest()
        {
            string programFilesPath = GetPathToProgramFiles();
            var checkedPaths = new StringBuilder();
            foreach (var msTestPossiblePath in _msTestPossiblePaths)
            {
                var msTestPath = Path.Combine(programFilesPath, msTestPossiblePath);

                if (File.Exists(msTestPath))
                {
                    return msTestPath;
                }

                checkedPaths.AppendLine(msTestPath);
            }

            throw new Exception("Path to mstest.exe file has not found. The following paths were checked:\r\n" + checkedPaths);
        }
    }
}