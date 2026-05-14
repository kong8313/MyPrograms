using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class FieldViewStateSerializer
    {
        private FieldInfo[] _storedInViewStateFields;
        private readonly Control _control;
        private readonly StateBag _viewState;

        public FieldViewStateSerializer(Control control, StateBag viewState)
        {
            _control = control;
            _viewState = viewState;
        }

        public void Load()
        {
            foreach (var fieldInfo in GetStoredInViewStateFields())
            {
                if (_viewState[fieldInfo.Name] != null)
                {
                    fieldInfo.SetValue(_control, _viewState[fieldInfo.Name]);
                }
            }
        }

        public void Save()
        {
            foreach (var fieldInfo in GetStoredInViewStateFields())
            {
                _viewState[fieldInfo.Name] = fieldInfo.GetValue(_control);
            }
        } 

        private IEnumerable<FieldInfo> GetStoredInViewStateFields()
        {
            if (_storedInViewStateFields == null)
            {
                _storedInViewStateFields = _control.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(
                    x => x.GetCustomAttributes(typeof(StoreInViewStateAttribute), false).Any()).ToArray();
            }

            return _storedInViewStateFields;
        }
    }
}