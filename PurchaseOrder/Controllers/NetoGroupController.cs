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
    public class NetoGroupController : BaseController
    {
        // GET: NetoGroup
        public ActionResult Index()
        {
            return View();
        }

        // GET: NetoGroup/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NetoGroup netoGroup = db.NetoGroup.Find(id);
            if (netoGroup == null)
            {
                return HttpNotFound();
            }
            return View(netoGroup);
        }

        // GET: NetoGroup/Create
        public ActionResult Create()
        {
            ViewBag.CurrencyID = new SelectList(db.Currency, "ID", "Name");
            return View();
        }

        // POST: NetoGroup/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,CurrencyID,MinSpend,SaleCategory,SaleRequire,CreateBy,CreateAt,UpdateBy,UpdateAt")] NetoGroup netoGroup)
        {
            if (ModelState.IsValid)
            {
                db.NetoGroup.Add(netoGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CurrencyID = new SelectList(db.Currency, "ID", "Name", netoGroup.CurrencyID);
            return View(netoGroup);
        }

        // GET: NetoGroup/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NetoGroup netoGroup = db.NetoGroup.Find(id);
            if (netoGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.CurrencyID = new SelectList(db.Currency, "ID", "Name", netoGroup.CurrencyID);
            return View(netoGroup);
        }

        // POST: NetoGroup/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,CurrencyID,MinSpend,SaleCategory,SaleRequire,CreateBy,CreateAt,UpdateBy,UpdateAt")] NetoGroup netoGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(netoGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CurrencyID = new SelectList(db.Currency, "ID", "Name", netoGroup.CurrencyID);
            return View(netoGroup);
        }

        // GET: NetoGroup/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NetoGroup netoGroup = db.NetoGroup.Find(id);
            if (netoGroup == null)
            {
                return HttpNotFound();
            }
            return View(netoGroup);
        }

        // POST: NetoGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NetoGroup netoGroup = db.NetoGroup.Find(id);
            db.NetoGroup.Remove(netoGroup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(NetoGroupFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var NetoGroupFilter = db.NetoGroup.AsQueryable();
            if (filter.ID.HasValue) NetoGroupFilter = NetoGroupFilter.Where(g => g.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) NetoGroupFilter = NetoGroupFilter.Where(g => g.Name.ToLower().Contains(filter.Name.ToLower()));
            if (filter.MinSpend.HasValue) NetoGroupFilter = NetoGroupFilter.Where(g => g.MinSpend.Equals(filter.MinSpend.Value));
            if (!string.IsNullOrEmpty(filter.SaleCategory)) NetoGroupFilter = NetoGroupFilter.Where(g => g.SaleCategory.ToLower().Equals(filter.SaleCategory.ToLower()));
            if (filter.SaleRequire.HasValue) NetoGroupFilter = NetoGroupFilter.Where(g => g.SaleRequire.Equals(filter.SaleRequire.Value));

            if (NetoGroupFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = NetoGroupFilter.Count();
                var results = NetoGroupFilter.OrderBy(g => g.ID).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(g => new
                {
                    g.ID,
                    g.Name,
                    g.CurrencyID,
                    g.MinSpend,
                    g.SaleCategory,
                    g.SaleRequire
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(NetoGroup updateData)
        {
            AjaxResult result = new AjaxResult();

            NetoGroup group = db.NetoGroup.Find(updateData.ID);
            SetUpdateData(group, updateData, new string[] { "CurrencyID", "MinSpend", "SaleCategory", "SaleRequire" });
            db.SaveChanges();

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
