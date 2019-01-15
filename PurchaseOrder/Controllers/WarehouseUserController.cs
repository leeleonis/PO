using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using inventorySKU;
namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class WarehouseUserController : BaseController
    {
        // GET: WarehouseUser
        public ActionResult Index()
        {
            var WarehouseUser = db.WarehouseUser.Where(x => x.IsEnable);
            return View(WarehouseUser);
        }

        public ActionResult Create()
        {
            ViewBag.Warehouse = db.Warehouse.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            ViewBag.AdminUser = db.AdminUser.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Create(WarehouseUser WarehouseUser , string[] PurviewList)
        {
            if (PurviewList!=null)
            {
                WarehouseUser.Purview = string.Join(";", PurviewList);
            }
            else
            {
                WarehouseUser.Purview = "";
            }
            WarehouseUser.CreateBy = UserBy;
            WarehouseUser.CreateAt = DateTime.UtcNow;
            db.WarehouseUser.Add(WarehouseUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int ID)
        {
            ViewBag.Warehouse = db.Warehouse.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            ViewBag.AdminUser = db.AdminUser.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            var WarehouseUser = db.WarehouseUser.Find(ID);
            if (WarehouseUser.IsEnable)
            {
                return View(WarehouseUser);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Edit(WarehouseUser WarehouseUser, string[] PurviewList)
        {
            var OldWarehouseUser = db.WarehouseUser.Find(WarehouseUser.ID);
            if (PurviewList != null)
            {
                OldWarehouseUser.Purview = string.Join(";", PurviewList);
            }
            else
            {
                OldWarehouseUser.Purview = "";
            }
            OldWarehouseUser.WarehouseID = WarehouseUser.WarehouseID;
            OldWarehouseUser.AdminUserID = WarehouseUser.AdminUserID;
            OldWarehouseUser.UpdateBy = UserBy;
            OldWarehouseUser.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int ID)
        {
            var OldWarehouseUser = db.WarehouseUser.Find(ID);
            OldWarehouseUser.IsEnable = false;
            OldWarehouseUser.UpdateBy = UserBy;
            OldWarehouseUser.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}