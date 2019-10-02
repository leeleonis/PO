using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PurchaseOrderSys.Models
{
    public class JobProcess : Common, IDisposable
    {
        public Task<string> Task;
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

        public void StartWork()
        {
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
            }, TaskContinuationOptions.None);

            StatusLog(EnumData.TaskStatus.執行中);
            Task.Start();
        }

        internal void AddWord(Func<string> work)
        {
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
            }, TaskContinuationOptions.None);

            StatusLog(EnumData.TaskStatus.執行中);

            Task.Start();
        }

        internal void AddWord(Func<object, string> work, object data)
        {
            StatusLog(EnumData.TaskStatus.執行中);

            Task = new Task<string>(work, data);
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
            }, TaskContinuationOptions.None);

            Task.Start();
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
            TaskScheduler.Status = (byte)status;
            TaskScheduler.UpdateAt = DateTime.UtcNow;
            TaskScheduler.UpdateBy = AdminName;
            db.SaveChanges();
        }

        internal void Fail(string message)
        {
            TaskScheduler.Result = message;
            StatusLog(EnumData.TaskStatus.執行失敗);
        }
    }
}