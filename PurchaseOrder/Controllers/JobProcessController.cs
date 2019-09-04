using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using inventorySKU;
using PurchaseOrderSys.Helpers;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class JobProcessController : BaseController
    {
        // GET: JobProcess
        public ActionResult Index()
        {
            ViewData["AdminList"] = db.AdminUser.Where(u => u.IsEnable).OrderBy(u => u.Name).ToList();

            return View();
        }

        public ActionResult GetData(TaskSchedulerFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var TaskFilter = db.TaskScheduler.AsQueryable();
            if (filter.ID.HasValue) TaskFilter = TaskFilter.Where(t => t.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) TaskFilter = TaskFilter.Where(t => t.Name.ToLower().Contains(filter.Name.ToLower()));
            if (filter.Status.HasValue) TaskFilter = TaskFilter.Where(t => t.Status.Equals(filter.Status.Value));
            if (!string.IsNullOrEmpty(filter.AdminName)) TaskFilter = TaskFilter.Where(t => t.CreateBy.ToLower().Contains(filter.AdminName.ToLower()));
            if (filter.DateFrom.HasValue)
            {
                DateTime dateFrom = new DateTime(filter.DateFrom.Value.Year, filter.DateFrom.Value.Month, filter.DateFrom.Value.Day, 0, 0, 0).ToUniversalTime();
                TaskFilter = TaskFilter.Where(t => DateTime.Compare(t.CreateAt, dateFrom) >= 0);
            }
            if (filter.DateTo.HasValue)
            {
                DateTime dateTo = new DateTime(filter.DateTo.Value.Year, filter.DateTo.Value.Month, filter.DateTo.Value.Day, 0, 0, 0).AddDays(1).ToUniversalTime();
                TaskFilter = TaskFilter.Where(t => DateTime.Compare(t.CreateAt, dateTo) < 0);
            }

            if (TaskFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = TaskFilter.Count();
                var results = TaskFilter.OrderByDescending(a => a.CreateAt).Skip(start).Take(length).ToList();

                TimeZoneConvert DateTimeConvert = new TimeZoneConvert();
                dataList.AddRange(results.Select(t => new
                {
                    t.ID,
                    t.Name,
                    t.Result,
                    AdminName = t.CreateBy,
                    Status = Enum.GetName(typeof(EnumData.TaskStatus), t.Status),
                    Date = DateTimeConvert.InitDateTime(t.CreateAt, EnumData.TimeZone.UTC).ConvertDateTime(EnumData.TimeZone.TST).ToString("MM/dd/yyyy hh:mm tt")
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TaskLogList(int TaskID)
        {
            var task = db.TaskScheduler.Find(TaskID);

            if (task != null)
            {
                return PartialView("_TaskLog", task);
            }

            return new EmptyResult();
        }
    }
}