using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubISurveySqlServerNameProvider : ISurveySqlServerNameProvider 
    {
        private ISurveySqlServerNameProvider _inner;

        public StubISurveySqlServerNameProvider()
        {
            _inner = null;
        }

        public ISurveySqlServerNameProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetSurveySqlServerNameStringDelegate(string projectId);
        public GetSurveySqlServerNameStringDelegate GetSurveySqlServerNameString;

        string ISurveySqlServerNameProvider.GetSurveySqlServerName(string projectId)
        {


            if (GetSurveySqlServerNameString != null)
            {
                return GetSurveySqlServerNameString(projectId);
            } else if (_inner != null)
            {
                return ((ISurveySqlServerNameProvider)_inner).GetSurveySqlServerName(projectId);
            }

            return default(string);
        }

    }
}