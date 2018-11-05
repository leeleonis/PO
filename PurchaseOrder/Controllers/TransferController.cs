using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class TransferController : BaseController
    {
        // GET: Transfer
        public ActionResult Index()
        {
            var Transferlist = db.Transfer.Where(x => x.IsEnable);
            return View(Transferlist);
        }
        public ActionResult Create()
        {
            var Warehouselist = db.Warehouse.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            ViewBag.Warehouselist = Warehouselist;
            return View();
        }
        [HttpPost]
        public ActionResult Create(Transfer Transfer)
        {
            var Warehouselist = db.Warehouse.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            ViewBag.Warehouselist = Warehouselist;
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Edit(int ID)
        {
            return View();
        }
        public ActionResult Delete(int ID)
        {
            return View();
        }
    }
}