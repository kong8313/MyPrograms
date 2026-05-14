using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubICommandLineParser : ICommandLineParser 
    {
        private ICommandLineParser _inner;

        public StubICommandLineParser()
        {
            _inner = null;
        }

        public ICommandLineParser Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int GetCompanyIdArrayOfStringDelegate(string[] commandLineArgs);
        public GetCompanyIdArrayOfStringDelegate GetCompanyIdArrayOfString;

        int ICommandLineParser.GetCompanyId(string[] commandLineArgs)
        {


            if (GetCompanyIdArrayOfString != null)
            {
                return GetCompanyIdArrayOfString(commandLineArgs);
            } else if (_inner != null)
            {
                return ((ICommandLineParser)_inner).GetCompanyId(commandLineArgs);
            }

            return default(int);
        }

    }
}