using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class InventoryController : BaseController
    {
        // GET: Inventory
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(int? WarehouseID, string SKU, bool IsInventory)
        {
            var Warehouse = db.Warehouse.Find(WarehouseID);
            int? FulfillableMin = null;
            int? FulfillableMax = null;
            if (Warehouse==null)
            {
                Warehouse = new Warehouse();
            }
            if (IsInventory)
            {
                FulfillableMin = 1;
            }
            else
            {
                FulfillableMax = 0;
            }
            var WarehouseVMList = GetWarehouseVMList(Warehouse, SKU, FulfillableMin, FulfillableMax);
            return View(WarehouseVMList);
        }
    }
}