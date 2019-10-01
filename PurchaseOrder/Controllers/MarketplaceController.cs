using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using inventorySKU;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class MarketplaceController : BaseController
    {
        private string[] EditList = new string[] { "FullName", "GlobalID", "CountryCode", "CompanyID", "CurrencyID", "NetoGroup", "DispatchTime" };
        // GET: Marketplace
        public ActionResult Index()
        {
            return View();
        }

        // GET: Marketplace/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Marketplace marketplace = db.Marketplace.Find(id);
            if (marketplace == null)
            {
                return HttpNotFound();
            }
            return View(marketplace);
        }

        // GET: Marketplace/Create
        public ActionResult Create()
        {
            ViewBag.CurrencyID = new SelectList(db.Currency.Select(c => new SelectListItem() { Text = c.Code + " - " + c.Name, Value = c.ID.ToString() })).Items;
            ViewBag.NetoGroup = new SelectList(db.NetoGroup, "ID", "Name");
            ViewBag.CompanyID = new SelectList(db.Company.Where(c => c.IsEnable), "ID", "Name");
            return View();
        }

        // POST: Marketplace/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IsEnable,ID,FullName,GlobalID,CountryCode,CompanyID,CurrencyID,NetoGroup,DispatchTime")] Marketplace marketplace)
        {
            marketplace.CreateAt = DateTime.UtcNow;
            marketplace.CreateBy = Session["AdminName"].ToString();
            db.Marketplace.Add(marketplace);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = marketplace.ID });
        }

        // GET: Marketplace/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Marketplace marketplace = db.Marketplace.Find(id);
            if (marketplace == null) return HttpNotFound();

            ViewBag.CurrencyID = new SelectList(db.Currency.Select(c => new SelectListItem() { Text = c.Code + " - " + c.Name, Value = c.ID.ToString(), Selected = c.ID.Equals(marketplace.CurrencyID.Value) })).Items;
            ViewBag.NetoGroup = new SelectList(db.NetoGroup, "ID", "Name", marketplace.NetoGroup);
            ViewBag.CompanyID = new SelectList(db.Company, "ID", "Name", marketplace.CompanyID);
            return View(marketplace);
        }

        // POST: Marketplace/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FullName,GlobalID,CountryCode,CompanyID,CurrencyID,NetoGroup,DispatchTime")] Marketplace updateData)
        {
            Marketplace marketplace = db.Marketplace.Find(updateData.ID);
            if (marketplace == null) return HttpNotFound();

            SetEditDatas(marketplace, updateData, EditList);
            db.SaveChanges();

            ViewBag.CurrencyID = new SelectList(db.Currency.Select(c => new SelectListItem() { Text = c.Code + " - " + c.Name, Value = c.ID.ToString(), Selected = c.ID.Equals(marketplace.CurrencyID.Value) })).Items;
            ViewBag.NetoGroup = new SelectList(db.NetoGroup, "ID", "Name", marketplace.NetoGroup);
            ViewBag.CompanyID = new SelectList(db.Company, "ID", "Name", marketplace.CompanyID);
            return View(marketplace);
        }

        // GET: Marketplace/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Marketplace marketplace = db.Marketplace.Find(id);
            if (marketplace == null)
            {
                return HttpNotFound();
            }
            return View(marketplace);
        }

        // POST: Marketplace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Marketplace marketplace = db.Marketplace.Find(id);
            db.Marketplace.Remove(marketplace);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(MarketplaceFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var MarketplaceFilter = db.Marketplace.Where(m => m.IsEnable).AsQueryable();
            if (filter.ID.HasValue) MarketplaceFilter = MarketplaceFilter.Where(m => m.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.FullName)) MarketplaceFilter = MarketplaceFilter.Where(m => m.FullName.ToLower().Contains(filter.FullName.ToLower()));
            if (!string.IsNullOrEmpty(filter.GlobalID)) MarketplaceFilter = MarketplaceFilter.Where(m => m.GlobalID.ToLower().Equals(filter.GlobalID.ToLower()));
            if (!string.IsNullOrEmpty(filter.CountryCode)) MarketplaceFilter = MarketplaceFilter.Where(m => m.CountryCode.ToLower().Equals(filter.CountryCode.ToLower()));
            if (filter.CompanyID.HasValue) MarketplaceFilter = MarketplaceFilter.Where(m => m.CompanyID.Value.Equals(filter.CompanyID.Value));
            if (filter.CurrencyID.HasValue) MarketplaceFilter = MarketplaceFilter.Where(m => m.CurrencyID.Value.Equals(filter.CurrencyID.Value));
            if (filter.NetoGroup.HasValue) MarketplaceFilter = MarketplaceFilter.Where(m => m.NetoGroup.Value.Equals(filter.NetoGroup.Value));
            if (filter.DispatchTime.HasValue) MarketplaceFilter = MarketplaceFilter.Where(m => m.DispatchTime.Equals(filter.DispatchTime.Value));

            if (MarketplaceFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = MarketplaceFilter.Count();
                var results = MarketplaceFilter.OrderBy(m => m.ID).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(m => new
                {
                    m.ID,
                    m.FullName,
                    m.GlobalID,
                    m.CountryCode,
                    m.CompanyID,
                    m.CurrencyID,
                    m.NetoGroup,
                    m.DispatchTime
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(Marketplace updateData)
        {
            AjaxResult result = new AjaxResult();

            Marketplace marketpalce = db.Marketplace.Find(updateData.ID);
            SetUpdateData(marketpalce, updateData, EditList);
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
