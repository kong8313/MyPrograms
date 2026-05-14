using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Build.Utilities;

using StaticTeamCityBuildEngine.CommonEngines;
using StaticTeamCityBuildEngine.Interfaces;

namespace StaticTeamCityBuildEngine
{
    public class CreateBranchFilesEngine
    {
        private readonly TaskLoggingHelper _logger;
        private readonly IExternalExecutor _externalExecutor;
        public readonly bool RecreateExistingFiles;

        public string ExecutablePath { get; private set; }

        public CreateBranchFilesEngine(TaskLoggingHelper logger, IExternalExecutor externalExecutor)
            : this(logger, externalExecutor, "True")
        {
        }

        public CreateBranchFilesEngine(TaskLoggingHelper logger, IExternalExecutor externalExecutor, string recreateExistingFiles)
        {
            _logger = logger;
            _externalExecutor = externalExecutor;
            if (!bool.TryParse(recreateExistingFiles, out RecreateExistingFiles))
            {
                RecreateExistingFiles = true;
                _logger.LogError("Parameter RecreateExistingFiles has wrong value: '{0}'. It was set to 'True'", recreateExistingFiles);
            }
            

            ExecutablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _logger.LogMessage("ExecutablePath=" + ExecutablePath);
        }

        public string GetLastCommitHash(string lastHash)
        {
            // if no LashHash was sent to this method - try to get it from Git
            return string.IsNullOrEmpty(lastHash) 
                ? _externalExecutor.ExecuteGitUtility("log -1 --pretty=format:\"%h\"")
                : lastHash.Substring(0, Math.Min(7, lastHash.Length));
        }

        public string GetSideBySideName(string defaultSideBySideName)
        {
            string sideBySideName = defaultSideBySideName;

            // if no SideBySideName was sent to this method - get root folder name, if it starts from FolderName.sxs. Otherwise set "Rel"
            if (string.IsNullOrEmpty(sideBySideName))
            {
                string parentFolderName = Directory.GetParent(ExecutablePath).Name;
                string[] folderNameParts = parentFolderName.ToLowerInvariant().Split(new[] { ".sxs." }, StringSplitOptions.RemoveEmptyEntries);
                sideBySideName = folderNameParts.Length == 2
                    ? folderNameParts[1]
                    : "Rel";
                
                _logger.LogMessage("sideBySideName=" + sideBySideName);
            }

            return sideBySideName.Replace(' ', '_').Replace('.', '_');
        }

        /// <summary>
        /// Return server path like this:
        /// $/Confirmit/Products/Fusion.Package/Main
        /// </summary>
        /// <returns></returns>
        public string GetSourcePath()
        {
            return Directory.GetParent(ExecutablePath.TrimEnd('\\')).FullName;
        }

        private bool NeedToGenerateFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            if (!RecreateExistingFiles && File.Exists(filePath))
            {
                _logger.LogMessage("Creation of {0} file was skipped because RecreateExistingFiles parameter is false and file exists", fileName);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create Confirmit.CATI.Common\SideBySide\sideBySideName.cs file with information about global instance name
        /// </summary>
        /// <param name="sideBySideName">Global instace name</param>
        public void CreateSideBySideNameFile(string sideBySideName)
        {
            _logger.LogMessage("Creating SideBySideName.cs file");

            string sideBySideFilePath = GetSideBySidePath();
            _logger.LogMessage("sideBySideFilePath=" + sideBySideFilePath);

            if (!NeedToGenerateFile(sideBySideFilePath))
            {
                return;
            }

            string fileContent = @"/* This file are generated automatically during build of _BuildProject\\_BuildProject.csproj project */
namespace Confirmit.CATI.Common.SideBySide
{
    internal class SideBySide
    {
        private static string _sideBySideName = """ + sideBySideName + @""";

        /// <summary>
        /// Global instance name for this branch.
        /// Do not change it excluding tests
        /// </summary>
        internal static string SideBySideName
        { 
            get 
            {
                return _sideBySideName;
            }

            set
            {
                _sideBySideName = value;
            }
        }
    }
}
";
            RecreateFileIfContentIsNew(sideBySideFilePath, fileContent);
        }

        /// <summary>
        /// Create d:\Project.Main\Confirmit.CATI.Setup\BranchInfo.wxs file with information about current branch for installations
        /// </summary>
        /// <param name="sideBySideName">Global instace name</param>
        public void CreateBranchInfoFile(string sideBySideName)
        {
            _logger.LogMessage("Creating BranchInfo.wxs file");

            string branchInfoFilePath = GetBranchInfoPath();
            _logger.LogMessage("branchInfoFilePath=" + branchInfoFilePath);

            if (!NeedToGenerateFile(branchInfoFilePath))
            {
                return;
            }

            string[] guids = CreateGuilds(sideBySideName, 16);

            string fileContent = string.Format(
                "<!-- This file are generated automatically during build of _BuildProject\\_BuildProject.csproj project -->\r\n" +
                "<Include>\r\n\r\n" +

                "<?define SideBySideName=\"{0}\" ?>\r\n\r\n" +

                "<?if $(var.ProcessorArchitecture)=x64 ?>\r\n" +
                "<?define ClientDeploymentUpgradeGUID={2} ?>\r\n" +
                "<?else ?>\r\n" +
                "<?define ClientDeploymentUpgradeGUID={3} ?>\r\n" +
                "<?endif ?>\r\n\r\n" +

                "<?if $(var.ProcessorArchitecture)=x64 ?>\r\n" +
                "<?define GenericWsUpgradeGUID={8} ?>\r\n" +
                "<?else ?>\r\n" +
                "<?define GenericWsUpgradeGUID={9} ?>\r\n" +
                "<?endif ?>\r\n\r\n" +

                "<?if $(var.ProcessorArchitecture)=x64 ?>\r\n" +
                "<?define SimulatorGWsUpgradeGUID={12} ?>\r\n" +
                "<?else ?>\r\n" +
                "<?define SimulatorGWsUpgradeGUID={13} ?>\r\n" +
                "<?endif ?>\r\n\r\n" +

                "<?if $(var.ProcessorArchitecture)=x64 ?>\r\n" +
                "<?define LtuSimulatorGWsUpgradeGUID={15} ?>\r\n" +
                "<?else ?>\r\n" +
                "<?define LtuSimulatorGWsUpgradeGUID={16} ?>\r\n" +
                "<?endif ?>\r\n\r\n" +

                "</Include>\r\n",
                sideBySideName,
                string.Empty,
                guids[1],
                guids[2],
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                guids[7],
                guids[8],
                string.Empty,
                string.Empty,
                guids[11],
                guids[12],
                string.Empty,
                guids[14],
                guids[15]);

            RecreateFileIfContentIsNew(branchInfoFilePath, fileContent);
        }

        /// <summary>
        /// Create GlobalAssemblyInfo with new information
        /// </summary>
        /// <param name="globalInfo">Information about product</param>
        public void CreateGlobalAssemblyInfo(GlobalInfo globalInfo)
        {
            _logger.LogMessage("Creating CreateGlobalAssemblyInfo.cs file");

            string globalAssemblyInfoPath = GetGlobalAssemblyInfoPath();
            _logger.LogMessage("globalAssemblyInfoPath=" + globalAssemblyInfoPath);

            if (!NeedToGenerateFile(globalAssemblyInfoPath))
            {
                return;
            }

            string fileContent = string.Format(
                "/* This file are generated automatically during build of _BuildProject\\_BuildProject.csproj project */" +
                " using System.Reflection;\r\n" +
                " // Version information for an assembly consists of the following four values:\r\n" +
                " //\r\n" +
                " //      Major Version\r\n" +
                " //      Minor Version\r\n" +
                " //      Build Number\r\n" +
                " //      Revision\r\n" +
                " //\r\n" +
                " [assembly: AssemblyVersion(\"{0}\")]\r\n" +
                " [assembly: AssemblyFileVersion(\"{0}\")]\r\n" +
                " [assembly: AssemblyCompany(\"{2}\")]\r\n" +
                " [assembly: AssemblyCopyright(\"{3}\")]\r\n" +
                " [assembly: AssemblyTrademark(\"{4}\")]\r\n" +
                " [assembly: AssemblyProduct(\"{5} {1}\")]\r\n",
                globalInfo.BuildNumber,
                globalInfo.Title,
                globalInfo.CompanyName,
                globalInfo.LegalCopyright,
                globalInfo.LegalTrademarks,
                globalInfo.ProductName);

            RecreateFileIfContentIsNew(globalAssemblyInfoPath, fileContent);
        }

        /// <summary>
        /// Create Directory.Build.props file with new information
        /// </summary>
        /// <param name="globalInfo">Information about product</param>
        public void CreateDirectoryBuildProps(GlobalInfo globalInfo)
        {
            _logger.LogMessage("Creating Directory.Build.props file");

            string directoryBuildPropsPath = GetDirectoryBuildPropsPath();
            _logger.LogMessage("directoryBuildPropsPath=" + directoryBuildPropsPath);

            if (!NeedToGenerateFile(directoryBuildPropsPath))
            {
                return;
            }
            
            string fileContent = 
$@"<Project>
  <PropertyGroup>
    <Version>{globalInfo.BuildNumber}</Version>
    <FileVersion>{globalInfo.BuildNumber}</FileVersion>
    <Company>{globalInfo.CompanyName}</Company>
    <Copyright>{globalInfo.LegalCopyright}</Copyright>
    <Product>{globalInfo.ProductName} {globalInfo.Title}</Product>
    <Authors>{globalInfo.LegalTrademarks}</Authors>
  </PropertyGroup>
</Project>";

            RecreateFileIfContentIsNew(directoryBuildPropsPath, fileContent);
        }

        public void CreateCatiBuildNumberFile(GlobalInfo globalInfo)
        {
            _logger.LogMessage("Create CreateCatiBuildNumberFile.cs file (for Confirmit.CATI.DialerInterface)");

            string catiBuildNumberPath = GetCatiBuildNumberPath();
            _logger.LogMessage("catiBuildNumberPath=" + catiBuildNumberPath);

            if (!NeedToGenerateFile(catiBuildNumberPath))
            {
                return;
            }

            var version = new Version(globalInfo.BuildNumber);
            string fileContent = string.Format(
                "namespace Confirmit.CATI.Build\r\n" +
                "{{\r\n" +
                "   static class CatiBuildNumber\r\n" +
                "   {{\r\n" +
                "       public const string Value = \"{0}\";\r\n" +
                "   }}\r\n" +
                "}}",
                version.Build);

            RecreateFileIfContentIsNew(catiBuildNumberPath, fileContent);
        }

        public void RecreateFileIfContentIsNew(string filePath, string fileContent)
        {
            if (File.Exists(filePath))
            {
                FileAttributes savedFileAttributes = File.GetAttributes(filePath);

                if (File.ReadAllText(filePath) == fileContent)
                {
                    _logger.LogMessage("The content of file is the same. Don't recreate file.");
                    return;
                }

                File.SetAttributes(filePath, FileAttributes.Normal);

                File.WriteAllText(filePath, fileContent);

                File.SetAttributes(filePath, savedFileAttributes);
            }
            else
            {
                File.WriteAllText(filePath, fileContent);
            }

            _logger.LogMessage("The creation has finished successfully");
        }

        private string[] CreateGuilds(string sideBySideName, int guidsCount)
        {
            var baseGuids = new string[guidsCount];
            string previousValue = sideBySideName;

            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0; i < guidsCount; i++)
                {
                    byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(previousValue));
                    baseGuids[i] = new Guid(hash).ToString().ToUpper();
                    previousValue = baseGuids[i];
                }
            }

            return baseGuids;
        }

        public bool IsAutoGeneratedFilesAvailable()
        {
            string branchInfoFilePath = GetBranchInfoPath();
            string sideBySideNameFilePath = GetSideBySidePath();
            string globalAssemblyInfoPath = GetGlobalAssemblyInfoPath();
            string catiBuildNumberPath = GetCatiBuildNumberPath();

            _logger.LogMessage("branchInfoFilePath=" + branchInfoFilePath);
            _logger.LogMessage("sideBySideNameFilePath=" + sideBySideNameFilePath);
            _logger.LogMessage("globalAssemblyInfoPath=" + globalAssemblyInfoPath);
            _logger.LogMessage("catiBuildNumberPath=" + catiBuildNumberPath);

            return File.Exists(branchInfoFilePath) && File.Exists(sideBySideNameFilePath) && 
                   File.Exists(globalAssemblyInfoPath) && File.Exists(catiBuildNumberPath);
        }

        private string GetBranchInfoPath()
        {
            return Path.GetFullPath(Path.Combine(ExecutablePath, @"..\Confirmit.CATI.Setup\BranchInfo.wxs"));
        }

        private string GetSideBySidePath()
        {
            return Path.GetFullPath(Path.Combine(ExecutablePath, @"..\Confirmit.CATI.Common\SideBySide\SideBySide.cs"));
        }

        private string GetGlobalAssemblyInfoPath()
        {
            return Path.GetFullPath(Path.Combine(ExecutablePath, @"..\GlobalAssemblyInfo.cs"));
        }

        private string GetDirectoryBuildPropsPath()
        {
            return Path.GetFullPath(Path.Combine(ExecutablePath, @"..\Directory.Build.props"));
        }

        private string GetCatiBuildNumberPath()
        {
            return Path.GetFullPath(Path.Combine(ExecutablePath, @"..\CatiBuildNumber.cs"));
        }
        
    }
}
