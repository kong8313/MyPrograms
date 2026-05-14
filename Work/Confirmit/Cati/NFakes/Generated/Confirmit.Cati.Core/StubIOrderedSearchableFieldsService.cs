using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIOrderedSearchableFieldsService : IOrderedSearchableFieldsService 
    {
        private IOrderedSearchableFieldsService _inner;

        public StubIOrderedSearchableFieldsService()
        {
            _inner = null;
        }

        public IOrderedSearchableFieldsService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void RegenerateFieldsInt32Delegate(int surveySid);
        public RegenerateFieldsInt32Delegate RegenerateFieldsInt32;

        void IOrderedSearchableFieldsService.RegenerateFields(int surveySid)
        {

            if (RegenerateFieldsInt32 != null)
            {
                RegenerateFieldsInt32(surveySid);
            } else if (_inner != null)
            {
                ((IOrderedSearchableFieldsService)_inner).RegenerateFields(surveySid);
            }
        }

    }
}