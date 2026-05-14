using System.Threading;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Logger;

namespace Confirmit.CATI.Core.Misc
{
    /// <summary>
    /// Enables SAFE (handles exception in the async code) the asynchronous processing of methods.
    /// </summary>
    public class AsyncManager : IAsyncManager
    {
        private readonly ConcurrentDictionary<Task, byte> _runningTasks = new ConcurrentDictionary<Task, byte>();

        public Task CreateTask(Action action)
        {
            // TODO: Should we wrap action in try catch?
            var task = new Task(action);

            return task;
        }

        public Task<T> CreateTask<T>(Func<T> func)
        {
            // TODO: Should we wrap func in try catch?
            var task = new Task<T>(func);

            return task;
        }

        public async void ScheduleTask(TimeSpan delay, Task task)
        {
            await Task.Delay(delay);

            StartTask(task);
        }

        public void AwaitRunningTasks(TimeSpan timeout)
        {
            var tasks = _runningTasks.Keys.ToArray();
            Task.WaitAll(tasks, timeout);
        }

        public void StartTask(Task task)
        {
            _runningTasks.TryAdd(task, 0);
            task.ContinueWith(completedTask =>
            {
                _runningTasks.TryRemove(completedTask, out _);
            }, TaskContinuationOptions.ExecuteSynchronously);
            task.Start();
        }

        public void StartTask<T>(Task<T> task)
        {
            _runningTasks.TryAdd(task, 0);
            task.ContinueWith(completedTask =>
            {
                _runningTasks.TryRemove(completedTask, out _);
            }, TaskContinuationOptions.ExecuteSynchronously);
            task.Start();
        }

        /// <summary>
        /// Executes the specified method. 
        /// </summary>
        public void QueueWorkItem(Action action)
        {
            QueueWorkItem(action, null);
        }

        /// <summary>
        /// Executes the specified method. 
        /// </summary>
        public void QueueWorkItem(Action action, Func<string> contextStringSource)
        {
            if (HttpContext.Current != null)
            {
                throw new NotSupportedException("Asynchronous operations in asp.net context are prohibited.");
            }

            var stackTrace = new StackTrace(true);
            var task = new Task(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    string exceptionText =
$@"{GetContextString(contextStringSource)}
{ex}
Parent stack:
{stackTrace}";
                    if (ex is UserMessageException)
                    {
                        Trace.TraceWarning(exceptionText);
                    }
                    else
                    {
                        Trace.TraceError(exceptionText);
                    }
                }
            });
            StartTask(task);
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        private string GetContextString(Func<string> contextStringSource)
        {
            if (contextStringSource == null)
            {
                return "null";
            }

            try
            {
                return contextStringSource();
            }
            catch (Exception)
            {
                return "exception";
            }
        }
    }
}