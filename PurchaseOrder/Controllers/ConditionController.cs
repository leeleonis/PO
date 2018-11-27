using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    //[CheckSession]
    public class ConditionController : BaseController
    {
        string[] EditList = new string[] { "Amazon", "eBay", "Buy_com", "NewEgg_com", "Sears", "Suffix" };
        // GET: Condition
        public ActionResult Index()
        {
            return View();
        }

        // GET: Condition/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Condition.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            return View(condition);
        }

        // GET: Condition/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Condition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IsEnable,Amazon,eBay,Buy_com,NewEgg_com,Sears,Suffix")] Condition condition, ConditionLang langData)
        {
            condition.CreateAt = DateTime.UtcNow;
            condition.CreateBy = Session["AdminName"].ToString();

            db.Condition.Add(condition);
            db.SaveChanges();

            langData.ConditionID = condition.ID;
            langData.LangID = EnumData.DataLangList().Keys.First();
            langData.CreateAt = DateTime.UtcNow;
            langData.CreateBy = Session["AdminName"].ToString();

            db.ConditionLang.Add(langData);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = condition.ID });
        }

        // GET: Condition/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Condition condition = db.Condition.Find(id);
            if (condition == null) return HttpNotFound();

            ViewBag.LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key });
            return View(condition);
        }

        // POST: Condition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Amazon,eBay,Buy_com,NewEgg_com,Sears,Suffix")] Condition updateData, ConditionLang langData)
        {
            Condition condition = db.Condition.Find(updateData.ID);
            if (condition == null) return HttpNotFound();

            SetUpdateData(condition, updateData, EditList);
            db.Entry(condition).State = EntityState.Modified;

            if (condition.ConditionLang.Any(l => l.LangID.Equals(langData.LangID)))
            {
                ConditionLang attributeLang = condition.ConditionLang.First(l => l.LangID.Equals(langData.LangID));
                SetUpdateData(attributeLang, langData, new string[] { "Name" });
                db.Entry(attributeLang).State = EntityState.Modified;
            }
            else
            {
                langData.ConditionID = condition.ID;
                langData.CreateAt = condition.UpdateAt.Value;
                langData.CreateBy = condition.UpdateBy;
                db.Entry(langData).State = EntityState.Added;
            }

            db.SaveChanges();

            ViewBag.LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key });
            return View(condition);
        }

        // GET: Condition/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Condition.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            return View(condition);
        }

        // POST: Condition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Condition condition = db.Condition.Find(id);
            db.Condition.Remove(condition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(ConditionFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var LangID = !string.IsNullOrEmpty(filter.LangID) ? filter.LangID : EnumData.DataLangList().First().Key;
            var CondtionFilter = db.Condition.Include(c => c.ConditionLang).AsQueryable();
            if (filter.ID.HasValue) CondtionFilter = CondtionFilter.Where(c => c.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) CondtionFilter = CondtionFilter.Where(c => c.ConditionLang.Any(l => l.LangID.Equals(LangID) && l.Name.ToLower().Contains(filter.Name.ToLower())));
            if (!string.IsNullOrEmpty(filter.Amazon)) CondtionFilter = CondtionFilter.Where(c => c.Amazon.ToLower().Contains(filter.Amazon.ToLower()));
            if (!string.IsNullOrEmpty(filter.eBay)) CondtionFilter = CondtionFilter.Where(c => c.eBay.ToLower().Contains(filter.eBay.ToLower()));
            if (!string.IsNullOrEmpty(filter.Buy_com)) CondtionFilter = CondtionFilter.Where(c => c.Buy_com.ToLower().Contains(filter.Buy_com.ToLower()));
            if (!string.IsNullOrEmpty(filter.NewEgg_com)) CondtionFilter = CondtionFilter.Where(c => c.NewEgg_com.ToLower().Contains(filter.NewEgg_com.ToLower()));
            if (!string.IsNullOrEmpty(filter.Sears)) CondtionFilter = CondtionFilter.Where(c => c.Sears.ToLower().Contains(filter.Sears.ToLower()));
            if (!string.IsNullOrEmpty(filter.Suffix)) CondtionFilter = CondtionFilter.Where(c => c.Suffix.ToLower().Contains(filter.Suffix.ToLower()));

            if (CondtionFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = CondtionFilter.Count();
                var results = CondtionFilter.OrderByDescending(a => a.CreateAt).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(c => new
                {
                    c.ID,
                    Name = c.ConditionLang.Any(l => l.LangID.Equals(LangID)) ? c.ConditionLang.First(l => l.LangID.Equals(LangID)).Name : "",
                    c.Amazon,
                    c.eBay,
                    c.Buy_com,
                    c.NewEgg_com,
                    c.Sears,
                    c.Suffix
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(Condition updateData, ConditionLang langData)
        {
            AjaxResult result = new AjaxResult();

            Condition condition = db.Condition.Find(updateData.ID);
            SetUpdateData(condition, updateData, EditList);
            db.Entry(condition).State = EntityState.Modified;

            langData.LangID = EnumData.DataLangList().First().Key;
            ConditionLang condtionLang = condition.ConditionLang.First(l => l.LangID.Equals(langData.LangID));
            SetUpdateData(condtionLang, langData, new string[] { "Name" });
            db.Entry(condtionLang).State = EntityState.Modified;

            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLangData(int ID, string LangID)
        {
            AjaxResult result = new AjaxResult
            {
                data = new { db.ConditionLang.FirstOrDefault(l => l.ConditionID.Equals(ID) && l.LangID.Equals(LangID))?.Name }
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
