using System;
using Confirmit.CATI.Supervisor.Classes.Filters;

namespace Confirmit.CATI.Supervisor.Classes.Filters.Fakes
{
    public class StubISupervisorFilterFactory : ISupervisorFilterFactory 
    {
        private ISupervisorFilterFactory _inner;

        public StubISupervisorFilterFactory()
        {
            _inner = null;
        }

        public ISupervisorFilterFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate FilterData CreateInt32StringStringStringStringDelegate(int id, string name, string description, string operatorString, string fieldsXml);
        public CreateInt32StringStringStringStringDelegate CreateInt32StringStringStringString;

        FilterData ISupervisorFilterFactory.Create(int id, string name, string description, string operatorString, string fieldsXml)
        {


            if (CreateInt32StringStringStringString != null)
            {
                return CreateInt32StringStringStringString(id, name, description, operatorString, fieldsXml);
            } else if (_inner != null)
            {
                return ((ISupervisorFilterFactory)_inner).Create(id, name, description, operatorString, fieldsXml);
            }

            return default(FilterData);
        }

    }
}