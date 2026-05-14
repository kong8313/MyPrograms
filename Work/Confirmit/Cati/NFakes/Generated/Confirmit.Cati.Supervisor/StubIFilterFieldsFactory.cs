using System;
using Confirmit.CATI.Supervisor.Classes.Filters;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Classes.Filters.Fakes
{
    public class StubIFilterFieldsFactory : IFilterFieldsFactory 
    {
        private IFilterFieldsFactory _inner;

        public StubIFilterFieldsFactory()
        {
            _inner = null;
        }

        public IFilterFieldsFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<BvFilterFieldsEntity> CreateStringDelegate(string fieldsXml);
        public CreateStringDelegate CreateString;

        IEnumerable<BvFilterFieldsEntity> IFilterFieldsFactory.Create(string fieldsXml)
        {


            if (CreateString != null)
            {
                return CreateString(fieldsXml);
            } else if (_inner != null)
            {
                return ((IFilterFieldsFactory)_inner).Create(fieldsXml);
            }

            return default(IEnumerable<BvFilterFieldsEntity>);
        }

    }
}