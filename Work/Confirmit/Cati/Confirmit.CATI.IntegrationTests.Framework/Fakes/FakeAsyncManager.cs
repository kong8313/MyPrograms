using System;
using Confirmit.CATI.Core.Misc;
using System.Threading.Tasks;

namespace Confirmit.CATI.IntegrationTests.Framework.Fakes
{
    public class FakeAsyncManager : IAsyncManager
    {
        public FakeAsyncManager()
        {
            Inner = null;
        }

        public IAsyncManager Inner { set; get; }

        public delegate Task CreateTaskActionDelegate(Action action);
        public CreateTaskActionDelegate CreateTaskAction;

        Task IAsyncManager.CreateTask(Action action)
        {
            if (CreateTaskAction != null)
            {
                return CreateTaskAction(action);
            }

            return (Inner != null) ? Inner.CreateTask(action) : default(Task);
        }

        Task<T> IAsyncManager.CreateTask<T>(Func<T> func)
        {
            var task = new Task<T>(func);

            return task;
        }

        public delegate void ScheduleTaskDelegate(TimeSpan delay, Task task);
        public ScheduleTaskDelegate OnScheduleTask;

        public void ScheduleTask(TimeSpan delay, Task task)
        {
            if (OnScheduleTask != null)
            {
                OnScheduleTask(delay, task);
            }
            else if (Inner != null)
            {
                Inner.ScheduleTask(delay, task);
            }

        }

        public delegate void StartTaskTaskDelegate(Task task);
        public StartTaskTaskDelegate StartTaskTask;

        void IAsyncManager.StartTask(Task task)
        {
            if (StartTaskTask != null)
            {
                StartTaskTask(task);
            }
            else if (Inner != null)
            {
                Inner.StartTask(task);
            }
        }

        void IAsyncManager.StartTask<T>(Task<T> task)
        {
            task.RunSynchronously();
        }

        public delegate void QueueWorkItemActionDelegate(Action action);
        public QueueWorkItemActionDelegate QueueWorkItemAction;

        void IAsyncManager.QueueWorkItem(Action action)
        {
            if (QueueWorkItemAction != null)
            {
                QueueWorkItemAction(action);
            }
            else if (Inner != null)
            {
                Inner.QueueWorkItem(action);
            }
        }

        public delegate void QueueWorkItemActionFuncOfStringDelegate(Action action, Func<string> contextStringSource);
        public QueueWorkItemActionFuncOfStringDelegate QueueWorkItemActionFuncOfString;

        void IAsyncManager.QueueWorkItem(Action action, Func<string> contextStringSource)
        {
            if (QueueWorkItemActionFuncOfString != null)
            {
                QueueWorkItemActionFuncOfString(action, contextStringSource);
            }
            else if (Inner != null)
            {
                Inner.QueueWorkItem(action, contextStringSource);
            }
        }

        public delegate void SleepInt32Delegate(int milliseconds);
        public SleepInt32Delegate SleepInt32;

        void IAsyncManager.Sleep(int milliseconds)
        {
            if (SleepInt32 != null)
            {
                SleepInt32(milliseconds);
            }
            else if (Inner != null)
            {
                Inner.Sleep(milliseconds);
            }
        }

        public void AwaitRunningTasks(TimeSpan timeout)
        {
        }
    }
}