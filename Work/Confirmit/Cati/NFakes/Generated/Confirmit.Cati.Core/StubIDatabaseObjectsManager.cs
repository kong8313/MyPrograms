using System;
using Confirmit.CATI.Core.AsynchronousTrigger.Database;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Database.Fakes
{
    public class StubIDatabaseObjectsManager : IDatabaseObjectsManager 
    {
        private IDatabaseObjectsManager _inner;

        public StubIDatabaseObjectsManager()
        {
            _inner = null;
        }

        public IDatabaseObjectsManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void DropIfExistSbMessageTypeDelegate();
        public DropIfExistSbMessageTypeDelegate DropIfExistSbMessageType;

        void IDatabaseObjectsManager.DropIfExistSbMessageType()
        {

            if (DropIfExistSbMessageType != null)
            {
                DropIfExistSbMessageType();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).DropIfExistSbMessageType();
            }
        }

        public delegate void DropIfExistSbContractDelegate();
        public DropIfExistSbContractDelegate DropIfExistSbContract;

        void IDatabaseObjectsManager.DropIfExistSbContract()
        {

            if (DropIfExistSbContract != null)
            {
                DropIfExistSbContract();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).DropIfExistSbContract();
            }
        }

        public delegate void DropIfExistSbQueueDelegate();
        public DropIfExistSbQueueDelegate DropIfExistSbQueue;

        void IDatabaseObjectsManager.DropIfExistSbQueue()
        {

            if (DropIfExistSbQueue != null)
            {
                DropIfExistSbQueue();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).DropIfExistSbQueue();
            }
        }

        public delegate void DropIfExistSbServiceDelegate();
        public DropIfExistSbServiceDelegate DropIfExistSbService;

        void IDatabaseObjectsManager.DropIfExistSbService()
        {

            if (DropIfExistSbService != null)
            {
                DropIfExistSbService();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).DropIfExistSbService();
            }
        }

        public delegate void DropIfExistAsyncNotificationsTriggerDelegate();
        public DropIfExistAsyncNotificationsTriggerDelegate DropIfExistAsyncNotificationsTrigger;

        void IDatabaseObjectsManager.DropIfExistAsyncNotificationsTrigger()
        {

            if (DropIfExistAsyncNotificationsTrigger != null)
            {
                DropIfExistAsyncNotificationsTrigger();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).DropIfExistAsyncNotificationsTrigger();
            }
        }

        public delegate void DropIfExistTriggerStringDelegate(string tableName);
        public DropIfExistTriggerStringDelegate DropIfExistTriggerString;

        void IDatabaseObjectsManager.DropIfExistTrigger(string tableName)
        {

            if (DropIfExistTriggerString != null)
            {
                DropIfExistTriggerString(tableName);
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).DropIfExistTrigger(tableName);
            }
        }

        public delegate void CreateAsyncNotificationsTriggerDelegate();
        public CreateAsyncNotificationsTriggerDelegate CreateAsyncNotificationsTrigger;

        void IDatabaseObjectsManager.CreateAsyncNotificationsTrigger()
        {

            if (CreateAsyncNotificationsTrigger != null)
            {
                CreateAsyncNotificationsTrigger();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).CreateAsyncNotificationsTrigger();
            }
        }

        public delegate void CreateSbMessageTypeDelegate();
        public CreateSbMessageTypeDelegate CreateSbMessageType;

        void IDatabaseObjectsManager.CreateSbMessageType()
        {

            if (CreateSbMessageType != null)
            {
                CreateSbMessageType();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).CreateSbMessageType();
            }
        }

        public delegate void CreateSbContractDelegate();
        public CreateSbContractDelegate CreateSbContract;

        void IDatabaseObjectsManager.CreateSbContract()
        {

            if (CreateSbContract != null)
            {
                CreateSbContract();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).CreateSbContract();
            }
        }

        public delegate void CreateSbQueueDelegate();
        public CreateSbQueueDelegate CreateSbQueue;

        void IDatabaseObjectsManager.CreateSbQueue()
        {

            if (CreateSbQueue != null)
            {
                CreateSbQueue();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).CreateSbQueue();
            }
        }

        public delegate void CreateSbServiceDelegate();
        public CreateSbServiceDelegate CreateSbService;

        void IDatabaseObjectsManager.CreateSbService()
        {

            if (CreateSbService != null)
            {
                CreateSbService();
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).CreateSbService();
            }
        }

        public delegate void CreateTriggerStringDelegate(string tableName);
        public CreateTriggerStringDelegate CreateTriggerString;

        void IDatabaseObjectsManager.CreateTrigger(string tableName)
        {

            if (CreateTriggerString != null)
            {
                CreateTriggerString(tableName);
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).CreateTrigger(tableName);
            }
        }

        public delegate string GetReadMessageQueryStringInt32Delegate(string queueName, int timeout);
        public GetReadMessageQueryStringInt32Delegate GetReadMessageQueryStringInt32;

        string IDatabaseObjectsManager.GetReadMessageQuery(string queueName, int timeout)
        {


            if (GetReadMessageQueryStringInt32 != null)
            {
                return GetReadMessageQueryStringInt32(queueName, timeout);
            } else if (_inner != null)
            {
                return ((IDatabaseObjectsManager)_inner).GetReadMessageQuery(queueName, timeout);
            }

            return default(string);
        }

        public delegate void DropIfExistDatabaseObjectsIEnumerableOfStringDelegate(IEnumerable<string> tables);
        public DropIfExistDatabaseObjectsIEnumerableOfStringDelegate DropIfExistDatabaseObjectsIEnumerableOfString;

        void IDatabaseObjectsManager.DropIfExistDatabaseObjects(IEnumerable<string> tables)
        {

            if (DropIfExistDatabaseObjectsIEnumerableOfString != null)
            {
                DropIfExistDatabaseObjectsIEnumerableOfString(tables);
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).DropIfExistDatabaseObjects(tables);
            }
        }

        public delegate void CreateDatabaseObjectsIEnumerableOfStringDelegate(IEnumerable<string> tables);
        public CreateDatabaseObjectsIEnumerableOfStringDelegate CreateDatabaseObjectsIEnumerableOfString;

        void IDatabaseObjectsManager.CreateDatabaseObjects(IEnumerable<string> tables)
        {

            if (CreateDatabaseObjectsIEnumerableOfString != null)
            {
                CreateDatabaseObjectsIEnumerableOfString(tables);
            } else if (_inner != null)
            {
                ((IDatabaseObjectsManager)_inner).CreateDatabaseObjects(tables);
            }
        }

    }
}