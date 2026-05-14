using System;
using Confirmit.CATI.Core.AsynchronousTrigger.Database;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Database.Fakes
{
    public class StubIDatabaseObjectsNameManager : IDatabaseObjectsNameManager 
    {
        private IDatabaseObjectsNameManager _inner;

        public StubIDatabaseObjectsNameManager()
        {
            _inner = null;
        }

        public IDatabaseObjectsNameManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string CreateMessageTypeNameDelegate();
        public CreateMessageTypeNameDelegate CreateMessageTypeName;

        string IDatabaseObjectsNameManager.CreateMessageTypeName()
        {


            if (CreateMessageTypeName != null)
            {
                return CreateMessageTypeName();
            } else if (_inner != null)
            {
                return ((IDatabaseObjectsNameManager)_inner).CreateMessageTypeName();
            }

            return default(string);
        }

        public delegate string CreateContractNameDelegate();
        public CreateContractNameDelegate CreateContractName;

        string IDatabaseObjectsNameManager.CreateContractName()
        {


            if (CreateContractName != null)
            {
                return CreateContractName();
            } else if (_inner != null)
            {
                return ((IDatabaseObjectsNameManager)_inner).CreateContractName();
            }

            return default(string);
        }

        public delegate string CreateSbQueueNameDelegate();
        public CreateSbQueueNameDelegate CreateSbQueueName;

        string IDatabaseObjectsNameManager.CreateSbQueueName()
        {


            if (CreateSbQueueName != null)
            {
                return CreateSbQueueName();
            } else if (_inner != null)
            {
                return ((IDatabaseObjectsNameManager)_inner).CreateSbQueueName();
            }

            return default(string);
        }

        public delegate string CreateSbServiceNameDelegate();
        public CreateSbServiceNameDelegate CreateSbServiceName;

        string IDatabaseObjectsNameManager.CreateSbServiceName()
        {


            if (CreateSbServiceName != null)
            {
                return CreateSbServiceName();
            } else if (_inner != null)
            {
                return ((IDatabaseObjectsNameManager)_inner).CreateSbServiceName();
            }

            return default(string);
        }

        public delegate string CreateTriggerNameStringDelegate(string tableName);
        public CreateTriggerNameStringDelegate CreateTriggerNameString;

        string IDatabaseObjectsNameManager.CreateTriggerName(string tableName)
        {


            if (CreateTriggerNameString != null)
            {
                return CreateTriggerNameString(tableName);
            } else if (_inner != null)
            {
                return ((IDatabaseObjectsNameManager)_inner).CreateTriggerName(tableName);
            }

            return default(string);
        }

        public delegate string CreateTriggerPostfixDelegate();
        public CreateTriggerPostfixDelegate CreateTriggerPostfix;

        string IDatabaseObjectsNameManager.CreateTriggerPostfix()
        {


            if (CreateTriggerPostfix != null)
            {
                return CreateTriggerPostfix();
            } else if (_inner != null)
            {
                return ((IDatabaseObjectsNameManager)_inner).CreateTriggerPostfix();
            }

            return default(string);
        }

    }
}