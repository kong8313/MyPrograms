using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Core.BlackList;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.BlackList.Fakes
{
    public class StubIBlackListService : IBlackListService 
    {
        private IBlackListService _inner;

        public StubIBlackListService()
        {
            _inner = null;
        }

        public IBlackListService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddNumberBvTelephoneBlacklistEntityDelegate(BvTelephoneBlacklistEntity entity);
        public AddNumberBvTelephoneBlacklistEntityDelegate AddNumberBvTelephoneBlacklistEntity;

        void IBlackListService.AddNumber(BvTelephoneBlacklistEntity entity)
        {

            if (AddNumberBvTelephoneBlacklistEntity != null)
            {
                AddNumberBvTelephoneBlacklistEntity(entity);
            } else if (_inner != null)
            {
                ((IBlackListService)_inner).AddNumber(entity);
            }
        }

        public delegate void UpdateNumberStringBvTelephoneBlacklistEntityDelegate(string oldNumber, BvTelephoneBlacklistEntity entity);
        public UpdateNumberStringBvTelephoneBlacklistEntityDelegate UpdateNumberStringBvTelephoneBlacklistEntity;

        void IBlackListService.UpdateNumber(string oldNumber, BvTelephoneBlacklistEntity entity)
        {

            if (UpdateNumberStringBvTelephoneBlacklistEntity != null)
            {
                UpdateNumberStringBvTelephoneBlacklistEntity(oldNumber, entity);
            } else if (_inner != null)
            {
                ((IBlackListService)_inner).UpdateNumber(oldNumber, entity);
            }
        }

        public delegate void ImportNumbersIEnumerableOfStringDelegate(IEnumerable<string> numbers);
        public ImportNumbersIEnumerableOfStringDelegate ImportNumbersIEnumerableOfString;

        void IBlackListService.ImportNumbers(IEnumerable<string> numbers)
        {

            if (ImportNumbersIEnumerableOfString != null)
            {
                ImportNumbersIEnumerableOfString(numbers);
            } else if (_inner != null)
            {
                ((IBlackListService)_inner).ImportNumbers(numbers);
            }
        }

    }
}