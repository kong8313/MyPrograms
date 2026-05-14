using System.Diagnostics;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class StationInfoFactory : IStationInfoFactory
    {
        private readonly IDialerSettings _dialerSettings;
        private readonly ICallCenterRepository _callCenterRepository;
        private readonly IStationIdParser _stationIdParser;

        public StationInfoFactory(
            IDialerSettings dialerSettings,
            ICallCenterRepository callCenterRepository, 
            IStationIdParser stationIdParser)
        {
            _dialerSettings = dialerSettings;
            _callCenterRepository = callCenterRepository;
            _stationIdParser = stationIdParser;
        }

        public StationInfo Create(string stationId, BvPersonEntity person)
        {
            var stationInfo = _stationIdParser.Parse(stationId);

            if (_dialerSettings.IgnoreDialerIdFromStationId)
            {
                stationInfo.DialerId = 0;
                stationInfo.IsLocal = false;
            }

            var callCenter = _callCenterRepository.Get(person.CallCenterID);

            if (callCenter.DialerId == 0)
            {
                return stationInfo;
            }

            // Console has so called "station id" that defines dialer id and interviewer binding type (local or not)
            // The values from the "station id" may conflict with the call center dialer id assignment
            // The logic below rosolves such kind of conflicts

            if (stationInfo.IsLocal && (stationInfo.DialerId != callCenter.DialerId))
            {
                // It assumed interviewer phone (or headset) is hardwired to particular dialer if console is configured to use 'local' interviewer binding type.
                // In this case interviewer can login as 'local' to that dialer only.
                // So we force the IsLocal to 'false' as the DialerId is not the same as defined in the station id.
                // That means we'll instruct dialer to use "not local" binding when loggin-in the interviewer (i.e. dial a phone number to connect the interviewer) 

                Trace.TraceWarning(
                    "StationInfoFactory.Create: Call center dialer id assignment conflicts with Console station id. IsLocal will be forced to [false]. /// " +
                    "Person: [{0}({1})], Call center: [{2}({3})], callCenter.DialerId: [{4})], Console StationInfo: [{5}]",
                    person.Name, person.SID, callCenter.Name, callCenter.ID, callCenter.DialerId, stationInfo);

                stationInfo.IsLocal = false;
            }

            // Note, that we leave the (IsLocal = 'true') in case
            // if (_stationInfo.IsLocal && (_stationInfo.DialerId == callCenter.DialerId))

            stationInfo.DialerId = callCenter.DialerId;

            return stationInfo;
        }
    }
}