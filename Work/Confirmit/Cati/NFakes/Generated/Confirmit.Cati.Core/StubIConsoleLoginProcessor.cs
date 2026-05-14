using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.Core.PersonLogin.Fakes
{
    public class StubIConsoleLoginProcessor : IConsoleLoginProcessor 
    {
        private IConsoleLoginProcessor _inner;

        public StubIConsoleLoginProcessor()
        {
            _inner = null;
        }

        public IConsoleLoginProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate PersonInfo GetPersonInfoBvPersonEntityBvTasksEntityBooleanDelegate(BvPersonEntity person, BvTasksEntity task, bool isAlreadyLoggedIn);
        public GetPersonInfoBvPersonEntityBvTasksEntityBooleanDelegate GetPersonInfoBvPersonEntityBvTasksEntityBoolean;

        PersonInfo IConsoleLoginProcessor.GetPersonInfo(BvPersonEntity person, BvTasksEntity task, bool isAlreadyLoggedIn)
        {


            if (GetPersonInfoBvPersonEntityBvTasksEntityBoolean != null)
            {
                return GetPersonInfoBvPersonEntityBvTasksEntityBoolean(person, task, isAlreadyLoggedIn);
            } else if (_inner != null)
            {
                return ((IConsoleLoginProcessor)_inner).GetPersonInfo(person, task, isAlreadyLoggedIn);
            }

            return default(PersonInfo);
        }

        public delegate DiallerInfo GetDialerInfoBvTasksEntityStationInfoBooleanDelegate(BvTasksEntity task, StationInfo stationInfo, bool isAlreadyLoggedIn);
        public GetDialerInfoBvTasksEntityStationInfoBooleanDelegate GetDialerInfoBvTasksEntityStationInfoBoolean;

        DiallerInfo IConsoleLoginProcessor.GetDialerInfo(BvTasksEntity task, StationInfo stationInfo, bool isAlreadyLoggedIn)
        {


            if (GetDialerInfoBvTasksEntityStationInfoBoolean != null)
            {
                return GetDialerInfoBvTasksEntityStationInfoBoolean(task, stationInfo, isAlreadyLoggedIn);
            } else if (_inner != null)
            {
                return ((IConsoleLoginProcessor)_inner).GetDialerInfo(task, stationInfo, isAlreadyLoggedIn);
            }

            return default(DiallerInfo);
        }

        public delegate CatiConsolePropertiesContainer GetConsolePropertiesInfoDelegate();
        public GetConsolePropertiesInfoDelegate GetConsolePropertiesInfo;

        CatiConsolePropertiesContainer IConsoleLoginProcessor.GetConsolePropertiesInfo()
        {


            if (GetConsolePropertiesInfo != null)
            {
                return GetConsolePropertiesInfo();
            } else if (_inner != null)
            {
                return ((IConsoleLoginProcessor)_inner).GetConsolePropertiesInfo();
            }

            return default(CatiConsolePropertiesContainer);
        }

        public delegate BvTasksEntity LoginBvPersonEntityBvTasksEntityStationInfoBooleanOutDelegate(BvPersonEntity person, BvTasksEntity task, StationInfo stationInfo, out bool isAlreadyLoggedIn);
        public LoginBvPersonEntityBvTasksEntityStationInfoBooleanOutDelegate LoginBvPersonEntityBvTasksEntityStationInfoBooleanOut;

        BvTasksEntity IConsoleLoginProcessor.Login(BvPersonEntity person, BvTasksEntity task, StationInfo stationInfo, out bool isAlreadyLoggedIn)
        {
            isAlreadyLoggedIn = default(bool);


            if (LoginBvPersonEntityBvTasksEntityStationInfoBooleanOut != null)
            {
                return LoginBvPersonEntityBvTasksEntityStationInfoBooleanOut(person, task, stationInfo, out isAlreadyLoggedIn);
            } else if (_inner != null)
            {
                return ((IConsoleLoginProcessor)_inner).Login(person, task, stationInfo, out isAlreadyLoggedIn);
            }

            return default(BvTasksEntity);
        }

    }
}