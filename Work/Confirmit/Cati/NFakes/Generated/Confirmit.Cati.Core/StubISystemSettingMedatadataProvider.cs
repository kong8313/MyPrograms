using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISystemSettingMedatadataProvider : ISystemSettingMedatadataProvider 
    {
        private ISystemSettingMedatadataProvider _inner;

        public StubISystemSettingMedatadataProvider()
        {
            _inner = null;
        }

        public ISystemSettingMedatadataProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate SystemSettingMedatadata[] GetAllDelegate();
        public GetAllDelegate GetAll;

        SystemSettingMedatadata[] ISystemSettingMedatadataProvider.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((ISystemSettingMedatadataProvider)_inner).GetAll();
            }

            return default(SystemSettingMedatadata[]);
        }

    }
}