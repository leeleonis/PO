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
    public class BoxTypeController : BaseController
    {
        // GET: BoxType
        public ActionResult Index()
        {
            return View();
        }

        // GET: BoxType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BoxType boxType = db.BoxType.Find(id);
            if (boxType == null)
            {
                return HttpNotFound();
            }
            return View(boxType);
        }

        // GET: BoxType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BoxType/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Width,Length,Height,Weight")] BoxType boxType)
        {
            boxType.CreateAt = DateTime.UtcNow;
            boxType.CreateBy = "Test";

            db.BoxType.Add(boxType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: BoxType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BoxType boxType = db.BoxType.Find(id);
            if (boxType == null)
            {
                return HttpNotFound();
            }
            return View(boxType);
        }

        // POST: BoxType/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Width,Length,Height,Weight,CreateBy,CreateAt,UpdateBy,UpdateAt")] BoxType boxType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(boxType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(boxType);
        }

        // GET: BoxType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BoxType boxType = db.BoxType.Find(id);
            if (boxType == null)
            {
                return HttpNotFound();
            }
            return View(boxType);
        }

        // POST: BoxType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BoxType boxType = db.BoxType.Find(id);
            db.BoxType.Remove(boxType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(BoxTypeFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var BoxTypeFilter = db.BoxType.AsQueryable();
            if (filter.ID.HasValue) BoxTypeFilter = BoxTypeFilter.Where(b => b.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) BoxTypeFilter = BoxTypeFilter.Where(b => b.Name.Equals(filter.Name));
            if (filter.Width.HasValue) BoxTypeFilter = BoxTypeFilter.Where(b => b.Width.Equals(filter.Width.Value));
            if (filter.Length.HasValue) BoxTypeFilter = BoxTypeFilter.Where(b => b.Length.Equals(filter.Length.Value));
            if (filter.Height.HasValue) BoxTypeFilter = BoxTypeFilter.Where(b => b.Height.Equals(filter.Height.Value));
            if (filter.Weight.HasValue) BoxTypeFilter = BoxTypeFilter.Where(b => b.Weight.Equals(filter.Weight.Value));

            if (BoxTypeFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = BoxTypeFilter.Count();
                var results = BoxTypeFilter.OrderByDescending(b => b.CreateAt).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(b => new
                {
                    b.ID,
                    b.Name,
                    b.Width,
                    WidthInch = Math.Round(b.Width * 0.0393700787, 9),
                    b.Length,
                    LengthInch = Math.Round(b.Length * 0.0393700787, 9),
                    b.Height,
                    HeightInch = Math.Round(b.Height * 0.0393700787, 9),
                    b.Weight,
                    WeightLbs = Math.Round(b.Weight * 0.00220462262, 9)
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(BoxType updateData)
        {
            AjaxResult result = new AjaxResult();

            BoxType boxType = db.BoxType.Find(updateData.ID);
            boxType.Name = updateData.Name;
            boxType.Width = updateData.Width;
            boxType.Length = updateData.Length;
            boxType.Height = updateData.Height;
            boxType.Weight = updateData.Weight;
            boxType.UpdateAt = DateTime.UtcNow;
            boxType.UpdateBy = Session["AdminName"].ToString();
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
