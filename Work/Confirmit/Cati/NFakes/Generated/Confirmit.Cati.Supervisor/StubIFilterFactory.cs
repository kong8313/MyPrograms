using System;
using Confirmit.CATI.Supervisor.Classes.Filters;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Classes.Filters.Fakes
{
    public class StubIFilterFactory : IFilterFactory 
    {
        private IFilterFactory _inner;

        public StubIFilterFactory()
        {
            _inner = null;
        }

        public IFilterFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvFiltersEntity CreateInt32StringStringStringDelegate(int id, string name, string description, string operatorString);
        public CreateInt32StringStringStringDelegate CreateInt32StringStringString;

        BvFiltersEntity IFilterFactory.Create(int id, string name, string description, string operatorString)
        {


            if (CreateInt32StringStringString != null)
            {
                return CreateInt32StringStringString(id, name, description, operatorString);
            } else if (_inner != null)
            {
                return ((IFilterFactory)_inner).Create(id, name, description, operatorString);
            }

            return default(BvFiltersEntity);
        }

    }
}