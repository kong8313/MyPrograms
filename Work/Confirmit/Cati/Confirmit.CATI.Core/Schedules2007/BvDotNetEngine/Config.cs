using System;
using System.IO;
using System.Diagnostics;

using Confirmit.CATI.Core.Misc;

namespace BvDotNetEngine
{
    /// <summary>
    /// TODO: Merge into single settings class
    /// </summary>
    public class Config
    {
        static Config _inst = null;

        private string _fusionPath = null;
        public string FusionPath { get { return _fusionPath; } }

        private string _dotNetFrameworkPath = null;
        public string DotNetFrameworkPath { get { return _dotNetFrameworkPath; } }

        private string _baseScriptPath = null;
        public string BaseScriptPath { get { return _baseScriptPath; } }

        private string _baseScriptDirectory = "Scripts";

        public string BaseScriptDirectory{ get { return _baseScriptDirectory; } }

        public string ConnectionString { get { return BackendInstance.Current.ConnectionString; } }


        private Config()
        {
            if (!BackendInstance.Current.IsDefaultInstance)
                _baseScriptDirectory += @"\" + BackendInstance.Current.CompanyId;
            else
                _baseScriptDirectory += @"\Default";

            _fusionPath = Path.GetDirectoryName(typeof(Config).Assembly.Location);

            _dotNetFrameworkPath = Path.GetDirectoryName(typeof(Int32).Assembly.Location);
            
            _baseScriptPath = Path.Combine(_fusionPath, _baseScriptDirectory );

            // clear old scripts
            if (Directory.Exists(_baseScriptPath))
            {
                //try delete directory
                try
                {
                    Directory.Delete(_baseScriptPath, true);
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    //we need not in System.IO.DirectoryNotFoundException
                    //if directory not exists we needn't exception.
                }
                catch (System.Exception e)
                {
                    Trace.TraceError(e.ToString());
                }
            }
            Directory.CreateDirectory(_baseScriptPath);
        }

        public static Config Inst
        {
            get
            {
                if (_inst != null)
                    return _inst;

                lock ( typeof(Config) )
                {
                    if (_inst != null)
                        return _inst;

                    _inst = new Config();
                    
                    return _inst;
                }
            }            
        }
    }
}
