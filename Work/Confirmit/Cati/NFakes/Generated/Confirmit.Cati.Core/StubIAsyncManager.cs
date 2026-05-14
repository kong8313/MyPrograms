using System;
using Confirmit.CATI.Core.Misc;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIAsyncManager : IAsyncManager 
    {
        private IAsyncManager _inner;

        public StubIAsyncManager()
        {
            _inner = null;
        }

        public IAsyncManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task CreateTaskActionDelegate(Action action);
        public CreateTaskActionDelegate CreateTaskAction;

        Task IAsyncManager.CreateTask(Action action)
        {


            if (CreateTaskAction != null)
            {
                return CreateTaskAction(action);
            } else if (_inner != null)
            {
                return ((IAsyncManager)_inner).CreateTask(action);
            }

            return default(Task);
        }

        Task<T> IAsyncManager.CreateTask<T>(Func<T> func)
        {


            return default(Task<T>);
        }

        public delegate void ScheduleTaskTimeSpanTaskDelegate(TimeSpan delay, Task task);
        public ScheduleTaskTimeSpanTaskDelegate ScheduleTaskTimeSpanTask;

        void IAsyncManager.ScheduleTask(TimeSpan delay, Task task)
        {

            if (ScheduleTaskTimeSpanTask != null)
            {
                ScheduleTaskTimeSpanTask(delay, task);
            } else if (_inner != null)
            {
                ((IAsyncManager)_inner).ScheduleTask(delay, task);
            }
        }

        public delegate void StartTaskTaskDelegate(Task task);
        public StartTaskTaskDelegate StartTaskTask;

        void IAsyncManager.StartTask(Task task)
        {

            if (StartTaskTask != null)
            {
                StartTaskTask(task);
            } else if (_inner != null)
            {
                ((IAsyncManager)_inner).StartTask(task);
            }
        }

        void IAsyncManager.StartTask<T>(Task<T> task)
        {

        }

        public delegate void QueueWorkItemActionDelegate(Action action);
        public QueueWorkItemActionDelegate QueueWorkItemAction;

        void IAsyncManager.QueueWorkItem(Action action)
        {

            if (QueueWorkItemAction != null)
            {
                QueueWorkItemAction(action);
            } else if (_inner != null)
            {
                ((IAsyncManager)_inner).QueueWorkItem(action);
            }
        }

        public delegate void QueueWorkItemActionFuncOfStringDelegate(Action action, Func<string> contextStringSource);
        public QueueWorkItemActionFuncOfStringDelegate QueueWorkItemActionFuncOfString;

        void IAsyncManager.QueueWorkItem(Action action, Func<string> contextStringSource)
        {

            if (QueueWorkItemActionFuncOfString != null)
            {
                QueueWorkItemActionFuncOfString(action, contextStringSource);
            } else if (_inner != null)
            {
                ((IAsyncManager)_inner).QueueWorkItem(action, contextStringSource);
            }
        }

        public delegate void SleepInt32Delegate(int milliseconds);
        public SleepInt32Delegate SleepInt32;

        void IAsyncManager.Sleep(int milliseconds)
        {

            if (SleepInt32 != null)
            {
                SleepInt32(milliseconds);
            } else if (_inner != null)
            {
                ((IAsyncManager)_inner).Sleep(milliseconds);
            }
        }

        public delegate void AwaitRunningTasksTimeSpanDelegate(TimeSpan timeout);
        public AwaitRunningTasksTimeSpanDelegate AwaitRunningTasksTimeSpan;

        void IAsyncManager.AwaitRunningTasks(TimeSpan timeout)
        {

            if (AwaitRunningTasksTimeSpan != null)
            {
                AwaitRunningTasksTimeSpan(timeout);
            } else if (_inner != null)
            {
                ((IAsyncManager)_inner).AwaitRunningTasks(timeout);
            }
        }

    }
}