using System;
using Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns.Fakes
{
    public class StubICustomizableColumnsService : ICustomizableColumnsService 
    {
        private ICustomizableColumnsService _inner;

        public StubICustomizableColumnsService()
        {
            _inner = null;
        }

        public ICustomizableColumnsService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BoundField> GetGridFieldsDelegate();
        public GetGridFieldsDelegate GetGridFields;

        List<BoundField> ICustomizableColumnsService.GetGridFields()
        {


            if (GetGridFields != null)
            {
                return GetGridFields();
            } else if (_inner != null)
            {
                return ((ICustomizableColumnsService)_inner).GetGridFields();
            }

            return default(List<BoundField>);
        }

        public delegate List<GridColumnSetting> GetColumnSettingsDelegate();
        public GetColumnSettingsDelegate GetColumnSettings;

        List<GridColumnSetting> ICustomizableColumnsService.GetColumnSettings()
        {


            if (GetColumnSettings != null)
            {
                return GetColumnSettings();
            } else if (_inner != null)
            {
                return ((ICustomizableColumnsService)_inner).GetColumnSettings();
            }

            return default(List<GridColumnSetting>);
        }

        public delegate void SaveColumnSettingsListOfGridColumnSettingDelegate(List<GridColumnSetting> settings);
        public SaveColumnSettingsListOfGridColumnSettingDelegate SaveColumnSettingsListOfGridColumnSetting;

        void ICustomizableColumnsService.SaveColumnSettings(List<GridColumnSetting> settings)
        {

            if (SaveColumnSettingsListOfGridColumnSetting != null)
            {
                SaveColumnSettingsListOfGridColumnSetting(settings);
            } else if (_inner != null)
            {
                ((ICustomizableColumnsService)_inner).SaveColumnSettings(settings);
            }
        }

        public delegate Object GetGridDataArrayOfObjectDelegate(Object[] searchParams);
        public GetGridDataArrayOfObjectDelegate GetGridDataArrayOfObject;

        Object ICustomizableColumnsService.GetGridData(Object[] searchParams)
        {


            if (GetGridDataArrayOfObject != null)
            {
                return GetGridDataArrayOfObject(searchParams);
            } else if (_inner != null)
            {
                return ((ICustomizableColumnsService)_inner).GetGridData(searchParams);
            }

            return default(Object);
        }

    }
}