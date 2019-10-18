using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PurchaseOrderSys.Models
{
    public class JobProcess : Common, IDisposable
    {
        public Thread Work;
        public Task<string> Task;
        private TaskScheduler TaskScheduler;

        public JobProcess(string Name)
        {
                TaskScheduler = new TaskScheduler()
                {
                    Name = Name,
                    Status = (byte)EnumData.TaskStatus.未執行,
                    CreateBy = AdminName,
                    CreateAt = DateTime.UtcNow
                };

                db.TaskScheduler.Add(TaskScheduler);
                db.SaveChanges();
        }

        public void StartWork(ParameterizedThreadStart work)
        {
            Work = new Thread(work);

            StatusLog(EnumData.TaskStatus.執行中);

            Work.Start(this);
        }

        public void FinishWork()
        {
            StatusLog(EnumData.TaskStatus.執行完);
        }

        public void AddWork(Func<string> work)
        {
            Task = System.Threading.Tasks.Task.Factory.StartNew(work);

            //Task.ContinueWith((Task) =>
            //{
            //    if (Task.IsFaulted)
            //    {
            //        Fail(Task.Exception.Message);
            //    }
            //    else if (Task.IsCanceled)
            //    {
            //        Fail("工作已取消");
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrWhiteSpace(Task.Result))
            //        {
            //            Fail(Task.Result);
            //        }
            //        else
            //        {
            //            StatusLog(EnumData.TaskStatus.執行完);
            //        }
            //    }
            //}, TaskContinuationOptions.ExecuteSynchronously);
        }

        public void AddWork(Func<object, string> work, object data)
        {
            Task = System.Threading.Tasks.Task.Factory.StartNew(work, data);

            //Task.ContinueWith((Task) =>
            //{
            //    if (Task.IsFaulted)
            //    {
            //        Fail(Task.Exception.Message);
            //    }
            //    else if (Task.IsCanceled)
            //    {
            //        Fail("工作已取消");
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrWhiteSpace(Task.Result))
            //        {
            //            Fail(Task.Result);
            //        }
            //        else
            //        {
            //            StatusLog(EnumData.TaskStatus.執行完);
            //        }
            //    }
            //}, TaskContinuationOptions.ExecuteSynchronously);
        }

        public void Func_test()
        {
            StatusLog(EnumData.TaskStatus.執行中);
            AddLog("Task Test3");
            Thread.Sleep(5000);
            FinishWork();
        }

        public void AddLog(string description)
        {
                db.TaskLog.Add(new TaskLog()
                {
                    Scheduler = TaskScheduler.ID,
                    Description = description,
                    CreateBy = AdminName,
                    CreateAt = DateTime.UtcNow
                });
                db.SaveChanges();
        }

        public void StatusLog(EnumData.TaskStatus status)
        {
                TaskScheduler.Status = (byte)status;
                TaskScheduler.UpdateAt = DateTime.UtcNow;
                TaskScheduler.UpdateBy = AdminName;
                db.SaveChanges();
        }

        public void Fail(string message)
        {
            TaskScheduler.Result = message;
            StatusLog(EnumData.TaskStatus.執行失敗);
        }
    }

    public class LimitedConcurrencyLevelTaskScheduler : System.Threading.Tasks.TaskScheduler
    {
        /// <summary>Whether the current thread is processing work items.</summary>
        [ThreadStatic]
        private static bool _currentThreadIsProcessingItems;
        /// <summary>The list of tasks to be executed.</summary>
        private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)
        /// <summary>The maximum concurrency level allowed by this scheduler.</summary>
        private readonly int _maxDegreeOfParallelism;
        /// <summary>Whether the scheduler is currently processing work items.</summary>
        private int _delegatesQueuedOrRunning = 0; // protected by lock(_tasks)

        /// <summary>
        /// Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the
        /// specified degree of parallelism.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        /// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be queued.</param>
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough
            // delegates currently queued or running to process tasks, schedule another.
            lock (_tasks)
            {
                _tasks.AddLast(task);
                if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                {
                    ++_delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        /// <summary>
        /// Informs the ThreadPool that there's work to be executed for this scheduler.
        /// </summary>
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                // Note that the current thread is now processing work items.
                // This is necessary to enable inlining of tasks into this thread.
                _currentThreadIsProcessingItems = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        Task item;
                        lock (_tasks)
                        {
                            // When there are no more items to be processed,
                            // note that we're done processing, and get out.
                            if (_tasks.Count == 0)
                            {
                                --_delegatesQueuedOrRunning;
                                break;
                            }

                            // Get the next item from the queue
                            item = _tasks.First.Value;
                            _tasks.RemoveFirst();
                        }

                        // Execute the task we pulled out of the queue
                        base.TryExecuteTask(item);
                    }
                }
                // We're done processing items on the current thread
                finally { _currentThreadIsProcessingItems = false; }
            }, null);
        }

        /// <summary>Attempts to execute the specified task on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued"></param>
        /// <returns>Whether the task could be executed on the current thread.</returns>
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!_currentThreadIsProcessingItems) return false;

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued) TryDequeue(task);

            // Try to run the task.
            return TryExecuteTask(task);
        }

        /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
        /// <param name="task">The task to be removed.</param>
        /// <returns>Whether the task could be found and removed.</returns>
        protected sealed override bool TryDequeue(Task task)
        {
            lock (_tasks) return _tasks.Remove(task);
        }

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

        /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
        /// <returns>An enumerable of the tasks currently scheduled.</returns>
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_tasks, ref lockTaken);
                if (lockTaken) return _tasks.ToArray();
                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_tasks);
            }
        }
    }
}