namespace Confirmit.CATI.Core.Services
{
    public interface IShiftServiceFactory
    {
        IShiftService Get(int scheduleId);
        void DropScheduleCache();
    }
}