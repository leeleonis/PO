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
    public class BrandController : BaseController
    {
        // GET: Brand
        public ActionResult Index()
        {
            return View();
        }

        // GET: Brand/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brand.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // GET: Brand/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Brand/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NetoID,Name,IsExport")] Brand brand)
        {
            brand.IsEnable = true;
            brand.CreateAt = DateTime.UtcNow;
            brand.CreateBy = "Test";

            db.Brand.Add(brand);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Brand/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brand.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Brand/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IsEnable,ID,NetoID,Name,IsExport,CreateBy,CreateAt,UpdateBy,UpdateAt")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(brand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        // GET: Brand/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brand.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Brand/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Brand brand = db.Brand.Find(id);
            db.Brand.Remove(brand);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(BrandFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var BrandFilter = db.Brand.AsQueryable();
            if (filter.ID.HasValue) BrandFilter = BrandFilter.Where(b => b.ID.Equals(filter.ID.Value));
            if (filter.NetoID.HasValue) BrandFilter = BrandFilter.Where(b => b.NetoID.Equals(filter.NetoID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) BrandFilter = BrandFilter.Where(b => b.Name.Equals(filter.Name));
            if (filter.IsExport.HasValue) BrandFilter = BrandFilter.Where(b => b.IsExport.Equals(filter.IsExport.Value));

            if (BrandFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = BrandFilter.Count();
                var results = BrandFilter.OrderByDescending(b => b.CreateAt).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(b => new
                {
                    b.ID,
                    b.NetoID,
                    b.Name,
                    b.IsExport
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(Brand updateData)
        {
            AjaxResult result = new AjaxResult();

            Brand brand = db.Brand.Find(updateData.ID);
            brand.NetoID = updateData.NetoID;
            brand.Name = updateData.Name;
            brand.IsExport = updateData.IsExport;
            brand.UpdateAt = DateTime.UtcNow;
            brand.UpdateBy = Session["AdminName"].ToString();
            db.Entry(brand).State = EntityState.Modified;
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
