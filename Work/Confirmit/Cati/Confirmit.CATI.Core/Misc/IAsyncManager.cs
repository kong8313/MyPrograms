using System;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Misc
{
    public interface IAsyncManager
    {
        Task CreateTask(Action action);
        Task<T> CreateTask<T>(Func<T> func);
        void ScheduleTask(TimeSpan delay, Task task);
        void StartTask(Task task);
        void StartTask<T>(Task<T> task);
        void QueueWorkItem(Action action);
        void QueueWorkItem(Action action, Func<string> contextStringSource);
        void Sleep(int milliseconds);
        void AwaitRunningTasks(TimeSpan timeout);
    }
}