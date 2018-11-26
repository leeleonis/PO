using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PurchaseOrderSys;
using PurchaseOrderSys.Areas.SKUSystem.Models;

namespace inventorySKU.Controllers.Product
{
    [CheckSession]
    public class SkuAttributeTypeController : BaseController
    {
        // GET: SkuAttributeType
        public ActionResult Index()
        {
            return View();
        }

        // GET: SkuAttributeType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkuAttributeType skuAttributeType = db.SkuAttributeType.Find(id);
            if (skuAttributeType == null)
            {
                return HttpNotFound();
            }
            return View(skuAttributeType);
        }

        // GET: SkuAttributeType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SkuAttributeType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IsEnable,ID,Name,CreateBy,CreateAt,UpdateBy,UpdateAt")] SkuAttributeType skuAttributeType)
        {
            if (ModelState.IsValid)
            {
                db.SkuAttributeType.Add(skuAttributeType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(skuAttributeType);
        }

        // GET: SkuAttributeType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkuAttributeType skuAttributeType = db.SkuAttributeType.Find(id);
            if (skuAttributeType == null)
            {
                return HttpNotFound();
            }
            return View(skuAttributeType);
        }

        // POST: SkuAttributeType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IsEnable,ID,Name,CreateBy,CreateAt,UpdateBy,UpdateAt")] SkuAttributeType skuAttributeType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(skuAttributeType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skuAttributeType);
        }

        // GET: SkuAttributeType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkuAttributeType skuAttributeType = db.SkuAttributeType.Find(id);
            if (skuAttributeType == null)
            {
                return HttpNotFound();
            }
            return View(skuAttributeType);
        }

        // POST: SkuAttributeType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SkuAttributeType skuAttributeType = db.SkuAttributeType.Find(id);
            db.SkuAttributeType.Remove(skuAttributeType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(SkuAttributeTypeFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var AttributeTypeFilter = db.SkuAttributeType.AsQueryable();
            if (filter.ID.HasValue) AttributeTypeFilter = AttributeTypeFilter.Where(t => t.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) AttributeTypeFilter = AttributeTypeFilter.Where(t => t.Name.ToLower().Contains(filter.Name.ToLower()));

            if (AttributeTypeFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = AttributeTypeFilter.Count();
                var results = AttributeTypeFilter.OrderBy(t => t.Order).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(t => new
                {
                    t.ID,
                    t.Name,
                    t.Order
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(SkuAttributeType updateData)
        {
            AjaxResult result = new AjaxResult();

            SkuAttributeType type = db.SkuAttributeType.Find(updateData.ID);
            SetUpdateData(type, updateData, new string[] { "Name", "Order" });
            db.Entry(type).State = EntityState.Modified;
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
