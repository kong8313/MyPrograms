using System;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService.Fakes
{
    public class StubIConsoleVersionValidator : IConsoleVersionValidator 
    {
        private IConsoleVersionValidator _inner;

        public StubIConsoleVersionValidator()
        {
            _inner = null;
        }

        public IConsoleVersionValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ValidateVersionStringDelegate(string version);
        public ValidateVersionStringDelegate ValidateVersionString;

        void IConsoleVersionValidator.ValidateVersion(string version)
        {

            if (ValidateVersionString != null)
            {
                ValidateVersionString(version);
            } else if (_inner != null)
            {
                ((IConsoleVersionValidator)_inner).ValidateVersion(version);
            }
        }

        public delegate bool IsLatestVersionStringDelegate(string version);
        public IsLatestVersionStringDelegate IsLatestVersionString;

        bool IConsoleVersionValidator.IsLatestVersion(string version)
        {


            if (IsLatestVersionString != null)
            {
                return IsLatestVersionString(version);
            } else if (_inner != null)
            {
                return ((IConsoleVersionValidator)_inner).IsLatestVersion(version);
            }

            return default(bool);
        }

    }
}