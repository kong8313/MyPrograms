namespace Confirmit.CATI.Core.DAL.Handmade.Cache
{
    public interface ISystemSettingCache
    {
        string Get(string settingSystemName);        

        void Set<T>(string settingSystemName, T value);

        void Reset();
    }
}
