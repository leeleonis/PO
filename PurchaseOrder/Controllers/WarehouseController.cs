using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class WarehouseController : BaseController
    {
        // GET: Warehouse
        public ActionResult Index()
        {
            var Warehouse = db.Warehouse.Where(x => x.IsEnable);
            return View(Warehouse);
        }
        public ActionResult Create()
        {
            ViewBag.CompanyList = db.Company.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Create(Warehouse Warehouse)
        {
            Warehouse.CreateBy = UserBy;
            Warehouse.CreateAt = DateTime.UtcNow;
            db.Warehouse.Add(Warehouse);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                var s = ex;
            }

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int ID)
        {
            ViewBag.CompanyList = db.Company.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            var Warehouse = db.Warehouse.Find(ID);
            if (Warehouse.IsEnable)
            {
                return View(Warehouse);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Edit(Warehouse Warehouse)
        {
            var OldWarehouse = db.Warehouse.Find(Warehouse.ID);
           

            OldWarehouse.UpdateBy = UserBy;
            OldWarehouse.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int ID)
        {
            var OldWarehouse = db.Warehouse.Find(ID);
            OldWarehouse.IsEnable = false;
            OldWarehouse.UpdateBy = UserBy;
            OldWarehouse.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}