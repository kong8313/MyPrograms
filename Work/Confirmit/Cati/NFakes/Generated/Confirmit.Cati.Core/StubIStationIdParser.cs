using System;
using Confirmit.CATI.Core.PersonLogin;

namespace Confirmit.CATI.Core.PersonLogin.Fakes
{
    public class StubIStationIdParser : IStationIdParser 
    {
        private IStationIdParser _inner;

        public StubIStationIdParser()
        {
            _inner = null;
        }

        public IStationIdParser Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate StationInfo ParseStringDelegate(string stationId);
        public ParseStringDelegate ParseString;

        StationInfo IStationIdParser.Parse(string stationId)
        {


            if (ParseString != null)
            {
                return ParseString(stationId);
            } else if (_inner != null)
            {
                return ((IStationIdParser)_inner).Parse(stationId);
            }

            return default(StationInfo);
        }

    }
}