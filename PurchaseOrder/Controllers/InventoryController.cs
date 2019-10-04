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
            var WarehouseVMList = new List<WarehouseVM>();
            var Warehouse = db.Warehouse.Find(WarehouseID);
            int? FulfillableMin = null;
            int? FulfillableMax = null;
            if (IsInventory)
            {
                FulfillableMin = 1;
            }
            else
            {
                FulfillableMax = 0;
            }
            if (Warehouse == null)
            {
                var WarehouseList = db.Warehouse.Where(x => x.Type != "Interim").ToList();
                foreach (var Warehouseitem in WarehouseList)
                {
                    WarehouseVMList.AddRange(GetWarehouseVMList(Warehouseitem, SKU, FulfillableMin, FulfillableMax));
                }
            }
            else
            {
                WarehouseVMList.AddRange(GetWarehouseVMList(Warehouse, SKU, FulfillableMin, FulfillableMax));
            }

            return View(WarehouseVMList);
        }
    }
}