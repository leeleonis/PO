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

            dbC.TaskScheduler.Add(TaskScheduler);
            dbC.SaveChanges();
        }

        public void AddWork(Func<string> work)
        {
            StatusLog(EnumData.TaskStatus.執行中);

            IAsyncResult asyncResult = work.BeginInvoke(WorkComplete, work);
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

        public void WorkComplete(IAsyncResult asyncResult)
        {
            if (asyncResult == null) return;

            var result = (asyncResult.AsyncState as Func<string>).EndInvoke(asyncResult).ToString();

            if (!string.IsNullOrEmpty(result)) AddLog(result);

            StatusLog(EnumData.TaskStatus.執行完);
        }

        public void AddLog(string description)
        {
            dbC.TaskLog.Add(new TaskLog()
            {
                Scheduler = TaskScheduler.ID,
                Description = description,
                CreateBy = AdminName,
                CreateAt = DateTime.UtcNow
            });
            dbC.SaveChanges();
        }

        public void StatusLog(EnumData.TaskStatus status)
        {
            TaskScheduler.Status = (byte)status;
            TaskScheduler.UpdateAt = DateTime.UtcNow;
            TaskScheduler.UpdateBy = AdminName;
            dbC.Entry(TaskScheduler).State = System.Data.Entity.EntityState.Modified;
            dbC.SaveChanges();
        }

        public void Fail(string message)
        {
            TaskScheduler.Result = message;
            StatusLog(EnumData.TaskStatus.執行失敗);
        }
    }
}