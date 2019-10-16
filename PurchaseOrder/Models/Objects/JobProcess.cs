using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PurchaseOrderSys.Models
{
    public class JobProcess : Common, IDisposable
    {
        public Thread Work;
        public Task<string> Task;
        public TaskScheduler TaskScheduler;

        public JobProcess(string Name)
        {
            lock (db)
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

            Task.ContinueWith((Task) =>
            {
                if (Task.IsFaulted)
                {
                    Fail(Task.Exception.Message);
                }
                else if (Task.IsCanceled)
                {
                    Fail("工作已取消");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(Task.Result))
                    {
                        Fail(Task.Result);
                    }
                    else
                    {
                        StatusLog(EnumData.TaskStatus.執行完);
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public void AddWork(Func<object, string> work, object data)
        {
            Task = System.Threading.Tasks.Task.Factory.StartNew(work, data);
            
            Task.ContinueWith((Task) =>
            {
                if (Task.IsFaulted)
                {
                    Fail(Task.Exception.Message);
                }
                else if (Task.IsCanceled)
                {
                    Fail("工作已取消");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(Task.Result))
                    {
                        Fail(Task.Result);
                    }
                    else
                    {
                        StatusLog(EnumData.TaskStatus.執行完);
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);   
        }

        internal void AddLog(string description)
        {
            lock (db)
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
        }

        internal void StatusLog(EnumData.TaskStatus status)
        {
            lock (db)
            {
                TaskScheduler.Status = (byte)status;
                TaskScheduler.UpdateAt = DateTime.UtcNow;
                TaskScheduler.UpdateBy = AdminName;
                db.SaveChanges();
            }
        }

        internal void Fail(string message)
        {
            TaskScheduler.Result = message;
            StatusLog(EnumData.TaskStatus.執行失敗);
        }
    }
}