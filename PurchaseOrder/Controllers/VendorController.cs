using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class VendorController : BaseController
    {
        // GET: Vendor
        public ActionResult Index()
        {
            var VendorLIst = db.VendorLIst.Where(x => x.IsEnable);
            return View(VendorLIst);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(VendorLIst VendorLIst)
        {
            VendorLIst.CreateBy = UserBy;
            VendorLIst.CreateAt = DateTime.UtcNow;
            db.VendorLIst.Add(VendorLIst);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
          var VendorLIst= db.VendorLIst.Find(id);
            return View(VendorLIst);
        }
        [HttpPost]
        public ActionResult Edit(VendorLIst VendorLIst)
        {
            var oVendorLIst = db.VendorLIst.Find(VendorLIst.ID);
            oVendorLIst.VendorNo = VendorLIst.VendorNo;
            oVendorLIst.Name = VendorLIst.Name;
            oVendorLIst.Principal = VendorLIst.Principal;
            oVendorLIst.Address = VendorLIst.Address;
            oVendorLIst.TEL = VendorLIst.TEL;
            oVendorLIst.Phone = VendorLIst.Phone;
            oVendorLIst.Fax = VendorLIst.Fax;
            oVendorLIst.Currency = VendorLIst.Currency;
            oVendorLIst.Tax = VendorLIst.Tax;
            oVendorLIst.UpdateBy = UserBy;
            oVendorLIst.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            var oVendorLIst = db.VendorLIst.Find(id);
            oVendorLIst.IsEnable = false;
            oVendorLIst.DelAt= DateTime.UtcNow;
            oVendorLIst.DelBy = UserBy;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}