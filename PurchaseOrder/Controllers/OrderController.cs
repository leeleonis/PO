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
    public class OrderController : BaseController
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Orders order = db.Orders.Find(id);

            if (order == null) return HttpNotFound();

            ViewBag.CompanyList = db.Company.AsNoTracking().Where(c => c.IsEnable).OrderBy(c => c.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            ViewBag.MethodList = db.ShippingMethods.AsNoTracking().Where(m => m.IsEnable).OrderBy(m => m.Name).Select(m => new SelectListItem() { Text = m.Name, Value = m.ID.ToString() }).ToList();
            ViewBag.WarehouseList = db.Warehouse.AsNoTracking().Where(w => w.IsEnable && w.IsSellable).OrderBy(w => w.Name)
                .Select(w => new SelectListItem() { Text = w.Name, Value = w.ID.ToString() }).ToList();
            ViewBag.CurrencyList = db.Currency.AsNoTracking().Select(c => new SelectListItem() { Text = c.Code + " - " + c.Name, Value = c.ID.ToString() }).OrderBy(c => c.Text).ToList();

            return View(order);
        }

        // POST: Order/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Orders order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PackageSave(Packages updatePackage)
        {
            AjaxResult result = new AjaxResult();

            var package = db.Packages.Find(updatePackage.ID);

            try
            {
                if (package == null) throw new Exception("Not found package!");
            }
            catch(Exception e)
            {
                result.SetError(e.InnerException?.Message ?? e.Message);
            }

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
