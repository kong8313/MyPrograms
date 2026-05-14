using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Core.PersonLogin;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIStationInfoFactory : IStationInfoFactory 
    {
        private IStationInfoFactory _inner;

        public StubIStationInfoFactory()
        {
            _inner = null;
        }

        public IStationInfoFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate StationInfo CreateStringBvPersonEntityDelegate(string stationId, BvPersonEntity person);
        public CreateStringBvPersonEntityDelegate CreateStringBvPersonEntity;

        StationInfo IStationInfoFactory.Create(string stationId, BvPersonEntity person)
        {


            if (CreateStringBvPersonEntity != null)
            {
                return CreateStringBvPersonEntity(stationId, person);
            } else if (_inner != null)
            {
                return ((IStationInfoFactory)_inner).Create(stationId, person);
            }

            return default(StationInfo);
        }

    }
}