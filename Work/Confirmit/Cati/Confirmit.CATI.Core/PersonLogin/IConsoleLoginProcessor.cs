using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.PersonLogin
{
    public interface IConsoleLoginProcessor
    {
        PersonInfo GetPersonInfo(BvPersonEntity person, BvTasksEntity task, bool isAlreadyLoggedIn);
        DiallerInfo GetDialerInfo(BvTasksEntity task, StationInfo stationInfo, bool isAlreadyLoggedIn);
        CatiConsolePropertiesContainer GetConsolePropertiesInfo();
        BvTasksEntity Login(BvPersonEntity person, BvTasksEntity task, StationInfo stationInfo, out bool isAlreadyLoggedIn);
    }
}