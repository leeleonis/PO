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
        public Task<string> Taskk;
        public TaskScheduler TaskScheduler;

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
            Taskk.ContinueWith((Task) =>
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
            }, TaskContinuationOptions.None);

            Task.Factory.StartNew(work).GetAwaiter().GetResult();
        }

        public string AddWork(Func<object, string> work, object data)
        {
            Taskk = Task.Factory.StartNew(work, data);

            Taskk.ContinueWith((Task) =>
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
            }, TaskContinuationOptions.None);

            return Taskk.GetAwaiter().GetResult();
            
        }

        internal void AddLog(string description)
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

        internal void StatusLog(EnumData.TaskStatus status)
        {
            lock (TaskScheduler)
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