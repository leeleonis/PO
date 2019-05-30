﻿using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using inventorySKU;
namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
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
            var VendorLIst = db.VendorLIst.Find(id);
            var BrandList = db.Brand.Where(x => x.IsEnable).ToList();
            if (!VendorLIst.Brand.Any())
            {
                foreach (var item in BrandList)
                {
                    VendorLIst.Brand.Add(item);
                }
                db.SaveChanges();
            }
            ViewBag.BrandList = BrandList;
            return View(VendorLIst);
        }
        [HttpPost]
        public ActionResult Edit(VendorLIst VendorLIst ,List<int> Brand)
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
            oVendorLIst.Email = VendorLIst.Email;
            oVendorLIst.EmailCC = VendorLIst.EmailCC;
            oVendorLIst.UpdateBy = UserBy;
            oVendorLIst.UpdateAt = DateTime.UtcNow;

            var BrandList = db.Brand.Where(x => x.IsEnable&& Brand.Contains(x.ID)).ToList();
            foreach (var item in oVendorLIst.Brand.ToList())
            {
                oVendorLIst.Brand.Remove(item);
            }
            foreach (var item in BrandList)
            {
                oVendorLIst.Brand.Add(item);
            }
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