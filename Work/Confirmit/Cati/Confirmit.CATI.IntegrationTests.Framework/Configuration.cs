using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Confirmit.CATI.IntegrationTests.Framework
{
    public class Configuration
    {
        private readonly System.Configuration.Configuration _config;

        public Configuration()
        {
            _config = ConfigurationManager.OpenExeConfiguration(
                Path.Combine(
                    TestPath,
                    "Confirmit.CATI.IntegrationTests.dll"));
        }

        public string TempPath
        {
            get
            {
                return @"c:\Temp\";
            }
        }

        public string TestPath
        {
            get
            {
                return Path.GetDirectoryName(
                    (new Uri(
                        Assembly.GetExecutingAssembly().CodeBase)).LocalPath).ToUpper();
            }
        }

        public string TestDataPath
        {
            get
            {
                return Path.Combine(
                    TestPath,
                    "TestsData");
            }
        }

        public string TestDBDataPath
        {
            get
            {
                string dbDataPath = Path.Combine(TempPath, "MDF");
                if (!Directory.Exists(dbDataPath))
                {
                    Directory.CreateDirectory(dbDataPath);
                }
                return dbDataPath;
            }
        }

        public string TestDBLogPath
        {
            get
            {
                string dbDataPath = Path.Combine(TempPath, "LDF");
                if (!Directory.Exists(dbDataPath))
                {
                    Directory.CreateDirectory(dbDataPath);
                }
                return dbDataPath;
            }
        }

        public string DbBaseScript
        {
            get
            {
                return _config.AppSettings.Settings["DbBaseScript"].Value;
            }
        }

        public string DbScript
        {
            get
            {
                return _config.AppSettings.Settings["DbScript"].Value;
            }
        }

        public string ConfirmitSurveyBackupDbName
        {
            get
            {
                return _config.AppSettings.Settings["ConfirmitSurveyBackupDbName"].Value;
            }
        }

        public string SqlUser
        {
            get
            {
                return _config.AppSettings.Settings["SqlUser"].Value;
            }
        }

        public string SqlPassword
        {
            get
            {
                return _config.AppSettings.Settings["SqlPassword"].Value;
            }
        }

        public string GetValue(string key)
        {
            return _config.AppSettings.Settings[key].Value;
        }

        public int DurationForMultiUserTests
        {
            get
            {
                string durationForMultiUserTests =
                    _config.AppSettings.Settings["DurationForMultiUserTests"].Value;
                return Int32.Parse(durationForMultiUserTests);
            }
        }

        public int MaxThreads
        {
            get
            {
                string maxThreads =
                    _config.AppSettings.Settings["MaxThreads"].Value;
                return Int32.Parse(maxThreads);
            }
        }

        public int MaxCompletionPorts
        {
            get
            {
                string maxCompletionPorts =
                    _config.AppSettings.Settings["MaxCompletionPorts"].Value;
                return Int32.Parse(maxCompletionPorts);
            }
        }
    }
}