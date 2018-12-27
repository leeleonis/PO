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
    public class CurrencyController : BaseController
    {
        // GET: Currency
        public ActionResult Index()
        {
            return View();
        }

        // GET: Currency/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currency.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // GET: Currency/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Currency/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IsDefault,ID,Name,Code,EXRate")] Currency currency)
        {
            currency.CreateAt = DateTime.UtcNow;
            currency.CreateBy = Session["AdminName"].ToString();

            db.Currency.Add(currency);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Currency/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currency.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // POST: Currency/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IsDefault,ID,Name,Code,EXRate,CreateBy,CreateAt,UpdateBy,UpdateAt")] Currency currency)
        {
            if (ModelState.IsValid)
            {
                db.Entry(currency).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(currency);
        }

        // GET: Currency/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currency.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // POST: Currency/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Currency currency = db.Currency.Find(id);
            db.Currency.Remove(currency);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(CurrencyFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var CurrencyFilter = db.Currency.AsQueryable();
            if (filter.ID.HasValue) CurrencyFilter = CurrencyFilter.Where(c => c.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) CurrencyFilter = CurrencyFilter.Where(c => c.Name.ToLower().Contains(filter.Name.ToLower()));
            if (!string.IsNullOrEmpty(filter.Code)) CurrencyFilter = CurrencyFilter.Where(c => c.Code.ToLower().Contains(filter.Code.ToLower()));
            if (filter.EXRate.HasValue) CurrencyFilter = CurrencyFilter.Where(c => c.EXRate.Equals(filter.EXRate.Value));

            if (CurrencyFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = CurrencyFilter.Count();
                var results = CurrencyFilter.OrderByDescending(c => c.CreateAt).Skip(start).Take(length).ToList();

                dataList.AddRange(results);
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(Currency updateData)
        {
            AjaxResult result = new AjaxResult();

            Currency currency = db.Currency.Find(updateData.ID);
            SetUpdateData(currency, updateData, new string[] { "Name", "EXRate" });
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
