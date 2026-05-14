using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIConfiguration : IConfiguration 
    {
        private IConfiguration _inner;

        public StubIConfiguration()
        {
            _inner = null;
        }

        public IConfiguration Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _SqlServerName;
        public Func<string> SqlServerNameGet;
        public Action<string> SqlServerNameSetString;

        string IConfiguration.SqlServerName
        {
            get
            {
                if (SqlServerNameGet != null)
                {
                    return SqlServerNameGet();
                } else if (_inner != null)
                {
                    return ((IConfiguration)_inner).SqlServerName;
                }

                if (SqlServerNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SqlServerName;
                }

                return default(string);
            }

        }

        private string _SqlUserName;
        public Func<string> SqlUserNameGet;
        public Action<string> SqlUserNameSetString;

        string IConfiguration.SqlUserName
        {
            get
            {
                if (SqlUserNameGet != null)
                {
                    return SqlUserNameGet();
                } else if (_inner != null)
                {
                    return ((IConfiguration)_inner).SqlUserName;
                }

                if (SqlUserNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SqlUserName;
                }

                return default(string);
            }

        }

        private string _SqlPassword;
        public Func<string> SqlPasswordGet;
        public Action<string> SqlPasswordSetString;

        string IConfiguration.SqlPassword
        {
            get
            {
                if (SqlPasswordGet != null)
                {
                    return SqlPasswordGet();
                } else if (_inner != null)
                {
                    return ((IConfiguration)_inner).SqlPassword;
                }

                if (SqlPasswordSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SqlPassword;
                }

                return default(string);
            }

        }

        private string _DefaultDatabaseName;
        public Func<string> DefaultDatabaseNameGet;
        public Action<string> DefaultDatabaseNameSetString;

        string IConfiguration.DefaultDatabaseName
        {
            get
            {
                if (DefaultDatabaseNameGet != null)
                {
                    return DefaultDatabaseNameGet();
                } else if (_inner != null)
                {
                    return ((IConfiguration)_inner).DefaultDatabaseName;
                }

                if (DefaultDatabaseNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DefaultDatabaseName;
                }

                return default(string);
            }

        }

        private string _DatabaseNamePattern;
        public Func<string> DatabaseNamePatternGet;
        public Action<string> DatabaseNamePatternSetString;

        string IConfiguration.DatabaseNamePattern
        {
            get
            {
                if (DatabaseNamePatternGet != null)
                {
                    return DatabaseNamePatternGet();
                } else if (_inner != null)
                {
                    return ((IConfiguration)_inner).DatabaseNamePattern;
                }

                if (DatabaseNamePatternSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DatabaseNamePattern;
                }

                return default(string);
            }

        }

        private string _ConfirmlogConnectionString;
        public Func<string> ConfirmlogConnectionStringGet;
        public Action<string> ConfirmlogConnectionStringSetString;

        string IConfiguration.ConfirmlogConnectionString
        {
            get
            {
                if (ConfirmlogConnectionStringGet != null)
                {
                    return ConfirmlogConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((IConfiguration)_inner).ConfirmlogConnectionString;
                }

                if (ConfirmlogConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ConfirmlogConnectionString;
                }

                return default(string);
            }

        }

        private Version _ProductVersion;
        public Func<Version> ProductVersionGet;
        public Action<Version> ProductVersionSetVersion;

        Version IConfiguration.ProductVersion
        {
            get
            {
                if (ProductVersionGet != null)
                {
                    return ProductVersionGet();
                } else if (_inner != null)
                {
                    return ((IConfiguration)_inner).ProductVersion;
                }

                if (ProductVersionSetVersion == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ProductVersion;
                }

                return default(Version);
            }

        }

        private bool _IsDBCreation;
        public Func<bool> IsDBCreationGet;
        public Action<bool> IsDBCreationSetBoolean;

        bool IConfiguration.IsDBCreation
        {
            get
            {
                if (IsDBCreationGet != null)
                {
                    return IsDBCreationGet();
                } else if (_inner != null)
                {
                    return ((IConfiguration)_inner).IsDBCreation;
                }

                if (IsDBCreationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsDBCreation;
                }

                return default(bool);
            }

        }

    }
}