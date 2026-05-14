using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace ClickOnceConsoleInstallationTest
{
    [TestClass]
    public class ConsoleInstallation
    {
        public TestContext TestContext { get; set; } // for data source using

        InPlaceHostingManager _iphm;
        Uri _deploymentUri;
        readonly ManualResetEvent _manifestDownloadEvent = new ManualResetEvent(false);
        readonly ManualResetEvent _applicationDownloadEvent = new ManualResetEvent(false);
        string _downloadApplicationResult;
        string _applicationTitle;
        string _applicationShortcut;
        long _bytesDownloaded;
        long _totalBytesToDownload;
        long _progressPercentage;
        GetManifestCompletedEventArgs _getManifestCompletedEventArgs;
        private ConsoleLogger _consoleLogger;

        static void DomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            string msg = string.Format(
                "ClickOnceConsoleInstallationTest: Unhandled exception occured: {0}\r\nIsTerminating: {1}\r\nSender: {2}",
                e.ExceptionObject,
                e.IsTerminating,
                sender);

            // Try to write to the output
            try
            {
                Trace.TraceError(msg);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception) { }

            // And to the file
            try
            {
                const string directoryName = "C:\\!!!CodedUITestsCrashReports\\";
                Directory.CreateDirectory(directoryName);
                var fileName = directoryName + DateTime.Now.ToString("yyyy-MM-dd  hh-mm-ss") + ".txt";

                using (var outfile = new StreamWriter(fileName))
                {
                    outfile.Write(msg);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to write unhandled exception error to the file. \r\n {0}", ex);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _consoleLogger = new ConsoleLogger();

            AppDomain.CurrentDomain.UnhandledException += DomainUnhandledExceptionHandler;
            Debug.Assert(TestContext != null, "TestContext != null");
            CheckThereIsNoConsoleStarted("CatiInterviewerConsole");
            DeleteFolderWithLicenseAgreementFile(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\Confirmit\CATI"));
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "..\\..\\..\\..\\ConsoleData\\ClickOnceURLs.xml", ServerInfo.TableName, DataAccessMethod.Sequential)]
        public void ConsoleClickOnceInstallUninstallTest()
        {
            string clickOnceUrl = TestContext.DataRow["deployManifestUriStr"].ToString();
            if (String.IsNullOrEmpty(clickOnceUrl))
            {
                return;
            }
        
            // Download application manifest
            GetManifest(clickOnceUrl);

            // Verify this application can be installed
            VerifyRequirements();

            // Download application
            DownloadApplication();

            if (Boolean.Parse(TestContext.DataRow["keepLocalcopy"].ToString()))
                CopyConsoleFilesToSubFolderInAssemblies();

            // Uninstall application
            UninstallApplication();
        }


        private void CopyConsoleFilesToSubFolderInAssemblies()
        {
            /*
             * this function does:
             * 1.check there are no cati console process already started
             * 2.run cati console via clickonce shortcut
             * 3.wait console process started
             * 4.get cati console location folder 
             * 5.kill cati console             
             * * 6.copy cati console files include subfolders to <project>\assemblies\OlympicTest 
             */

            CheckThereIsNoConsoleStarted("CatiInterviewerConsole");
            Thread.Sleep(2000);

            // run console via clickOnce shortcut
            _consoleLogger.Log(@"Run cati console by clickonce link...");
            using (Process.Start("rundll32.exe",
                "dfshim.dll,ShOpenVerbApplication " +
                _applicationShortcut.Substring(0, _applicationShortcut.IndexOf("#", StringComparison.Ordinal))))
            {
                _consoleLogger.Log(@"... console started, find path to exe");
                Thread.Sleep(5000); // extra time to start console console
                var pathToConsole = GetConsoleProcessLocationFolderAndKill("CatiInterviewerConsole");
                _consoleLogger.Log($"path to console: {pathToConsole}");
                int pos1 = TestContext.TestRunDirectory.LastIndexOf("QA\\TestCode", StringComparison.Ordinal);
                if (pos1 > 0)
                {
                    var pathToCopyConsole = TestContext.TestRunDirectory.Substring(0, pos1) +
                                            "assemblies\\OlympicTest";

                    DirectoryCopy(pathToConsole, pathToCopyConsole, true);
                }
            }
        }

        void GetManifest(string deployManifestUriStr)
        {
            try
            {
                _deploymentUri = new Uri(deployManifestUriStr);
                _iphm = new InPlaceHostingManager(_deploymentUri, false); //Create a new instance of InPlaceHostingManager
            }
            catch (UriFormatException uriEx)
            {
                throw new Exception(String.Format("Cannot install the application for URL={0}: " +
                    "The deployment manifest URL supplied is not a valid URL. " +
                    "Error: {1}", _deploymentUri,uriEx.Message));
            }
            catch (PlatformNotSupportedException platformEx)
            {
                throw new Exception(String.Format("Cannot install the application for URL={0}: " +
                    "This program requires Windows XP or higher. " +
                    "Error: {1}", _deploymentUri,platformEx.Message));
            }
            catch (ArgumentException argumentEx)
            {
                throw new Exception(String.Format("Cannot install the application for URL={0}: " +
                    "The deployment manifest URL supplied is not a valid URL. " +
                    "Error: {1}", _deploymentUri,argumentEx.Message));
            }

            //add event handler for Manifest download completeted event
            _iphm.GetManifestCompleted += _iphm_GetManifestCompleted;

            // Download manifest
            _iphm.GetManifestAsync();
            _manifestDownloadEvent.WaitOne();

            if (_getManifestCompletedEventArgs.Error != null)
            {
                Assert.Fail("Could not download manifest for URL={0}. Error: {1}", _deploymentUri, _getManifestCompletedEventArgs.Error.Message);
            }            

            _applicationTitle = _getManifestCompletedEventArgs.ProductName;
        }

        // event handler for Manifest download completeted event
        private void _iphm_GetManifestCompleted(object sender, GetManifestCompletedEventArgs e)
        {
            _getManifestCompletedEventArgs = e;
            _manifestDownloadEvent.Set();
        }

        // Verify this application can be installed
        void VerifyRequirements()
        {            
            try
            {
                // the true parameter allows InPlaceHostingManager
                // to grant the permissions requested in the applicaiton manifest.
                _iphm.AssertApplicationRequirements(true);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("An error occurred while verifying the application for URL={0}. Error: {1}", _deploymentUri, ex.Message));
            }
        }

        // Download the deployment manifest. 
        void DownloadApplication()
        {
            //add event handlers for download progress changed event
            _iphm.DownloadProgressChanged += _iphm_DownloadProgressChanged;
            //add event handlers for download application completed event
            _iphm.DownloadApplicationCompleted += _iphm_DownloadApplicationCompleted;

            try
            {
                // Usually this shouldn't throw an exception unless AssertApplicationRequirements() failed, 
                // or you did not call that method before calling this one.
                _iphm.DownloadApplicationAsync();
            }
            catch (Exception downloadEx)
            {
                throw new Exception(String.Format("Cannot initiate download of application for URL={0}. Error: {1}", _deploymentUri, downloadEx.Message));
            }
            _applicationDownloadEvent.WaitOne();

            // Check for an error.
            Assert.AreEqual(null, _downloadApplicationResult,
                String.Format("Could not download and install application for URL={0}. Error: {1}, " +
            "percentage completed={2}, bytes downloaded={3}, total bytes to download={4}.", 
            _deploymentUri,_downloadApplicationResult,_progressPercentage,_bytesDownloaded,_totalBytesToDownload));
        }

        // event handler for download application completed event
        void _iphm_DownloadApplicationCompleted(object sender, DownloadApplicationCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _downloadApplicationResult = e.Error.Message;
            }
            _applicationShortcut = e.ShortcutAppId;
            _applicationDownloadEvent.Set();
        }

        // event handler for download progress changed event
        void _iphm_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {   // application download process variable's initialization to pass results to assert message in case of download's fail
            _progressPercentage = e.ProgressPercentage; // percentage of task completed variable initialized
            _bytesDownloaded = e.BytesDownloaded; // bytes downladed variable initialized
            _totalBytesToDownload = e.TotalBytesToDownload; // total bytes to download in application variable initialized
        }

        // uninstall application
        void UninstallApplication()
        {
            string publicKeyTokenString = GetPublicKeyToken();
    
            // Find Uninstall string in registry
            string uninstallString = GetUninstallString(publicKeyTokenString);
            Assert.AreNotEqual(string.Empty, uninstallString,
                String.Format("No application key with {0} and displayName = {1} in registry",
                publicKeyTokenString, _applicationTitle));

            string runDll32 = uninstallString.Substring(0, 12);
            string args = uninstallString.Substring(13);

            _consoleLogger.Log($"Run uninstall. runDll32={runDll32}, args={args}");

            //start the uninstall; this will bring up the uninstall dialog
            //  asking if it's ok            
            using (Process uninstallProcess = Process.Start(runDll32, args))
            {
                _consoleLogger.Log("Before PushUninstallOkButton");
                PushUninstallOkButton(_applicationTitle);

                _consoleLogger.Log("Before WaitForExit");
                //waiting for uninstall process to exit correctly
                Debug.Assert(uninstallProcess != null, "uninstallProcess != null",
                    String.Format("Uninstall process unexpectedly got null value for {0}",
                        TestContext.DataRow["deployManifestUriStr"]));
                uninstallProcess.WaitForExit();
            }

            _consoleLogger.Log("Finish uninstall");
        }

        // Gets the uninstall string for the current ClickOnce app
        // from the Windows Registry.
        // <returns>The command line to execute that will
        // uninstall the app.</returns>
        public string GetUninstallString(string publicKeyTokenString)
        {
            string uninstallString = null;
            
            //open the registry key and get the subkey names
            RegistryKey uninstallKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
            Debug.Assert(uninstallKey != null, "uninstallKey != null");
            string[] appKeyNames = uninstallKey.GetSubKeyNames();

            bool found = false;

            //search through the list for one with a match
            foreach (string appKeyName in appKeyNames)
            {
                RegistryKey appKey = uninstallKey.OpenSubKey(appKeyName);
                Debug.Assert(appKey != null, "appKey != null");
                uninstallString = (string)appKey.GetValue("UninstallString");
                string displayName = (string)appKey.GetValue("displayName");
                appKey.Close();
                if (uninstallString.Contains(publicKeyTokenString)
                  && displayName == _applicationTitle)
                {
                    found = true;
                    break;
                }
            }

            uninstallKey.Close();

            if (found)
            {
                return uninstallString;
            }
            
            return string.Empty;
        }
        
        // <summary>
        // Find and Push the OK button on the uninstall dialog.
        // </summary>
        // <param name="displayName">Display Name value from the registry</param>
        public static void PushUninstallOkButton(string displayName)
        {
            bool success;

            //Find the uninstall dialog.
            AutomationElement uninstallerWin =
              FindUninstallerWindow(displayName, out success);

            AutomationElement okButton = null;
            //If it found the window, look for the button.
            if (success)
                okButton = FindUninstallerOkButton(uninstallerWin, out success);

            //If it found the button, press it.
            //success = false;
            if (success)
                DoButtonClick(okButton);          
        }
        
        // <summary>
        // Find the uninstall dialog.
        // </summary>
        // <param name="displayName">Display Name retrieved
        // from the registry.</param>
        // <param name="success">Whether the window was found or not.</param>
        // <returns>Pointer to the uninstall dialog.</returns>
        private static AutomationElement FindUninstallerWindow(string displayName, out bool success)
        {
            //Max number of times to look for the window,
            //used to let you out if there's a problem.
            AutomationElement uninstallerWindow = null;
            int i = 25;
            while (uninstallerWindow == null && i > 0)
            {
                uninstallerWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, displayName + " Maintenance"));
                Thread.Sleep(500);
                i--;
            }

            if (uninstallerWindow == null)
                success = false;
            else
                success = true;

            return uninstallerWindow;
        }

        // <summary>
        // Find the OK button on the uninstall dialog.
        // </summary>
        // <param name="uninstallerWindow">The pointer to
        // the Uninstall Dialog</param>
        // <param name="success">Whether it succeeded or not.</param>
        // <returns>A pointer to the OK button</returns>
        public static AutomationElement FindUninstallerOkButton(AutomationElement uninstallerWindow,
          out bool success)
        {
            //max number of times to look for the button,
            //lets you out if there's a problem
            int i = 25;

            AutomationElement okButton = null;
            success = false;
            while (i > 0)
            {
                okButton = uninstallerWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "OK"));
                if (okButton != null)
                {
                    success = true;
                    break;
                }
                Thread.Sleep(500);
                i--;
            }

            return okButton;
        }
        public static void DoButtonClick(AutomationElement uiButton)
        {
            GetInvokePattern(uiButton).Invoke();
        }

        public static InvokePattern GetInvokePattern(AutomationElement element)
        {
            return element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
        }

        private string GetPublicKeyToken()
        {
            Debug.Assert(_applicationShortcut.Length > 0,
                "DownloadApplicationCompletedEventArgs.ShortcutAppId is empty");
            int startPos = _applicationShortcut.IndexOf("PublicKeyToken=", StringComparison.Ordinal);
            Assert.AreNotEqual(-1, startPos,
                "No PublicKeyToken in DownloadApplicationCompletedEventArgs.ShortcutAppId");
            string publicKeyTokenString =
                _applicationShortcut.Substring(startPos);
            int endPos = publicKeyTokenString.IndexOf(",", StringComparison.Ordinal);
            if (endPos != -1)
            {
                publicKeyTokenString = publicKeyTokenString.Substring(0, endPos);
            }
            return publicKeyTokenString;
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory exists, delete it.
            if (Directory.Exists(destDirName))
                Directory.Delete(destDirName,true);
            
            Directory.CreateDirectory(destDirName);
            
            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subDir.Name);
                    DirectoryCopy(subDir.FullName, tempPath, copySubDirs: true);
                }
            }
        }

        private void CheckThereIsNoConsoleStarted(string name)
        {
            _consoleLogger.Log("Kill all working CATI consoles if they exist");
            foreach (var process in Process.GetProcessesByName(name))
            {
                _consoleLogger.Log($"Kill process with id {process.Id} and name {process.ProcessName}");
                process.Kill();
            }

            _consoleLogger.Log("All CATI consoles processes were killed");
        }

        private string GetConsoleProcessLocationFolderAndKill(string name)
        {
            string pathToConsole = String.Empty;

            // 30 sec(loopMax * 500ms) loop to wait console process has been started
            const int loopMax = 60;
            for (int i = 0; i <= loopMax; i++)
            {
                Thread.Sleep(500);
                var processes = Process.GetProcessesByName(name);
                if (processes.Length == 0 && i < loopMax)
                {
                    continue;
                }

                if (processes.Length != 1)
                {
                    Assert.Fail("Error!No one or more than one ({0})process with name {1} after {2} iterations is run",
                        processes.Length, name, i);
                }

                var proc = processes[0];
                _consoleLogger.Log($"{proc.Id},{proc.ProcessName},{proc.StartInfo.FileName}");
                var pathToExe = proc.MainModule.FileName;
                _consoleLogger.Log(pathToExe);
                if (string.IsNullOrWhiteSpace(pathToExe))
                {
                    continue;
                }

                pathToConsole = Directory.GetParent(pathToExe).ToString();
                Thread.Sleep(1000);
                proc.WaitForInputIdle();
                proc.Kill();
                if (!proc.HasExited)
                    _consoleLogger.Log(@"can't kill cati console process ");

                break;
            }

            return pathToConsole;
        }

        private void DeleteFolderWithLicenseAgreementFile(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}
