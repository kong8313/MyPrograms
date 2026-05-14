using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;






namespace OlympicInterviewerConsole
{ 
    
    /// <summary>
    /// Summary description for OlympicConsoleAutopilotTest
    /// </summary>
    [TestClass]
    public class OlympicConsoleAutopilotTest
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Console.WriteLine("Console Test Initialize!!!");
            
        }

        [TestMethod]
        [DeploymentItem("OlympicAutopilot.txt")]
        [DeploymentItem("OlympicData.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", @"|DataDirectory|\OlympicData.xml", "work", DataAccessMethod.Sequential)]
        public void InterviewAutopilotPassingTest()
        {
            Console.WriteLine("Console Test method!!!");
            
            string userName = TestContext.DataRow["interviewerName"].ToString();
            string password = TestContext.DataRow["interviewerPassword"].ToString();
            string minDelay = TestContext.DataRow["AutoPilotMinDelay"].ToString();
            string maxDelay = TestContext.DataRow["AutoPilotMaxDelay"].ToString();
            string autoPilotAnswersFileName = TestContext.DataRow["AutoPilotAnswersFileName"].ToString();
            string timesToRepeat = TestContext.DataRow["AutoPilotTimesToRepeat"].ToString();
            string extensionNumber = TestContext.DataRow["AutoPilotExtensionNumber"].ToString();
            string company = TestContext.DataRow["company"].ToString();
            string companyId = TestContext.DataRow["companyId"].ToString();
            string locale = TestContext.DataRow["AutoPilotLocale"].ToString();
            string pathToConsole = Environment.GetEnvironmentVariable("CatiConsoleLocation");
            string catiSqlConnectionString = Environment.GetEnvironmentVariable("CatiSQLConnectionString");

            string stationId = GetStationIdForFirstActiveDialer(companyId, catiSqlConnectionString);

            var startInfo = new ProcessStartInfo(pathToConsole)
            {
                Arguments = String.Format(
                    @"UserName={0} Password={1} MinDelay={2} MaxDelay={3} FileName=""{4}"" TimesToRepeat={5} ExtensionNumber={6} StationId={7} Company={8} Locale={9}",
                    userName, password, minDelay, maxDelay, autoPilotAnswersFileName,
                    timesToRepeat, extensionNumber, stationId, company,
                    locale),
                UseShellExecute = false
            };

            Console.WriteLine(startInfo.Arguments);

            int exitCode = 255;

            var pathWithEnv = @"%USERPROFILE%\AppData\Local\Confirmit\CATI\License Agreement.txt";
            var catiProfilePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            bool needAccept = !File.Exists(catiProfilePath);
            
            using (Process consoleProcess = Process.Start(startInfo))
            {
                if (consoleProcess != null && 
                    (FindApplicationWindow("CATI Interviewer Console [AutoPilot]", needAccept)))
                {
                    consoleProcess.WaitForExit();
                    exitCode = consoleProcess.ExitCode;
                }              
            }
            Assert.AreEqual(exitCode, 0L);

        }

        private bool FindApplicationWindow(string displayName, bool needAccept)
        {
            //Max number of times to look for the window,
            //used to let you out if there's a problem.
            AutomationElement appWindow = null;
            int i = 25;
            while (appWindow == null && i > 0)
            {
                appWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, displayName));
                Thread.Sleep(500);
                i--;
            }

            if (appWindow == null)
                return false;
            
            // find "I accept..." and click it
            if (needAccept)
            {
                AutomationElement iAcceptButton = FindButton(appWindow, "I accept...");
                if (iAcceptButton != null)
                    DoButtonClick(iAcceptButton);
            }

            return true;
        }

        private static AutomationElement FindButton(AutomationElement appWindow, string buttonText)
        {
            //max number of times to look for the button,
            //lets you out if there's a problem
            int i = 25;

            AutomationElement okButton = null;

            while (i > 0)
            {
                okButton = appWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, buttonText));
                if (okButton != null)
                {
                    break;
                }
                Thread.Sleep(500);
                i--;
            }

            return okButton;
        }

        private static void DoButtonClick(AutomationElement uiButton)
        {
            Thread.Sleep(500);
            GetInvokePattern(uiButton).Invoke();
        }

        private static InvokePattern GetInvokePattern(AutomationElement element)
        {
            return element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
        }

        private static string GetStationIdForFirstActiveDialer(string companyId, string catiSqlConnectionString)
        {

            using (var connection = new SqlConnection(catiSqlConnectionString))
            {
                connection.Open();
                var command =
                    new SqlCommand(
                        "SELECT [Id],[DialerOperationalStateNotification]  FROM  [ConfirmitCATIV15_" +
                        companyId.Trim() + "].[dbo].[BvDialers]", connection);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        throw new Exception("There are no dialers for company with id: " + companyId);

                    reader.Read();
                    int dialerId = (int)reader["Id"];
                    if (dialerId <= 0)
                        throw new Exception("There is incorrect dialer with id " + dialerId+ "for company with id: " + companyId);

                    return "vm" + (dialerId-1).ToString().PadRight(6,'0')  + "L";
                }
            }
            
        }
    }

}