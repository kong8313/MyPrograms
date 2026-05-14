using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIProcessAndEnvironmentInfo : IProcessAndEnvironmentInfo 
    {
        private IProcessAndEnvironmentInfo _inner;

        public StubIProcessAndEnvironmentInfo()
        {
            _inner = null;
        }

        public IProcessAndEnvironmentInfo Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _ProcessName;
        public Func<string> ProcessNameGet;
        public Action<string> ProcessNameSetString;

        string IProcessAndEnvironmentInfo.ProcessName
        {
            get
            {
                if (ProcessNameGet != null)
                {
                    return ProcessNameGet();
                } else if (_inner != null)
                {
                    return ((IProcessAndEnvironmentInfo)_inner).ProcessName;
                }

                if (ProcessNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ProcessName;
                }

                return default(string);
            }

        }

        private int _ProcessId;
        public Func<int> ProcessIdGet;
        public Action<int> ProcessIdSetInt32;

        int IProcessAndEnvironmentInfo.ProcessId
        {
            get
            {
                if (ProcessIdGet != null)
                {
                    return ProcessIdGet();
                } else if (_inner != null)
                {
                    return ((IProcessAndEnvironmentInfo)_inner).ProcessId;
                }

                if (ProcessIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ProcessId;
                }

                return default(int);
            }

        }

        private string _MachineName;
        public Func<string> MachineNameGet;
        public Action<string> MachineNameSetString;

        string IProcessAndEnvironmentInfo.MachineName
        {
            get
            {
                if (MachineNameGet != null)
                {
                    return MachineNameGet();
                } else if (_inner != null)
                {
                    return ((IProcessAndEnvironmentInfo)_inner).MachineName;
                }

                if (MachineNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MachineName;
                }

                return default(string);
            }

        }

        private string _Version;
        public Func<string> VersionGet;
        public Action<string> VersionSetString;

        string IProcessAndEnvironmentInfo.Version
        {
            get
            {
                if (VersionGet != null)
                {
                    return VersionGet();
                } else if (_inner != null)
                {
                    return ((IProcessAndEnvironmentInfo)_inner).Version;
                }

                if (VersionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Version;
                }

                return default(string);
            }

        }

        private string _Changeset;
        public Func<string> ChangesetGet;
        public Action<string> ChangesetSetString;

        string IProcessAndEnvironmentInfo.Changeset
        {
            get
            {
                if (ChangesetGet != null)
                {
                    return ChangesetGet();
                } else if (_inner != null)
                {
                    return ((IProcessAndEnvironmentInfo)_inner).Changeset;
                }

                if (ChangesetSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Changeset;
                }

                return default(string);
            }

        }

    }
}