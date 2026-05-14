using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISQLServerSettings : ISQLServerSettings 
    {
        private ISQLServerSettings _inner;

        public StubISQLServerSettings()
        {
            _inner = null;
        }

        public ISQLServerSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _DefaultSqlCommandTimeout;
        public Func<int> DefaultSqlCommandTimeoutGet;
        public Action<int> DefaultSqlCommandTimeoutSetInt32;

        int ISQLServerSettings.DefaultSqlCommandTimeout
        {
            get
            {
                if (DefaultSqlCommandTimeoutGet != null)
                {
                    return DefaultSqlCommandTimeoutGet();
                } else if (_inner != null)
                {
                    return ((ISQLServerSettings)_inner).DefaultSqlCommandTimeout;
                }

                if (DefaultSqlCommandTimeoutSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DefaultSqlCommandTimeout;
                }

                return default(int);
            }

            set
            {
                if (DefaultSqlCommandTimeoutSetInt32 != null)
                {
                    DefaultSqlCommandTimeoutSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISQLServerSettings)_inner).DefaultSqlCommandTimeout = value;
                    return;
                }

                if (DefaultSqlCommandTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DefaultSqlCommandTimeout = value;
                }

            }
        }

        private int _DefaultSqlConnectionTimeout;
        public Func<int> DefaultSqlConnectionTimeoutGet;
        public Action<int> DefaultSqlConnectionTimeoutSetInt32;

        int ISQLServerSettings.DefaultSqlConnectionTimeout
        {
            get
            {
                if (DefaultSqlConnectionTimeoutGet != null)
                {
                    return DefaultSqlConnectionTimeoutGet();
                } else if (_inner != null)
                {
                    return ((ISQLServerSettings)_inner).DefaultSqlConnectionTimeout;
                }

                if (DefaultSqlConnectionTimeoutSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DefaultSqlConnectionTimeout;
                }

                return default(int);
            }

            set
            {
                if (DefaultSqlConnectionTimeoutSetInt32 != null)
                {
                    DefaultSqlConnectionTimeoutSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISQLServerSettings)_inner).DefaultSqlConnectionTimeout = value;
                    return;
                }

                if (DefaultSqlConnectionTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DefaultSqlConnectionTimeout = value;
                }

            }
        }

        private string _SqlServerDataPath;
        public Func<string> SqlServerDataPathGet;
        public Action<string> SqlServerDataPathSetString;

        string ISQLServerSettings.SqlServerDataPath
        {
            get
            {
                if (SqlServerDataPathGet != null)
                {
                    return SqlServerDataPathGet();
                } else if (_inner != null)
                {
                    return ((ISQLServerSettings)_inner).SqlServerDataPath;
                }

                if (SqlServerDataPathSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SqlServerDataPath;
                }

                return default(string);
            }

            set
            {
                if (SqlServerDataPathSetString != null)
                {
                    SqlServerDataPathSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISQLServerSettings)_inner).SqlServerDataPath = value;
                    return;
                }

                if (SqlServerDataPathGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SqlServerDataPath = value;
                }

            }
        }

        private string _SqlServerLogPath;
        public Func<string> SqlServerLogPathGet;
        public Action<string> SqlServerLogPathSetString;

        string ISQLServerSettings.SqlServerLogPath
        {
            get
            {
                if (SqlServerLogPathGet != null)
                {
                    return SqlServerLogPathGet();
                } else if (_inner != null)
                {
                    return ((ISQLServerSettings)_inner).SqlServerLogPath;
                }

                if (SqlServerLogPathSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SqlServerLogPath;
                }

                return default(string);
            }

            set
            {
                if (SqlServerLogPathSetString != null)
                {
                    SqlServerLogPathSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISQLServerSettings)_inner).SqlServerLogPath = value;
                    return;
                }

                if (SqlServerLogPathGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SqlServerLogPath = value;
                }

            }
        }

    }
}